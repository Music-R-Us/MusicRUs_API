using Microsoft.AspNetCore.Mvc;
using MusicRUs.API.DTOs;
using MusicRUs.API.Services.Interfaces;

namespace MusicRUs.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IForumProfileService _fpService;

    public UserController(IUserService userService, IForumProfileService fpService)
    {
        _userService = userService;
        _fpService = fpService;
    }

    // Implement the endpoints for user-related operations

    // CREATE
    //##############################################################################
    // Create User
    [HttpPost]
    public async Task<ActionResult<UserResponseDto?>> CreateUser([FromBody] CreateUserDto dto)
    {
        var user = await _userService.CreateUserAsync(dto);
        var fp = await _fpService.CreateFpAsync(new CreateForumProfileDto { UserId = user.Id});
        if (user is null)
        {
            return Conflict("User with the same email or username already exists.");
        }
        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
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

    // Get User by username and password
    [HttpPost("login/username")]
    public async Task<ActionResult<UserResponseDto?>> GetUserByUsernameAndPassword([FromBody] UsernameLoginDto dto)
    {
        var user = await _userService.GetUserByUsernameAndPasswordAsync(dto.Username, dto.Password);
        if (user is null)
        {
            return Unauthorized("Invalid username or password.");
        }
        return Ok(user);
    }

    // Get User by email and password
    [HttpPost("login/email")]
    public async Task<ActionResult<UserResponseDto?>> GetUserByEmailAndPassword([FromBody] EmailLoginDto dto)
    {
        var user = await _userService.GetUserByEmailAndPasswordAsync(dto.Email, dto.Password);
        if (user is null)
        {
            return Unauthorized("Invalid email or password.");
        }
        return Ok(user);
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
    [HttpPatch("updateEmail/{id:int}")]
    public async Task<ActionResult<UserResponseDto?>> UpdateEmail(int id, [FromBody] UpdateEmailDto dto)
    {
        var user = await _userService.UpdateEmailAsync(id, dto);
        if (user is null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    // Update password
    [HttpPatch("updatePassword/{id:int}")]
    public async Task<ActionResult<UserResponseDto?>> UpdatePassword(int id, [FromBody] UpdatePasswordDto dto)
    {
        var user = await _userService.UpdatePasswordAsync(id, dto);
        if (user is null)
        {
            return NotFound();
        }
        return Ok(user);
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