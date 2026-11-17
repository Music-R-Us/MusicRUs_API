using Microsoft.AspNetCore.Mvc;
using MusicRUs.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using MusicRUs.API.Backend.Logic.Jwt;
using MusicRUs.API.DTOs;

namespace MusicRUs.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    // Implement the endpoints for comment related operations

    // CREATE
    //##############################################################################
    // Create a comment
    [HttpPost("forumpost/{postId:int}")]
    [Authorize]
    public async Task<ActionResult<CommentResponseDto>> CreateComment(int postId, CreateCommentDto dto)
    {
        try
        {
            var viewerId = User.GetUserId();
            var userId = User.GetUserId();
            if(userId == null)
                return Unauthorized();

            var comment = await _commentService.CreateCommentAsync(postId, userId.Value, viewerId, dto);
            return CreatedAtAction(nameof(GetCommentById), new { id = comment.Id }, comment);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // READ
    //##############################################################################

    // Get comment by ID
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentResponseDto>> GetCommentById(int id)
    {
        try
        {
            var viewerId = User.GetUserId();
            var comment = await _commentService.GetCommentByIdAsync(id, viewerId);
            return Ok(comment);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // Get all comments for a forum post
    [HttpGet("forumpost/{postId:int}")]
    public async Task<ActionResult<IEnumerable<CommentResponseDto>>> GetAllCommentsByForumPostId(int postId)
    {
        try
        {
            var viewerId = User.GetUserId();
            var comments = await _commentService.GetAllCommentsByForumPostIdAsync(postId, viewerId);
            return Ok(comments);   
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // UPDATE
    //##############################################################################
    // Update comment
    [HttpPatch("{id:int}")]
    [Authorize]
    public async Task<ActionResult<CommentResponseDto>> UpdateComment(int id, CommentUpdateDto dto)
    {
        try
        {
            var userId = User.GetUserId();
            if(userId == null)
                return Unauthorized();

            var comment = await _commentService.UpdateCommentAsync(id, userId.Value, dto);
            return Ok(comment);
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
    // Delete comment
    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<ActionResult<bool>> DeleteComment(int id)
    {
        try
        {
            var userId = User.GetUserId();
            if(userId == null)
                return Unauthorized();

            await _commentService.DeleteCommentAsync(id, userId.Value);
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