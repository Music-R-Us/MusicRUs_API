using Microsoft.AspNetCore.Mvc;
using MusicRUs.API.Backend.Logic.GoogleAuth;
using MusicRUs.API.DTOs;
using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.Backend.Logic.Jwt;
using MusicRUs.API.Services;
using Microsoft.AspNetCore.RateLimiting;

namespace MusicRUs.API.Controllers;

[ApiController]
[Route("api/google-auth")]
public class GoogleAuthController : ControllerBase
{
    private readonly IGoogleAuthService _googleAuthService;
    private readonly IUserService _userService;
    private readonly IForumProfileService _fpService;
    private readonly ITokenService _tokenService;

    public GoogleAuthController(IGoogleAuthService googleAuthService, IUserService userSrevice, IForumProfileService fpService, ITokenService tokenService)
    {
        _googleAuthService = googleAuthService;
        _userService = userSrevice;
        _fpService = fpService;
        _tokenService = tokenService;
    }
    [EnableRateLimiting("auth")]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> GoogleLogin([FromBody] GoogleLoginDto dto, [FromServices] IConfiguration config)
        {
        var googleUser = await _googleAuthService.VerifyIdTokenAsync(dto.IdToken);
        if(googleUser == null)
            return Unauthorized(new { message = "Invalid Google ID token."});
        
        var result = await _userService.GoogleLoginAsync(googleUser, config.GetValue("Signup:Enabled", true));
        if (result.Error == UpdateUserError.SignupDisabled)
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Sign-up is disabled." });
        if (result.Error == UpdateUserError.EmailInUse)
            return Conflict(new { message = "An account with this e-mail already exists. Log in with your password." });

        var user = result.User!;
       
       // Users with 2FA must also enter a code from their authenticator app
        if (user.TwoFactorEnabled)
            return Accepted(_tokenService.CreateTwoFactorChallenge(user.Id));
        
        //await _fpService.CreateFpAsync(new CreateForumProfileDto { UserId = user.Id});

        return Ok(_tokenService.CreateAuthResponse(user));
    }
} 