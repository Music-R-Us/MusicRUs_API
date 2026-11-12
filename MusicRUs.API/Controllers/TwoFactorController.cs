using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MusicRUs.API.Backend.Logic.Jwt;
using MusicRUs.API.DTOs;
using MusicRUs.API.Services;
using MusicRUs.API.Services.Interfaces;

namespace MusicRUs.API.Controllers;

[ApiController]
[Route("api/2fa")]
[Authorize] // All endpoints here require a logged-in user
public class TwoFactorController : ControllerBase
{
    private readonly ITwoFactorService _twoFactorService;
    private readonly ITokenService _tokenService;
    private readonly IUserService _userService;

    public TwoFactorController(ITwoFactorService twoFactorService, ITokenService tokenService, IUserService userService)
    {
        _twoFactorService = twoFactorService;
        _tokenService = tokenService;
        _userService = userService;
    }

    // Create a new secret and return the QR code for the authenticator app
    [HttpPost("setup")]
    public async Task<ActionResult<TwoFactorSetupDto>> Setup()
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _twoFactorService.SetupAsync(userId.Value);
        if (result.Error != TwoFactorError.None) return MapError(result.Error);
        return Ok(result.Setup);
    }

    // Turn on 2FA when the first code from the app is correct
    [EnableRateLimiting("auth")]
    [HttpPost("enable")]
    public async Task<IActionResult> Enable([FromBody] TwoFactorCodeDto dto)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();

        var error = await _twoFactorService.EnableAsync(userId.Value, dto.Code);
        return error == TwoFactorError.None ? NoContent() : MapError(error);
    }

    // Turn off 2FA. Requires a valid code, so a stolen token alone cannot disable it
    [EnableRateLimiting("auth")]
    [HttpPost("disable")]
    public async Task<IActionResult> Disable([FromBody] TwoFactorCodeDto dto)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();

        var error = await _twoFactorService.DisableAsync(userId.Value, dto.Code);
        return error == TwoFactorError.None ? NoContent() : MapError(error);
    }

    // Second step of login. Anonymous, because the user has no access token yet
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [HttpPost("verify")]
    public async Task<ActionResult<AuthResponseDto>> Verify([FromBody] TwoFactorLoginDto dto)
    {
        var userId = await _tokenService.ValidateTwoFactorTokenAsync(dto.TwoFactorToken);
        if (userId is null)
            return Unauthorized("The 2FA session has expired. Log in again.");

        var result = await _userService.VerifyTwoFactorLoginAsync(userId.Value, dto.Code);
        if (result.Status == LoginStatus.AccountLocked)
            return StatusCode(StatusCodes.Status423Locked,
                new { message = "Account temporarily locked due to too many failed attempts.", lockedUntil = result.LockedUntil });
        if (result.Status != LoginStatus.Success)
            return Unauthorized("Invalid code.");

        return Ok(_tokenService.CreateAuthResponse(result.User!));
    }

    // Translates a 2FA error to the right HTTP status code (shared by all three endpoints)
    private ActionResult MapError(TwoFactorError error) => error switch
    {
        TwoFactorError.NotFound => NotFound(),
        TwoFactorError.AlreadyEnabled => Conflict("2FA is already enabled."),
        TwoFactorError.NotSetUp => BadRequest("2FA is not set up."),
        TwoFactorError.InvalidCode => BadRequest("Invalid code."),
        _ => StatusCode(StatusCodes.Status500InternalServerError)
    };
}