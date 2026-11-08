using Microsoft.EntityFrameworkCore;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;

namespace MusicRUs.API.Repositories;

public class UserBlockRepository : Repository<UserBlock>, IUserBlockRepository
{

    public UserBlockRepository(DbContext context) : base(context) {}

    // Get specific block between two users
    public async Task<UserBlock?> GetBlockAsync (int blockerId, int blockedId)
        => await _dbSet.FirstOrDefaultAsync(ub => ub.BlockerId == blockerId && ub.BlockedId == blockedId);

    // Check if either user has blocked the other
    public async Task<bool> IsBlockedEitherWayAsync(int userAId, int userBId)
        => await _dbSet.AnyAsync(ub => 
            (ub.BlockerId == userAId && ub.BlockedId == userBId) || 
            (ub.BlockerId == userBId && ub.BlockedId == userAId));

    // Get all blocks by a specific user
    public async Task<List<UserBlock>> GetBlocksByBlockerIdAsync(int blockerId)
        => await _dbSet
            .Include(ub => ub.Blocked)
            .Where(ub => ub.BlockerId == blockerId)
            .ToListAsync();
}