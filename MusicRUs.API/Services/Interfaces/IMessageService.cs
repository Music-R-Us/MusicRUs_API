using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.DTOs;

namespace MusicRUs.API.Services.Interfaces;

public interface IMessageService
{
    Task<MessageResponseDto> SendMessageAsync(int senderId, int recipientId, CreateMessageDto dto); // CREATE
    Task<MessageResponseDto> GetMessageByIdAsync(int messageId); // READ
    Task<List<MessageResponseDto>> GetMessagesBySenderAndRecipientIdAsync(int senderId, int recipientId); // READ
    Task<List<MessageResponseDto>> GetLatestConversationAsync(int userId); // READ
    Task<MessageResponseDto> MarkMessageAsReadAsync(int messageId, int userId); // UPDATE
    Task<bool> DeleteMessageAsync(int messageId, int userId); // DELETE
}