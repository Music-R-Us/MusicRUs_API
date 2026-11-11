using MusicRUs.API.DTOs;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;
using MusicRUs.API.Services.Interfaces;

namespace MusicRUs.API.Services;

public class AlbumService : IAlbumService
{
    private readonly IAlbumRepository _albumRepo;
    private readonly ICatalogImportService _catalogImportService;
    private readonly ILogger<AlbumService> _logger;

    public AlbumService(IAlbumRepository albumRepo, ICatalogImportService catalogImportService, ILogger<AlbumService> logger)
    {
        _albumRepo = albumRepo;
        _catalogImportService = catalogImportService;
        _logger = logger;
    }

    // Implement the methods defined in IAlbumService here
    public async Task<AlbumDetailsResponseDto?> GetAlbumByIdAsync(int id) // READ
    {
        await TryImportAsync(() => _catalogImportService.ImportAlbumTracksAsync(id)); // Fetch tracks from Spotify on first view
        var album = await _albumRepo.GetByIdAsync(id); // Overriden to include artists and tracks
        if(album == null)
            throw new KeyNotFoundException($"Album with ID {id} not found.");
        
        return MapToDetailsDto(album);
    }

    public async Task<AlbumDetailsResponseDto?> GetAlbumBySpotifyIdAsync(string spotifyId) // READ
    {
        var album = await _albumRepo.GetAlbumWithDetailsBySpotifyIdAsync(spotifyId);
        if (album == null || album.Tracks.Count == 0)
        {
            await TryImportAsync(() => _catalogImportService.ImportAlbumAsync(spotifyId)); // Fetch from Spotify if missing
            album = await _albumRepo.GetAlbumWithDetailsBySpotifyIdAsync(spotifyId);
        }
        if(album == null)
            return null;
        
        return MapToDetailsDto(album);
    }

    public async Task<AlbumDetailsResponseDto?> GetAlbumByTitleAsync(string title) // READ
    {
        var album = await _albumRepo.GetAlbumWithDetailsByTitleAsync(title);
        if(album == null)
            return null;

        return MapToDetailsDto(album);
    } 

    public async Task<IEnumerable<AlbumResponseDto>> GetAlbumsByArtistIdAsync(int artistId) // READ
    {
        var albums = await _albumRepo.GetAllAlbumsByArtistIdAsync(artistId);
        return albums.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<AlbumResponseDto>> SearchAlbumsByKeywordAsync(string keyword) // READ
    {
        try
        {
            var albums = await _catalogImportService.SearchAlbumsAsync(keyword);
            return albums.Select(MapToDto).ToList();
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            _logger.LogWarning(ex, "Spotify search failed. Searching the database only.");
            var albums = await _albumRepo.SearchAlbumsByKeywordAsync(keyword);
            return albums.Select(MapToDto).ToList();
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
            _logger.LogWarning(ex, "Could not fetch album data from Spotify.");
        }
    }


    //#########################################

    // Helper methods to map Album entity to AlbumResponseDto / AlbumDetailsResponseDto 
    private static T MapAlbum<T>(Album album) where T : AlbumResponseDto, new() => new() 
    {
        Id = album.Id,
        SpotifyId = album.SpotifyId,
        Title = album.Title,
        AlbumType = album.AlbumType,
        ReleaseDate = album.ReleaseDate,
        TotalTracks = album.TotalTracks,
        ImageUrl = album.ImageUrl,
        SpotifyUrl = album.SpotifyUrl,
        Artists = album.Artists.Select(MapArtist).ToList()
    };

    private static AlbumResponseDto MapToDto(Album album) // Album with artists
        => MapAlbum<AlbumResponseDto>(album);

    private static AlbumDetailsResponseDto MapToDetailsDto(Album album) // Album with artists and tracks
    {
        var dto = MapAlbum<AlbumDetailsResponseDto>(album); 
        dto.Tracks = album.Tracks
            .OrderBy(t => t.DiscNumber).ThenBy(t => t.TrackNumber)
            .Select(MapTrack)
            .ToList();
        return dto;
    }
   
    private static ArtistIncludeDto MapArtist(Artist artist) => new()
    {
        Id = artist.Id,
        SpotifyId = artist.SpotifyId,
        Name = artist.Name,
        ImageUrl = artist.ImageUrl,
        SpotifyUrl = artist.SpotifyUrl
    };

    
    private static TrackResponseDto MapTrack(Track track) => new()
    {
        Id = track.Id,
        SpotifyId = track.SpotifyId,
        Title = track.Title,
        TrackNumber = track.TrackNumber,
        DiscNumber = track.DiscNumber,
        DurationMs = track.DurationMs,
        Explicit = track.Explicit,
        SpotifyUrl = track.SpotifyUrl,
        Artists = track.Artists.Select(MapArtist).ToList()
    };
}