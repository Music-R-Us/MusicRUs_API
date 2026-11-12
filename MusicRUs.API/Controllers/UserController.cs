using Microsoft.AspNetCore.Mvc;
using MusicRUs.API.DTOs;
using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.Services;
using MusicRUs.API.Backend.Logic.Jwt;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.RateLimiting;
namespace MusicRUs.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IForumProfileService _fpService;
    private readonly ITokenService _tokenService;
    private readonly ITokenDenylist _tokenDenylist;

    public UserController(IUserService userService, IForumProfileService fpService, ITokenService tokenService, ITokenDenylist tokenDenylist)
    {
        _userService = userService;
        _fpService = fpService;
        _tokenService = tokenService;
        _tokenDenylist = tokenDenylist;
    }

    // Implement the endpoints for user-related operations

    // CREATE
    //##############################################################################
    // Create User
    [EnableRateLimiting("auth")]
    [HttpPost]
    public async Task<ActionResult<AuthResponseDto>> CreateUser([FromBody] CreateUserDto dto)
    {
        try
        {
            var user = await _userService.CreateUserAsync(dto);
            var fp = await _fpService.CreateFpAsync(new CreateForumProfileDto { UserId = user!.Id});
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, _tokenService.CreateAuthResponse(user));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    // READ
    //##############################################################################
    // Get User by ID
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponseDto?>> GetUserById(int id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user is null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("me")]
    // Get Current User by JWT token
    public async Task<ActionResult<UserResponseDto?>> GetCurrentUser()
    {
        try
        {
            var userId = User.GetUserId();
            if (userId is null)
            {
                return Unauthorized();
            }
            var user = await _userService.GetCurrentUserByIdAsync(userId.Value);
            return Ok(user);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // Get User by username and password
    [EnableRateLimiting("auth")]
    [HttpPost("login/username")]
    public async Task<ActionResult<AuthResponseDto>> GetUserByUsernameAndPassword([FromBody] UsernameLoginDto dto)
    {
        var result = await _userService.GetUserByUsernameAndPasswordAsync(dto.Username, dto.Password);
        if (result.Status == LoginStatus.AccountLocked)
            return StatusCode(StatusCodes.Status423Locked,
                new { message = "Account temporarily locked due to too many failed attempts.", lockedUntil = result.LockedUntil });
        
        if (result.Status == LoginStatus.TwoFactorRequired)
            return Accepted(_tokenService.CreateTwoFactorChallenge(result.User!.Id));

        if (result.Status != LoginStatus.Success)
            return Unauthorized("Invalid username or password.");

        return Ok(_tokenService.CreateAuthResponse(result.User!));
    }

    // Get User by email and password
    [EnableRateLimiting("auth")]
    [HttpPost("login/email")]
    public async Task<ActionResult<AuthResponseDto>> GetUserByEmailAndPassword([FromBody] EmailLoginDto dto)
    {
        var result = await _userService.GetUserByEmailAndPasswordAsync(dto.Email, dto.Password);
        if (result.Status == LoginStatus.AccountLocked)
            return StatusCode(StatusCodes.Status423Locked,
                new { message = "Account temporarily locked due to too many failed attempts.", lockedUntil = result.LockedUntil });

        if (result.Status == LoginStatus.TwoFactorRequired)
            return Accepted(_tokenService.CreateTwoFactorChallenge(result.User!.Id));
           
        if (result.Status != LoginStatus.Success)
            return Unauthorized("Invalid email or password.");

        return Ok(_tokenService.CreateAuthResponse(result.User!));
    }

    // Logout: revoke the current token until it would have expired
    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var jti = User.FindFirstValue(JwtRegisteredClaimNames.Jti);
        var exp = User.FindFirstValue(JwtRegisteredClaimNames.Exp);
        if (jti is null || !long.TryParse(exp, out var expSeconds))
            return Unauthorized();

        _tokenDenylist.Revoke(jti, DateTimeOffset.FromUnixTimeSeconds(expSeconds));
        return NoContent();
    }

    //Search users by username keyword
    [HttpGet("searchUsers")]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> SearchUsersByKeyword([FromQuery] string keyword)
    {
        var users = await _userService.SearchUsersByKeywordAsync(keyword);
        return Ok(users);
    }


    // UPDATE
    //##############################################################################
    // Update Username
    [Authorize]
    [HttpPatch("updateUsername/me")]
    public async Task<ActionResult<UserResponseDto?>> UpdateUsername([FromBody] UpdateUsernameDto dto)
    {
        try
        {
            var userId = User.GetUserId();
            if (userId is null)
                return Unauthorized();

            var user = await _userService.UpdateUsernameAsync(userId.Value, dto);
            return Ok(user);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // Update email
    [Authorize]
    [EnableRateLimiting("auth")]
    [HttpPatch("updateEmail/me")]
    public async Task<ActionResult<UserResponseDto?>> UpdateEmail([FromBody] UpdateEmailDto dto)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Unauthorized();

        var result = await _userService.UpdateEmailAsync(userId.Value, dto);
        switch (result.Error)
        {
            case UpdateUserError.NotFound:
                return NotFound();
            case UpdateUserError.WrongPassword:
                return BadRequest("Wrong current password.");
            case UpdateUserError.EmailInUse:
                return Conflict("The e-mail is already in use.");
            default:
                return Ok(result.User);
        }
    }

    // Update password
    [Authorize]
    [EnableRateLimiting("auth")]
    [HttpPatch("updatePassword/me")]
    public async Task<ActionResult<UserResponseDto?>> UpdatePassword([FromBody] UpdatePasswordDto dto)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Unauthorized();

        var result = await _userService.UpdatePasswordAsync(userId.Value, dto);
        switch (result.Error)
        {
            case UpdateUserError.NotFound:
                return NotFound();
            case UpdateUserError.WrongPassword:
                return BadRequest("Wrong current password.");
            default:
                return Ok(result.User);
        }
    }

    // DELETE
    //##############################################################################
    // Delete User by ID
    [Authorize]
    [HttpDelete("deleteUser/me")]
    public async Task<ActionResult> DeleteUserById()
    {
        try
        {
            var userId = User.GetUserId();
            if (userId is null)
                return Unauthorized();

            var jti = User.FindFirstValue(JwtRegisteredClaimNames.Jti);
            var exp = User.FindFirstValue(JwtRegisteredClaimNames.Exp);
            if (jti is null || !long.TryParse(exp, out var expSeconds))
                return Unauthorized();

            _tokenDenylist.Revoke(jti, DateTimeOffset.FromUnixTimeSeconds(expSeconds));

            var success = await _userService.DeleteUserAsync(userId.Value);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}