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
    public async Task<ActionResult<ForumPostResponseDto>> CreateForumPost([FromBody] CreateForumPostDto dto)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(dto.UserId);
            if (user == null)
            {
                return NotFound($"User with ID {dto.UserId} not found.");
            }

            var fp = await _fpService.CreateForumPostAsync(dto);
            return CreatedAtAction(nameof(GetForumPostById), new { id = fp.Id }, fp);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new {error = ex.Message});
        }
    }

    // READ
    //##############################################################################
    // Get ForumPost by ID
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ForumPostResponseDto>> GetForumPostById(int id)
    {
        var fp = await _fpService.GetForumPostByIdAsync(id);
        if (fp == null)
            return NotFound();

        return Ok(fp);
    }

    // Get ForumPosts by User ID
    [HttpGet("user/{userId:int}")]
    public async Task<ActionResult<IEnumerable<ForumPostResponseDto?>>> GetForumPostsByUserId(int userId)
    {
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
            return NotFound();

        var fps = await _fpService.GetForumPostsByUserIdAsync(userId);
        return Ok(fps);
    }

    // Get ForumPosts by Tracks ID
    [HttpGet("track/{trackId:int}")]
    public async Task<ActionResult<IEnumerable<ForumPostResponseDto?>>> GetForumPostsByTrackId(int trackId)
    {
        var fps = await _fpService.GetForumPostsByTrackIdAsync(trackId);
        if (fps == null)
            return NotFound();

        return Ok(fps);
    }

    // Get ForumPosts by Album ID
    [HttpGet("album/{albumId:int}")]
    public async Task<ActionResult<IEnumerable<ForumPostResponseDto?>>> GetForumPostsByAlbumId(int albumId)
    {
        var fps = await _fpService.GetForumPostsByAlbumIdAsync(albumId);
        if (fps == null)
            return NotFound();

        return Ok(fps);
    }

    // Get ForumPosts by Artist ID
    [HttpGet("artist/{artistId:int}")]
    public async Task<ActionResult<IEnumerable<ForumPostResponseDto?>>> GetForumPostsByArtistId(int artistId)
    {
        var fps = await _fpService.GetForumPostsByArtistIdAsync(artistId);
        if (fps == null)
            return NotFound();

        return Ok(fps);
    }

    // Get ForumPosts by Genre ID
    [HttpGet("genre/{genreId:int}")]
    public async Task<ActionResult<IEnumerable<ForumPostResponseDto?>>> GetForumPostsByGenreId(int genreId)
    {
        var fps = await _fpService.GetForumPostsByGenreIdAsync(genreId);
        if (fps == null)
            return NotFound();

        return Ok(fps);
    }

    // Get all ForumPosts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ForumPostResponseDto?>>> GetAllForumPosts()
    {
        var fps = await _fpService.GetAllForumPostsAsync();
        if (fps == null)
            return NotFound();

        return Ok(fps);
    }

    // UPDATE
    //##############################################################################
    // Update ForumPost
    [HttpPatch("updateForumPost/{id:int}")]
    public async Task<ActionResult<ForumPostResponseDto?>> UpdateForumPost(int id, int userId, [FromBody] ForumPostUpdateDto dto)
    {
        try
        {
            var fp = await _fpService.UpdateForumPostAsync(id, userId, dto);
            if(fp == null)
                return NotFound();

            return Ok(fp);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new {error = ex.Message});
        }
    }

    // DELETE
    //##############################################################################
    // Delete ForumPost
    [HttpDelete("deleteForumPost/{id:int}")]
    public async Task<ActionResult> DeleteForumPost(int id)
    {
        var success = await _fpService.DeleteForumPostAsync(id);
        if(!success)
            return NotFound();

        return NoContent(); 
    }
}