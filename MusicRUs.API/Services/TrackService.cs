using MusicRUs.API.DTOs;
using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.Repositories.Interfaces;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Services;

public class TrackService : ITrackService
{
    private readonly ITrackRepository _trackRepo;

    public TrackService(ITrackRepository trackRepo)
    {
        _trackRepo = trackRepo;
    }

    public async Task<TrackResponseDto> GetTrackByIdAsync(int id) // READ
    {
        var track = await _trackRepo.GetByIdAsync(id);
        if (track == null) 
            return null!;
        
        return MapToDto(track);

    }

    public async Task<IEnumerable<TrackResponseDto>> SearchTracksByKeywordAsync(string keyword)
    {
        var tracks = await _trackRepo.SearchTracksByKeywordAsync(keyword);
        return tracks.Select(MapToDto).ToList();
    }

    //#########################################

    // Helper mapper ForumPost entity to ForumPostResponseDto (MapToDto)
    private TrackResponseDto MapToDto(Track track)
    {
        return new TrackResponseDto
        {
            Id = track.Id,
            SpotifyId = track.SpotifyId,
            Title = track.Title,
            TrackNumber = track.TrackNumber,
            DiscNumber = track.DiscNumber,
            DurationMs = track.DurationMs,
            Explicit = track.Explicit,
            SpotifyUrl = track.SpotifyUrl,
            Artists = track.Artists.Select(a => new ArtistIncludeDto
            {
                Id = a.Id, 
                SpotifyId = a.SpotifyId,
                Name = a.Name, 
                ImageUrl = a.ImageUrl, 
                SpotifyUrl = a.SpotifyUrl
            }).ToList(),
            Album = new AlbumIncludeDto
            {
                Id = track.Album.Id,
                SpotifyId = track.Album.SpotifyId,
                Title = track.Album.Title,
                ReleaseDate = track.Album.ReleaseDate,
                ImageUrl = track.Album.ImageUrl,
                SpotifyUrl = track.Album.SpotifyUrl
            }
        };
    }
}