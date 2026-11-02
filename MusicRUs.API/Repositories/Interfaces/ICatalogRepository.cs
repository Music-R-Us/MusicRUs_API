using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Repositories.Interfaces;

// Used by CatalogImportService to save data from Spotify
public interface ICatalogRepository
{
    Task<Artist?> GetArtistByIdAsync(int id);
    Task<Artist?> GetArtistBySpotifyIdAsync(string spotifyId);
    Task<Album?> GetAlbumByIdAsync(int id);
    Task<Album?> GetAlbumBySpotifyIdAsync(string spotifyId);
    Task<List<Album>> GetAlbumsBySpotifyIdsAsync(IEnumerable<string> spotifyIds);
    Task<Genre?> GetGenreByNameAsync(string name);
    void Add<T>(T entity) where T : class;
    Task SaveChangesAsync();
}