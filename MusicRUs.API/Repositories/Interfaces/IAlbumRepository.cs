using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Repositories.Interfaces;

public interface IAlbumRepository : IReadRepository<Album>
{
    //Task<Album?> GetAlbumWithDetailsByIdAsync(int id);
    Task<Album?> GetAlbumWithDetailsBySpotifyIdAsync(string spotifyId);
    Task<Album?> GetAlbumWithDetailsByTitleAsync(string title);
    Task<IEnumerable<Album>> GetAllAlbumsByArtistIdAsync(int artistId);
    Task<IEnumerable<Album>> SearchAlbumsByKeywordAsync(string keyword);
}