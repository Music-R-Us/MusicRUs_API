using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Repositories.Interfaces;

public interface IUserBlockRepository : IRepository<UserBlock>
{
    // Get specific block between two users
    Task<UserBlock?> GetBlockAsync(int blockerId, int blockedId);

    // Check if either user has blocked the other
    Task<bool> IsBlockedEitherWayAsync(int userAId, int userBId);

    // Get all blocks by a specific user
    Task<List<UserBlock>> GetBlocksByBlockerIdAsync(int blockerId);
}