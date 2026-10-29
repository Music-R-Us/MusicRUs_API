using Microsoft.EntityFrameworkCore;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;

namespace MusicRUs.API.Repositories;

public class AlbumRepository : Repository<Album>, IAlbumRepository
{
    public AlbumRepository(DbContext context) : base(context) {}

    // Private method for building a query for albums with details included
    private IQueryable<Album> IncludeDetails() => _dbSet 
        .AsNoTracking() // Ensures the query is read only
        .AsSplitQuery() // Avoids multiple joins in a single query
        .Include(a => a.Artists)
        .Include(a => a.Tracks).ThenInclude(t => t.Artists);

    //##############################

    public override async Task<Album?> GetByIdAsync(int id) // (Override) Get album by ID with details included
        => await IncludeDetails().FirstOrDefaultAsync(a => a.Id == id);

    public async Task<Album?> GetAlbumWithDetailsBySpotifyIdAsync(string spotifyId)
        => await IncludeDetails().FirstOrDefaultAsync(a => a.SpotifyId == spotifyId);

    public async Task<Album?> GetAlbumWithDetailsByTitleAsync(string title)
        => await IncludeDetails().FirstOrDefaultAsync(a => a.Title == title);

    public async Task<IEnumerable<Album>> GetAllAlbumsByArtistIdAsync(int artistId)
        => await _dbSet.AsNoTracking() // Ensures the query is read only
            .Where(a => a.Artists.Any(ar => ar.Id == artistId))
            .Include(a => a.Artists)
            .ToListAsync();
    
    public async Task<IEnumerable<Album>> SearchAlbumsByKeywordAsync(string keyword)
    {
        var pattern = keyword 
            .Replace("\\", "\\\\")
            .Replace("%", "\\%")
            .Replace("_", "\\_");

        return await _dbSet.AsNoTracking() // Ensures the query is read only
            .Where(a => EF.Functions.ILike(a.Title, $"%{pattern}%")) // Case-insensitive search by album title
            .Include(a => a.Artists)
            .ToListAsync();   
    }
}