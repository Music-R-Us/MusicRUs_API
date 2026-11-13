using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MusicRUs.API.Backend.Logic.Jwt;
using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.DTOs;

namespace MusicRUs.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
     private readonly IMessageService _messageService;

     public MessageController(IMessageService messageService)
     {
         _messageService = messageService;
     }

    // Implement the endpoints for Message related operations

    // CREATE
    //##############################################################################
    // Send a message
    [HttpPost]
    public async Task<ActionResult<MessageResponseDto>> SendMessage(int recipientId, [FromBody] CreateMessageDto Dto)
    {
        try
        {
            var senderId = User.GetUserId();
            if(senderId == null)
                return Unauthorized();

            var message = await _messageService.SendMessageAsync(senderId.Value, recipientId, Dto);
            return Ok(message);
        }
        catch (InvalidOperationException ex)
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
    // Get Messages by sender and recipient Id
    [HttpGet("sender/me/recipient/{recipientId:int}")]
    public async Task<ActionResult<List<MessageResponseDto>>> GetMessagesBySenderAndRecipient(int recipientId)
    {
        try
        {
            var senderId = User.GetUserId();
            if(senderId == null)
                return Unauthorized();

            var messages = await _messageService.GetMessagesBySenderAndRecipientIdAsync(senderId.Value, recipientId);
            return Ok(messages);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // Get latest messages (conversation) for a specific user
    [HttpGet("latestConversation/me")]
    public async Task<ActionResult<List<MessageResponseDto>>> GetLatestConversation()
    {
        try
        {
            var userId = User.GetUserId();
            if(userId == null)
                return Unauthorized();

            var messages = await _messageService.GetLatestConversationAsync(userId.Value);
            return Ok(messages);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // UPDATE
    //##############################################################################
    // Mark a message as read
    [HttpPatch("{messageId:int}/me")]
    public async Task<ActionResult<MessageResponseDto>> MarkMessageAsRead(int messageId)
    {
        try
        {
            var userId = User.GetUserId();
            if(userId == null)
                return Unauthorized();

            var updatedMessage = await _messageService.MarkMessageAsReadAsync(messageId, userId.Value);

            return Ok(updatedMessage);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // DELETE
    //##############################################################################
    // Delete a message
    [HttpDelete("{messageId:int}/me")]
    public async Task<ActionResult<bool>> DeleteMessage(int messageId)
    {
        try
        {
            var userId = User.GetUserId();
            if(userId == null)
                return Unauthorized();

            var result = await _messageService.DeleteMessageAsync(messageId, userId.Value);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}