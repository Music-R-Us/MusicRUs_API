using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicRUs.API.Data;
using MusicRUs.API.DTOs;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Backend.Logic.Jwt;
using Microsoft.AspNetCore.Authorization;
using MusicRUs.API.Services.Interfaces;

namespace MusicRUs.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ForumProfileController : ControllerBase
{
    private readonly IForumProfileService _fpService;
    public ForumProfileController(IForumProfileService fpService)
    {
        _fpService = fpService;
    }

    // Implement the endpoints for user-related operations

    // CREATE
    //##############################################################################
    // Create ForumProfile
    [HttpPost("CreateForumProfile/{userId:int}")]
    public async Task<ActionResult<ForumProfileResponseDto>> CreateFp([FromBody] CreateForumProfileDto dto)
    {
        try
        {
            var fp = await _fpService.CreateFpAsync(dto);
            return CreatedAtAction(nameof(CreateFp), new {id = fp!.Id}, fp);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    // READ
    //##############################################################################
    // Get ForumProfile by UserId
    [HttpGet("user/{userId:int}")]
    public async Task<ActionResult<ForumProfileResponseDto>> GetFpByUserId(int userId)
    {
        try
        {
            var viewerId = User.GetUserId();
            var fp = await _fpService.GetFpByUserIdAsync(userId, viewerId);
            return Ok(fp);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<ForumProfileResponseDto>> GetFpByCurrentUser()
    {
        try
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var fp = await _fpService.GetFpByCurrentUserIdAsync(userId.Value);
            return Ok(fp);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // Search ForumProfiles by DisplayName keyword
    [HttpGet("searchForumProfiles")]
    public async Task<ActionResult<IEnumerable<ForumProfileResponseDto>>> SearchPublicFpByKeyword([FromQuery] string keyword)
    {
        var viewerId = User.GetUserId();
        return Ok(await _fpService.SearchPublicFpByKeywordAsync(keyword, viewerId));
    }

    // UPDATE
    //##############################################################################
    // Update ForumProfile
    [Authorize]
    [HttpPatch("me")]
    public async Task<ActionResult<ForumProfileResponseDto>> UpdateFpByUserId([FromBody] ForumProfileUpdatedDto dto)
    {
        try
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var fp = await _fpService.UpdateFpAsync(userId.Value, dto);
            return Ok(fp);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
}
