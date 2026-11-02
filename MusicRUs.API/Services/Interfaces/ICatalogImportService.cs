using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Services.Interfaces;

public interface ICatalogImportService
{
    Task<IReadOnlyList<Artist>> SearchArtistsAsync(string query);
    Task ImportArtistAlbumsAsync(int artistId);
    Task ImportAlbumTracksAsync(int albumId);
    Task<int?> ImportAlbumAsync(string spotifyId);
}