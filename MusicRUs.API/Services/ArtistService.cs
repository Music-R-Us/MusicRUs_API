using MusicRUs.API.DTOs;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;
using MusicRUs.API.Services.Interfaces;

namespace MusicRUs.API.Services;

public class ArtistService : IArtistService
{
    private readonly IArtistRepository _artistRepo;
    private readonly ICatalogImportService _catalogImportService;
    private readonly ILogger<ArtistService> _logger;

    public ArtistService(IArtistRepository artistRepo, ICatalogImportService catalogImportService, ILogger<ArtistService> logger)
    {
        _artistRepo = artistRepo;
        _catalogImportService = catalogImportService;
        _logger = logger;
    }

    // Implement the methods defined in IArtistService here
    public async Task<ArtistWithAlbumsResponseDto?> GetArtistByIdAsync(int id) // READ
    {
        await TryImportAsync(() => _catalogImportService.ImportArtistAlbumsAsync(id)); // Fetch releases from Spotify on first view
        var artist = await _artistRepo.GetByIdAsync(id);
        if(artist == null)
            throw new KeyNotFoundException($"Artist with ID {id} not found.");
        
        return MapToWithAlbumsDto(artist);
    }

    public async Task<ArtistWithAlbumsResponseDto?> GetArtistByNameAsync(string name) // READ
    {
        var artist = await _artistRepo.GetByStringAsync(name);
        if(artist == null)
            throw new KeyNotFoundException($"Artist with name {name} not found.");

        return MapToWithAlbumsDto(artist);
    }

    public async Task<IEnumerable<ArtistResponseDto>> SearchArtistsByKeywordAsync(string keyword) // READ
    {
        try
        {
            var artists = await _catalogImportService.SearchArtistsAsync(keyword); // Search Spotify and save the results
            return artists.Select(MapToDto).ToList();
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            _logger.LogWarning(ex, "Spotify search failed. Searching the database only.");
            var artists = await _artistRepo.SearchAsync(a => a.Name.Contains(keyword));
            return artists.Select(MapToDto).ToList();
        }
    }

    private async Task TryImportAsync(Func<Task> import)
    {
        try
        {
            await import();
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            _logger.LogWarning(ex, "Could not fetch artist data from Spotify.");
        }
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
        Albums = artist.Albums
            .OrderByDescending(a => a.ReleaseDate) // Newest first
            .Select(MapAlbum)
            .ToList()
    };

    private static AlbumIncludeDto MapAlbum(Album album) => new() // Map Album entity to AlbumIncludeDto
    {
        Id = album.Id,
        SpotifyId = album.SpotifyId,
        Title = album.Title,
        AlbumType = album.AlbumType,
        ReleaseDate = album.ReleaseDate,
        TotalTracks = album.TotalTracks,
        ImageUrl = album.ImageUrl,
        SpotifyUrl = album.SpotifyUrl
    };
}