using Microsoft.AspNetCore.Mvc;
using MusicRUs.API.Backend.Logic.GoogleAuth;
using MusicRUs.API.DTOs;
using MusicRUs.API.Services.Interfaces;

namespace MusicRUs.API.Controllers;

[ApiController]
[Route("api/google-auth")]
public class GoogleAuthController : ControllerBase
{
    private readonly IGoogleAuthService _googleAuthService;
    private readonly IUserService _userService;
    private readonly IForumProfileService _fpService;

    public GoogleAuthController(IGoogleAuthService googleAuthService, IUserService userSrevice, IForumProfileService fpService)
    {
        _googleAuthService = googleAuthService;
        _userService = userSrevice;
        _fpService = fpService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserResponseDto>> GoogleLogin([FromBody] GoogleLoginDto dto)
    {
        var googleUser = await _googleAuthService.VerifyIdTokenAsync(dto.IdToken);
        if(googleUser == null)
            return Unauthorized(new { message = "Invalid Google ID token."});
        
        var user = await _userService.GoogleLoginAsync(googleUser);
        if(user == null)
            return StatusCode(500, new { message = "Unable to create or find user."});
        
        await _fpService.CreateFpAsync(new CreateForumProfileDto { UserId = user.Id});

        return Ok(user);
    }
} 