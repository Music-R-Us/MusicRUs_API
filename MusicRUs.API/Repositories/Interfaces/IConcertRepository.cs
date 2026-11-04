using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Repositories.Interfaces;

public interface IConcertRepository
{
    Task<Artist?> GetArtistAsync(int artistId);
    Task<bool> TicketmasterIdInUseAsync(string ticketmasterId, int exceptArtistId);
    Task<List<Concert>> GetUpcomingFromTicketmasterAsync(int artistId, DateTime fromUtc);
    Task<List<Concert>> GetUpcomingAsync(int artistId, DateTime fromUtc, string? countryCode);
    void Add(Concert concert);
    void Remove(Concert concert);
    Task SaveChangesAsync();
}