using MusicRUs.API.Repositories.Interfaces;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Repositories.Interfaces;

public interface IMessageRepository : IRepository<Message>
{
    Task<List<Message>> GetMessagesBySenderAndRecipientIdAsync(int senderId, int recipientId);
    Task<List<Message>> GetLatestConversationAsync(int userId);
}