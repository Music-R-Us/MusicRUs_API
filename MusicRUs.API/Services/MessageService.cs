using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.DTOs;
using MusicRUs.API.Repositories.Interfaces;

namespace MusicRUs.API.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepo;
    private readonly IUserBlockRepository _userBlockRepo;
    public MessageService(IMessageRepository messageRepo, IUserBlockRepository userBlockRepo)
    {
        _messageRepo = messageRepo;
        _userBlockRepo = userBlockRepo;
    }

    public async Task<MessageResponseDto> SendMessageAsync(int senderId, int recipientId, CreateMessageDto dto) // CREATE
    {
        var isBlocked = await _userBlockRepo.IsBlockedEitherWayAsync(senderId, recipientId);
        if (isBlocked)
            throw new InvalidOperationException("Cannot send message. Either you or the recepient is blocked.");

        var message = new Message
        {
            SenderId = senderId,
            RecipientId = recipientId,
            Content = dto.Content,
        };

        await _messageRepo.AddAsync(message);
        await _messageRepo.SaveChangesAsync();
        var saved = await _messageRepo.GetByIdAsync(message.Id);
        return MapToDto(saved!);
    }

    public async Task<MessageResponseDto> GetMessageByIdAsync(int id)
    {
        var message = await _messageRepo.GetByIdAsync(id);
        if (message == null)
            throw new Exception("Message not found.");

        return MapToDto(message);
    }

    public async Task<List<MessageResponseDto>> GetMessagesBySenderAndRecipientIdAsync(int senderId, int recipientId) // READ
    {
        var messages = await _messageRepo.GetMessagesBySenderAndRecipientIdAsync(senderId, recipientId);
        return messages.Select(MapToDto).ToList();
    }

    public async Task<List<MessageResponseDto>> GetLatestConversationAsync(int userId) // READ
    {
        var messages = await _messageRepo.GetLatestConversationAsync(userId);
        return messages.Select(MapToDto).ToList();
    }

    public async Task<MessageResponseDto> MarkMessageAsReadAsync(int messageId, int userId) // UPDATE
    {
        var message = await _messageRepo.GetByIdAsync(messageId);
        if (message == null || message.RecipientId != userId)
            //throw new Exception("Message not found or access denied.");
            return null!;

        message.ReadAt = DateTime.UtcNow;
        await _messageRepo.SaveChangesAsync();
        var saved = await _messageRepo.GetByIdAsync(messageId);
        return MapToDto(saved!);
    }

    public async Task<bool> DeleteMessageAsync(int messageId, int userId) // DELETE
    {
        var message = await _messageRepo.GetByIdAsync(messageId);
        if (message == null || message.RecipientId != userId)
            throw new Exception("Message not found or access denied.");

        _messageRepo.Delete(message);
        await _messageRepo.SaveChangesAsync();
        return true;
    }
    //#########################################

    // Helper method to map Message entity to MessageResponseDto (MapToDto)
    private static MessageResponseDto MapToDto(Message message) => new()
    {
        Id = message.Id,
        SenderId = message.SenderId,
        Sender = message.Sender != null ? new UserIncludeDto { Id = message.Sender.Id, Username = message.Sender.Username } : null!,
        RecipientId = message.RecipientId,
        Recipient = message.Recipient != null ? new UserIncludeDto { Id = message.Recipient.Id, Username = message.Recipient.Username } : null!,
        Content = message.Content,
        ReadAt = message.ReadAt,
        UpdatedAt = message.UpdatedAt,
        CreatedAt = message.CreatedAt
    };
}