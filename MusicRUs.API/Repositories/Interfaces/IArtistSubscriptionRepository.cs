using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Repositories.Interfaces;

public interface IArtistSubscriptionRepository : IRepository<ArtistSubscription>
{
    Task<ArtistSubscription?> GetSubscriptionByUserAndArtistAsync(int userId, int artistId);
    Task<List<ArtistSubscription>> GetAllSubscriptionsByUserAsync(int userId);
}