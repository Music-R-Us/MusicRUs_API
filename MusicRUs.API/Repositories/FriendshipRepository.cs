using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MusicRUs.API.Repositories;

public class FriendshipRepository : Repository<Friendship>,IFriendshipRepository
{
    public FriendshipRepository(DbContext context) : base(context) {}

    public override async Task<Friendship?> GetByIdAsync(int id)
        => await WithUsers().FirstOrDefaultAsync(f => f.Id == id);

    public async Task<Friendship?> GetFsByRequesterAndAddresseeIdAsync(int requesterId, int addresseeId)
        => await WithUsers().FirstOrDefaultAsync(
            f => f.RequesterId == requesterId && 
            f.AddresseeId == addresseeId
        );

    public async Task<List<Friendship>> GetFsListByUserIdAsync(int userId)
        => await WithUsers().Where(
            f => (f.RequesterId == userId || f.AddresseeId == userId) &&
            (f.Status == FriendshipStatus.Accepted || f.Status == FriendshipStatus.Pending)
        ).ToListAsync();

    public async Task<List<Friendship>> GetPendingFsListByUserIdAsync(int userId)
        => await WithUsers().Where(
            f => f.AddresseeId == userId && f.Status == FriendshipStatus.Pending
        ).ToListAsync();

    //####################################
    // Include related users in queries
    private IQueryable<Friendship> WithUsers()
        => _dbSet.Include(f => f.Requester).Include(f => f.Addressee);
}