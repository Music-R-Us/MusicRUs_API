using MusicRUs.API.DTOs;

namespace MusicRUs.API.Services.Interfaces;

public interface IConcertService
{
    Task<IReadOnlyList<ConcertResponseDto>?> GetUpcomingConcertsAsync(int artistId, string? countryCode); // null = the artist doesn't exist

}