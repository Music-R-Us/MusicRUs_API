using Microsoft.AspNetCore.Mvc;
using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.DTOs;

namespace MusicRUs.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
     private readonly IMessageService _messageService;
     private readonly IUserService _userService;

     public MessageController(IMessageService messageService, IUserService userService)
     {
         _messageService = messageService;
         _userService = userService;
     }

    // Implement the endpoints for Message related operations

    // CREATE
    //##############################################################################
    // Send a message
    [HttpPost]
    public async Task<ActionResult<MessageResponseDto>> SendMessage(int senderId, int recipientId, [FromBody] CreateMessageDto Dto)
    {
        var senderExists = await _userService.GetUserByIdAsync(senderId); // Check if sender exists
        if (senderExists == null)
            return NotFound("Sender not found");

        var recipientExists = await _userService.GetUserByIdAsync(recipientId); // Check if recipient exists
        if (recipientExists == null)
            return NotFound("Recipient not found");

        var message = await _messageService.SendMessageAsync(senderExists.Id, recipientExists.Id, Dto);
        if (message == null)
        {
            return BadRequest("Failed to send message");
        }
        return Ok(message);
    }

    // READ
    //##############################################################################
    // Get Messages by sender and recipient Id
    [HttpGet("sender/{senderId:int}/recipient/{recipientId:int}")]
    public async Task<ActionResult<List<MessageResponseDto>>> GetMessagesBySenderAndRecipient(int senderId, int recipientId)
    {
        var senderExists = await _userService.GetUserByIdAsync(senderId); // Check if sender exists
        if (senderExists == null)
            return NotFound("Sender not found");

        var recipientExists = await _userService.GetUserByIdAsync(recipientId); // Check if recipient exists
        if (recipientExists == null)
            return NotFound("Recipient not found");

        var messages = await _messageService.GetMessagesBySenderAndRecipientIdAsync(senderId, recipientId);
        return Ok(messages);
    }

    // Get latest messages (conversation) for a specific user
    [HttpGet("latestConversation/user/{userId:int}")]
    public async Task<ActionResult<List<MessageResponseDto>>> GetLatestConversation(int userId)
    {
        var userExists = await _userService.GetUserByIdAsync(userId); // Check if user exists
        if (userExists == null)
            return NotFound("User not found");

        var messages = await _messageService.GetLatestConversationAsync(userId);
        return Ok(messages);
    }

    // UPDATE
    //##############################################################################
    // Mark a message as read
    [HttpPatch("{messageId:int}/{userId:int}")]
    public async Task<ActionResult<MessageResponseDto>> MarkMessageAsRead(int messageId, int userId)
    {
        var message = await _messageService.GetMessageByIdAsync(messageId);
        if (message == null)
            return NotFound("Message not found");

        var updatedMessage = await _messageService.MarkMessageAsReadAsync(messageId, userId);
        if (updatedMessage == null)
            return BadRequest("Failed to update message");

        return Ok(updatedMessage);
    }

    // DELETE
    //##############################################################################
    // Delete a message
    [HttpDelete("{messageId:int}/{userId:int}")]
    public async Task<ActionResult<bool>> DeleteMessage(int messageId, int userId)
    {
        var result = await _messageService.DeleteMessageAsync(messageId, userId);
        if (!result)
            return BadRequest("Failed to delete message");

        return Ok(result);
    }
}