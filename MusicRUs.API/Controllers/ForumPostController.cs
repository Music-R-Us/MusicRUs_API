using Microsoft.AspNetCore.Mvc;
using MusicRUs.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using MusicRUs.API.Backend.Logic.Jwt;
using MusicRUs.API.DTOs;

namespace MusicRUs.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ForumPostController : ControllerBase
{
    private readonly IForumPostService _fpService;

    public ForumPostController(IForumPostService fpService)
    {
        _fpService  = fpService;
    }

    // Implement the endpoints for user-related operations

    // CREATE
    //##############################################################################
    // Create ForumPost
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ForumPostResponseDto>> CreateForumPost([FromBody] CreateForumPostDto dto)
    {
        try
        {
            var userId = User.GetUserId();
            if(userId == null)
                return Unauthorized();

            var fp = await _fpService.CreateForumPostAsync(userId.Value, dto);
            return CreatedAtAction(nameof(GetForumPostById), new { id = fp.Id }, fp);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // READ
    //##############################################################################
    // Get ForumPost by ID
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ForumPostResponseDto>> GetForumPostById(int id)
    {
        try
        {
            var fp = await _fpService.GetForumPostByIdAsync(id);
            return Ok(fp);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // Get ForumPosts by User ID
    [HttpGet("user/{userId:int}")]
    public async Task<ActionResult<IEnumerable<ForumPostResponseDto>>> GetForumPostsByUserId(int userId)
    {
        try
        {
            var fps = await _fpService.GetForumPostsByUserIdAsync(userId);
            return Ok(fps);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // Get ForumPosts by Tracks ID
    [HttpGet("track/{trackId:int}")]
    public async Task<ActionResult<IEnumerable<ForumPostResponseDto>>> GetForumPostsByTrackId(int trackId)
    {
        var fps = await _fpService.GetForumPostsByTrackIdAsync(trackId);
        return Ok(fps);
    }

    // Get ForumPosts by Album ID
    [HttpGet("album/{albumId:int}")]
    public async Task<ActionResult<IEnumerable<ForumPostResponseDto>>> GetForumPostsByAlbumId(int albumId)
    {
        var fps = await _fpService.GetForumPostsByAlbumIdAsync(albumId);
        return Ok(fps);
    }

    // Get ForumPosts by Artist ID
    [HttpGet("artist/{artistId:int}")]
    public async Task<ActionResult<IEnumerable<ForumPostResponseDto>>> GetForumPostsByArtistId(int artistId)
    {
        var fps = await _fpService.GetForumPostsByArtistIdAsync(artistId);
        return Ok(fps);
    }

    // Get ForumPosts by Genre ID
    [HttpGet("genre/{genreId:int}")]
    public async Task<ActionResult<IEnumerable<ForumPostResponseDto>>> GetForumPostsByGenreId(int genreId)
    {
        var fps = await _fpService.GetForumPostsByGenreIdAsync(genreId);
        return Ok(fps);
    }

    // Get all ForumPosts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ForumPostResponseDto>>> GetAllForumPosts()
    {
        var fps = await _fpService.GetAllForumPostsAsync();
        return Ok(fps);
    }

    // UPDATE
    //##############################################################################
    // Update ForumPost
    [Authorize]
    [HttpPatch("updateForumPost/{id:int}")]
    public async Task<ActionResult<ForumPostResponseDto?>> UpdateForumPost(int id, [FromBody] ForumPostUpdateDto dto)
    {
        try
        {
            var userId = User.GetUserId();
            if(userId == null)
                return Unauthorized();

            var fp = await _fpService.UpdateForumPostAsync(id, userId.Value, dto);

            return Ok(fp);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
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

    // DELETE
    //##############################################################################
    // Delete ForumPost
    [Authorize]
    [HttpDelete("deleteForumPost/{id:int}")]
    public async Task<ActionResult> DeleteForumPost(int id)
    {
        try
        {
            var userId = User.GetUserId();
            if(userId == null)
                return Unauthorized();

            await _fpService.DeleteForumPostAsync(id, userId.Value);
            return NoContent(); 
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
}