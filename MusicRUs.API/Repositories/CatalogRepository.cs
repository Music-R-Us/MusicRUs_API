using Microsoft.EntityFrameworkCore;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;

namespace MusicRUs.API.Repositories;

// Tracked queries for the Spotify import (changes are saved with SaveChangesAsync)
public class CatalogRepository : ICatalogRepository
{
    private readonly DbContext _context;

    public CatalogRepository(DbContext context) => _context = context;

    private IQueryable<Artist> TrackedArtists => _context.Set<Artist>()
        .Include(a => a.Genres);

    private IQueryable<Album> TrackedAlbums => _context.Set<Album>()
        .AsSplitQuery()
        .Include(a => a.Artists)
        .Include(a => a.Tracks);

    public Task<Artist?> GetArtistByIdAsync(int id)
        => TrackedArtists.FirstOrDefaultAsync(a => a.Id == id);

    public Task<Artist?> GetArtistBySpotifyIdAsync(string spotifyId)
        => TrackedArtists.FirstOrDefaultAsync(a => a.SpotifyId == spotifyId);

    public Task<Album?> GetAlbumByIdAsync(int id)
        => TrackedAlbums.FirstOrDefaultAsync(a => a.Id == id);

    public Task<Album?> GetAlbumBySpotifyIdAsync(string spotifyId)
        => TrackedAlbums.FirstOrDefaultAsync(a => a.SpotifyId == spotifyId);

    public Task<List<Album>> GetAlbumsBySpotifyIdsAsync(IEnumerable<string> spotifyIds)
    {
        var ids = spotifyIds.ToList();
        return TrackedAlbums.Where(a => ids.Contains(a.SpotifyId)).ToListAsync();
    }

    public Task<Genre?> GetGenreByNameAsync(string name)
        => _context.Set<Genre>().FirstOrDefaultAsync(g => g.Name == name);

    public void Add<T>(T entity) where T : class => _context.Set<T>().Add(entity);

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}