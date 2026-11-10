using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicRUs.API.Data;
using MusicRUs.API.DTOs;
using MusicRUs.API.Models.Entities;
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
        var fp = await _fpService.CreateFpAsync(dto);
        if(fp == null)
            return Conflict("Profile for the current user already exist.");
        return CreatedAtAction(nameof(CreateFp), new {id = fp.Id}, fp);
    }

    // READ
    //##############################################################################
    // Get ForumProfile by UserId
    [HttpGet("user/{userId:int}")]
    public async Task<ActionResult<ForumProfileResponseDto>> GetFpByUserId(int userId)
    {
        var fp = await _fpService.GetFpByUserIdAsync(userId);
        if(fp == null)
            return NotFound();

        return Ok(fp);
    }

    // Search ForumProfiles by DisplayName keyword
    [HttpGet("searchForumProfiles")]
    public async Task<ActionResult<IEnumerable<ForumProfileResponseDto>>> SearchFpByKeyword([FromQuery] string keyword)
    {
        return Ok(await _fpService.SearchFpByKeywordAsync(keyword));
    }

    // UPDATE
    //##############################################################################
    // Update ForumProfile by UserId
    [HttpPatch("user/{userId:int}")]
    public async Task<ActionResult<ForumProfileResponseDto>> UpdateFpByUserId(int userId, [FromBody] ForumProfileUpdatedDto dto)
    {
        var fp = await _fpService.UpdateFpAsync(userId, dto);

        if(fp == null)
            return NotFound();
        
        return Ok(fp);
    }
}
