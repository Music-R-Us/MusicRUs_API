using MusicRUs.API.DTOs;

namespace MusicRUs.API.Services.Interfaces;

public interface IArtistSubscriptionService
{
    Task<ArtistSubscriptionResponseDto> SubscribeAsync(int userId, int artistId, CreateArtistSubscriptionDto dto); // CREATE
    Task<ArtistSubscriptionResponseDto> GetSubscriptionByUserAndArtistAsync(int userId, int artistId); // READ
    Task<List<ArtistSubscriptionResponseDto>> GetAllSubscriptionsByUserAsync(int userId); // READ
    Task<ArtistSubscriptionResponseDto> UpdateSubscriptionAsync(int userId, int artistId, ArtistSubscriptionUpdateDto dto); // UPDATE
    Task DeleteSubscriptionAsync(int userId, int artistId); // DELETE
}