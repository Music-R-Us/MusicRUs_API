using MusicRUs.API.DTOs;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.Repositories.Interfaces;

namespace MusicRUs.API.Services;

public class ArtistSubscriptionService : IArtistSubscriptionService
{
    private readonly IArtistSubscriptionRepository _asRepo;
    private readonly IArtistRepository _artistRepo;
    private readonly IUserRepository _userRepo;

    public ArtistSubscriptionService(
        IArtistSubscriptionRepository asRepo, 
        IArtistRepository artistRepo, 
        IUserRepository userRepo)
    {
        _asRepo = asRepo;
        _artistRepo = artistRepo;
        _userRepo = userRepo;
    }

    // Implement the contracts defined in IArtistSubscriptionService interface
    public async Task<ArtistSubscriptionResponseDto> SubscribeAsync (int userId, int artistId, CreateArtistSubscriptionDto dto) // CREATE
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        var artist = await _artistRepo.GetByIdAsync(artistId);
        if (artist == null)
            throw new KeyNotFoundException("Artist not found");

        var subscription = await _asRepo.GetSubscriptionByUserAndArtistAsync(userId, artistId);
        if(subscription != null)
            throw new InvalidOperationException("User is already subscribed to this artist");

        var newSubscription = new ArtistSubscription
        {
            UserId = userId, 
            ArtistId = artistId,
            CountryCode = dto.CountryCode?.ToUpperInvariant(),
            Region = dto.Region,
            City = dto.City,
            NotifyEmail = true,
            NotifyFeed = true
        };

        await _asRepo.AddAsync(newSubscription);
        await _asRepo.SaveChangesAsync();

        var saved = await _asRepo.GetSubscriptionByUserAndArtistAsync(userId, artistId);
        return MapToDto(saved!);
    }

    public async Task<ArtistSubscriptionResponseDto> GetSubscriptionByUserAndArtistAsync(int userId, int artistId) // READ
    {
        var subscription = await _asRepo.GetSubscriptionByUserAndArtistAsync(userId, artistId);
        if(subscription == null)
            throw new KeyNotFoundException("Subscription not found");

        return MapToDto(subscription);
    }

    public async Task<List<ArtistSubscriptionResponseDto>> GetAllSubscriptionsByUserAsync(int userId) // READ
    {
        var subscriptions = await _asRepo.GetAllSubscriptionsByUserAsync(userId);
        return subscriptions.Select(MapToDto).ToList();
    }

    public async Task<ArtistSubscriptionResponseDto> UpdateSubscriptionAsync(int userId, int artistId, ArtistSubscriptionUpdateDto dto) // UPDATE
    {
        var subscription = await _asRepo.GetSubscriptionByUserAndArtistAsync(userId, artistId);
        if (subscription == null)
            throw new KeyNotFoundException("Subscription not found");

        subscription.CountryCode = dto.CountryCode?.ToUpperInvariant(); 
        subscription.Region = dto.Region;
        subscription.City = dto.City;
        subscription.NotifyEmail = dto.NotifyEmail ?? subscription.NotifyEmail;
        subscription.NotifyFeed = dto.NotifyFeed ?? subscription.NotifyFeed;

        _asRepo.Update(subscription);
        await _asRepo.SaveChangesAsync();
        return MapToDto(subscription);
    }

    public async Task DeleteSubscriptionAsync(int userId, int artistId) // DELETE
    {
        var subscription = await _asRepo.GetSubscriptionByUserAndArtistAsync(userId, artistId);
        if (subscription == null)
            throw new KeyNotFoundException("Subscription not found");

        _asRepo.Delete(subscription);
        await _asRepo.SaveChangesAsync();
    }


//#########################################

    // Helper method to map ArtistSubscription entity to ArtistSubscriptionResponseDto (MapToDto)
    private static ArtistSubscriptionResponseDto MapToDto(ArtistSubscription subscription)
    {
        return new ArtistSubscriptionResponseDto
        {
            UserId = subscription.UserId,
            ArtistId = subscription.ArtistId,
            Artist = new ArtistIncludeDto
            {
                Id = subscription.Artist.Id,
                Name = subscription.Artist.Name,
                SpotifyId = subscription.Artist.SpotifyId,
                SpotifyUrl = subscription.Artist.SpotifyUrl,
                ImageUrl = subscription.Artist.ImageUrl
            },
            CountryCode = subscription.CountryCode,
            Region = subscription.Region,
            City = subscription.City,
            NotifyEmail = subscription.NotifyEmail,
            NotifyFeed = subscription.NotifyFeed
        };
    }
}