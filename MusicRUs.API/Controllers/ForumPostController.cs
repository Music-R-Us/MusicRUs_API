using Microsoft.AspNetCore.Mvc;
using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.DTOs;

namespace MusicRUs.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ForumPostController : ControllerBase
{
    private readonly IForumPostService _fpService;
    private readonly IUserService _userService;

    public ForumPostController(IForumPostService fpService, IUserService userService)
    {
        _fpService  = fpService;
        _userService = userService;
    }

    // Implement the endpoints for user-related operations

    // CREATE
    //##############################################################################
    // Create ForumPost
    [HttpPost]
    public async Task<IActionResult> CreateForumPost([FromBody] CreateForumPostDto dto)
    {
        var user = await _userService.GetUserByIdAsync(dto.UserId);
        if (user == null)
        {
            return NotFound($"User with ID {dto.UserId} not found.");
        }

        var fp = await _fpService.CreateForumPostAsync(dto);
        return CreatedAtAction(nameof(GetForumPostById), new { id = fp.Id }, fp);
    }

    // READ
    //##############################################################################
    // Get ForumPost by ID
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetForumPostById(int id)
    {
        var fp = await _fpService.GetForumPostByIdAsync(id);
        if (fp == null)
            return NotFound();

        return Ok(fp);
    }

    // Get ForumPosts by User ID
    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetForumPostsByUserId(int userId)
    {
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
            return NotFound();

        var fps = await _fpService.GetForumPostsByUserIdAsync(userId);
        return Ok(fps);
    }

    // Get ForumPosts by Tracks ID
    [HttpGet("track/{trackId:int}")]
    public async Task<IActionResult> GetForumPostsByTrackId(int trackId)
    {
        var fps = await _fpService.GetForumPostsByTrackIdAsync(trackId);
        if (fps == null)
            return NotFound();

        return Ok(fps);
    }

    // Get ForumPosts by Album ID
    [HttpGet("album/{albumId:int}")]
    public async Task<IActionResult> GetForumPostsByAlbumId(int albumId)
    {
        var fps = await _fpService.GetForumPostsByAlbumIdAsync(albumId);
        if (fps == null)
            return NotFound();

        return Ok(fps);
    }

    // Get ForumPosts by Artist ID
    [HttpGet("artist/{artistId:int}")]
    public async Task<IActionResult> GetForumPostsByArtistId(int artistId)
    {
        var fps = await _fpService.GetForumPostsByArtistIdAsync(artistId);
        if (fps == null)
            return NotFound();

        return Ok(fps);
    }

    // Get all ForumPosts
    [HttpGet]
    public async Task<IActionResult> GetAllForumPosts()
    {
        var fps = await _fpService.GetAllForumPostsAsync();
        if (fps == null)
            return NotFound();

        return Ok(fps);
    }
}