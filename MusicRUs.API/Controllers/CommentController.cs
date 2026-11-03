using Microsoft.AspNetCore.Mvc;
using MusicRUs.API.Services.Interfaces;
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
    [HttpPost]
    public async Task<ActionResult<CommentResponseDto>> CreateComment(int postId, CreateCommentDto dto)
    {
        var comment = await _commentService.CreateCommentAsync(postId, dto);
        return Ok(comment);
    }

    // READ
    //##############################################################################

    // Get comment by ID
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentResponseDto>> GetCommentById(int id)
    {
        var comment = await _commentService.GetCommentByIdAsync(id);
        return Ok(comment);
    }

    // Get all comments for a forum post
    [HttpGet("forumpost/{postId:int}")]
    public async Task<ActionResult<IEnumerable<CommentResponseDto>>> GetAllCommentsByForumPostId(int postId)
    {
        var comments = await _commentService.GetAllCommentsByForumPostIdAsync(postId);
        return Ok(comments);
    }

    // UPDATE
    //##############################################################################
    // Update comment
    [HttpPatch("{id:int}")]
    public async Task<ActionResult<CommentResponseDto>> UpdateComment(int id, CommentUpdateDto dto)
    {
        var comment = await _commentService.UpdateCommentAsync(id, dto);
        return Ok(comment);
    }
    
    // DELETE
    //##############################################################################
    // Delete comment
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<bool>> DeleteComment(int id)
    {
        var result = await _commentService.DeleteCommentAsync(id);
        return Ok(result);
    }
}