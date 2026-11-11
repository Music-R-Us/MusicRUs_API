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
        var user = await _userService.CreateUserAsync(dto);
        if (user is null)
        {
            return Conflict("User with the same email or username already exists.");
        }
        var fp = await _fpService.CreateFpAsync(new CreateForumProfileDto { UserId = user.Id});
        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, _tokenService.CreateAuthResponse(user));
    }

    // READ
    //##############################################################################
    // Get User by ID
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponseDto?>> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [Authorize]
    [HttpGet("me")]
    // Get Current User by JWT token
    public async Task<ActionResult<UserResponseDto?>> GetCurrentUser()
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }
        var user = await _userService.GetUserByIdAsync(userId.Value);
        if (user is null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    // Get User by username and password
    [EnableRateLimiting("auth")]
    [HttpPost("login/username")]
    public async Task<ActionResult<AuthResponseDto>> GetUserByUsernameAndPassword([FromBody] UsernameLoginDto dto)
    {
        var user = await _userService.GetUserByUsernameAndPasswordAsync(dto.Username, dto.Password);
        if (user is null)
        {
            return Unauthorized("Invalid username or password.");
        }
        return Ok(_tokenService.CreateAuthResponse(user));
    }

    // Get User by email and password
    [EnableRateLimiting("auth")]
    [HttpPost("login/email")]
    public async Task<ActionResult<AuthResponseDto>> GetUserByEmailAndPassword([FromBody] EmailLoginDto dto)
    {
        var user = await _userService.GetUserByEmailAndPasswordAsync(dto.Email, dto.Password);
        if (user is null)
        {
            return Unauthorized("Invalid email or password.");
        }
        return Ok(_tokenService.CreateAuthResponse(user));
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
    [HttpPatch("updateUsername/{id:int}")]
    public async Task<ActionResult<UserResponseDto?>> UpdateUsername(int id, [FromBody] UpdateUsernameDto dto)
    {
        var user = await _userService.UpdateUsernameAsync(id, dto);
        if (user is null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    // Update email
    [EnableRateLimiting("auth")]
    [HttpPatch("updateEmail/{id:int}")]
    public async Task<ActionResult<UserResponseDto?>> UpdateEmail(int id, [FromBody] UpdateEmailDto dto)
    {
        var result = await _userService.UpdateEmailAsync(id, dto);
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
    [EnableRateLimiting("auth")]
    [HttpPatch("updatePassword/{id:int}")]
    public async Task<ActionResult<UserResponseDto?>> UpdatePassword(int id, [FromBody] UpdatePasswordDto dto)
    {
        var result = await _userService.UpdatePasswordAsync(id, dto);
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
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteUserById(int id)
    {
        var success = await _userService.DeleteUserAsync(id);
        if (!success)
        {
            return NotFound();
        }
        return NoContent();
    }
}