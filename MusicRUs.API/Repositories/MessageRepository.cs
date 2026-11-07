using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace MusicRUs.API.Repositories;

public class MessageRepository : Repository<Message>, IMessageRepository
{

    public MessageRepository(DbContext context) : base(context) {}

    private IQueryable<Message> WithUsers() // Include sender and recipient users
    => _dbSet
        .Include(m => m.Sender)
        .Include(m => m.Recipient);



    public async Task<List<Message>> GetMessagesBySenderAndRecipientIdAsync(int senderId, int recipientId)
        => await WithUsers()
            .Where(m => (m.SenderId == senderId && m.RecipientId == recipientId) || 
                (m.SenderId == recipientId && m.RecipientId == senderId))
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

    public async Task<List<Message>> GetLatestConversationAsync(int userId) // Get the latest conversation for the user
    {
        // Find the latest Id of each Conversations
        var latestIds = await _dbSet
            .Where(m => m.RecipientId == userId || m.SenderId == userId)
            .GroupBy(m => m.SenderId == userId ? m.RecipientId : m.SenderId)
            .Select(g => g.Max(m => m.Id))
            .ToListAsync();

        // Get latest messages based on the latest Ids
        return await WithUsers()
            .Where(m => latestIds.Contains(m.Id))
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }
}