using MusicRUs.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using MusicRUs.API.Repositories.Interfaces;

namespace MusicRUs.API.Repositories;


public class ArtistSubscriptionRepository : Repository<ArtistSubscription>, IArtistSubscriptionRepository
{
    public ArtistSubscriptionRepository(DbContext context) : base(context) {}

    public async Task<ArtistSubscription?> GetSubscriptionByUserAndArtistAsync(int userId, int artistId)
        => await _dbSet
            .Include(s => s.Artist)
            .FirstOrDefaultAsync(s => s.UserId == userId && s.ArtistId == artistId);

    public async Task<List<ArtistSubscription>> GetAllSubscriptionsByUserAsync(int userId)
        => await _dbSet
            .Include(s => s.Artist)
            .Where(s => s.UserId == userId)
            .ToListAsync();
}