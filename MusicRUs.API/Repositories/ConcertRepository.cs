using Microsoft.EntityFrameworkCore;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;

namespace MusicRUs.API.Repositories;

public class ConcertRepository : IConcertRepository
{
    private readonly DbContext _context;

    public ConcertRepository(DbContext context) => _context = context;

    public Task<Artist?> GetArtistAsync(int artistId) // Tracked, so new TicketmasterId is saved via SaveChangesAsync
        => _context.Set<Artist>().FirstOrDefaultAsync(a => a.Id == artistId);

    public Task<bool> TicketmasterIdInUseAsync(string ticketmasterId, int exceptArtistId)
        => _context.Set<Artist>().AnyAsync(a => a.TicketmasterId == ticketmasterId && a.Id != exceptArtistId);

    public Task<List<Concert>> GetUpcomingFromTicketmasterAsync(int artistId, DateTime fromUtc) // Tracked concerts from Ticketmaster that maybe has been updated or removed

        => _context.Set<Concert>()
            .Where(c => c.ArtistId == artistId && c.ExternalId != null && c.StartsAt >= fromUtc)
            .ToListAsync();

    public Task<List<Concert>> GetUpcomingAsync(int artistId, DateTime fromUtc, string? countryCode)
        => _context.Set<Concert>()
            .AsNoTracking()
            .Where(c => c.ArtistId == artistId && c.StartsAt >= fromUtc)
            .Where(c => countryCode == null || c.CountryCode == countryCode)
            .OrderBy(c => c.StartsAt)
            .ToListAsync();

    public void Add(Concert concert) => _context.Set<Concert>().Add(concert);

    public void Remove(Concert concert) => _context.Set<Concert>().Remove(concert);

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}