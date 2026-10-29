using MusicRUs.API.DTOs;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;
using MusicRUs.API.Services.Interfaces;
using System.Linq.Expressions;

namespace MusicRUs.API.Services;

public class ArtistService : IArtistService
{
    private readonly IArtistRepository _artistRepo;

    public ArtistService(IArtistRepository artistRepo)
    {
        _artistRepo = artistRepo;
    }

    // Implement the methods defined in IArtistService here
    public async Task<ArtistWithAlbumsResponseDto?> GetArtistByIdAsync(int id) // READ
    {
        var artist = await _artistRepo.GetByIdAsync(id);
        if(artist == null)
            return null;
        
        return MapToWithAlbumsDto(artist);
    }

    public async Task<ArtistWithAlbumsResponseDto?> GetArtistByNameAsync(string name) // READ
    {
        var artist = await _artistRepo.GetByStringAsync(name);
        if(artist == null)
            return null;

        return MapToWithAlbumsDto(artist);
    }

    public async Task<IEnumerable<ArtistResponseDto>> SearchArtistsByKeywordAsync(string keyword) // READ
    {
        var artists = await _artistRepo.SearchAsync(a => a.Name.Contains(keyword));
        return artists.Select(MapToDto).ToList();
    }

    //#########################################

    // Helper methods to map Artist entity to ArtistWithAlbumsResponseDto
    private static ArtistResponseDto MapToDto(Artist artist) => new()
    {
        Id = artist.Id,
        SpotifyId = artist.SpotifyId,
        Name = artist.Name,
        ImageUrl = artist.ImageUrl,
        SpotifyUrl = artist.SpotifyUrl
    };

    private static ArtistWithAlbumsResponseDto MapToWithAlbumsDto(Artist artist) => new()
    {
        Id = artist.Id,
        SpotifyId = artist.SpotifyId,
        Name = artist.Name,
        ImageUrl = artist.ImageUrl,
        SpotifyUrl = artist.SpotifyUrl,
        Albums = artist.Albums.Select(MapAlbum).ToList() 
    };

    private static AlbumIncludeDto MapAlbum(Album album) => new() // Map Album entity to AlbumIncludeDto
    {
        Id = album.Id,
        SpotifyId = album.SpotifyId,
        Title = album.Title,
        ImageUrl = album.ImageUrl,
        SpotifyUrl = album.SpotifyUrl
    };

}