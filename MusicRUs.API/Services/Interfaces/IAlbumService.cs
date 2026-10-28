using MusicRUs.API.DTOs;

namespace MusicRUs.API.Services.Interfaces;

public interface IAlbumService
{
    Task<AlbumDetailsResponseDto?> GetAlbumByIdAsync(int id); // READ
    Task<AlbumDetailsResponseDto?> GetAlbumBySpotifyIdAsync(string spotifyId); // READ
    Task<AlbumDetailsResponseDto?> GetAlbumByTitleAsync(string title); // READ
    Task<IEnumerable<AlbumResponseDto>> GetAlbumsByArtistIdAsync(int artistId); // READ
    Task<IEnumerable<AlbumResponseDto>> SearchAlbumsByKeywordAsync(string keyword); // READ
}