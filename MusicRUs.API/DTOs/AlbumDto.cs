using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.DTOs;

// Represents a Data Transfer Object (DTO) for the Album entity.
// In other words, a 'payloads' for specific operations that is sent to the API.

public class AlbumResponseDto //DTO for returning album information
{
    public int Id { get; set; }
    public string SpotifyId { get; set; } = null!;
    public string Title { get; set; } = null!;
    public AlbumType AlbumType { get; set; }
    public string? ReleaseDate { get; set; } // ReleaseDate kept as strings
    public int? TotalTracks { get; set; }
    public string? ImageUrl { get; set; }
    public string? SpotifyUrl { get; set; }
    public List<ArtistResponseDto> Artists { get; set; } = [];
}

public class AlbumDetailsResponseDto : AlbumResponseDto // Single album, include all tracks
{
    [JsonPropertyOrder(1)]
    public List<TrackResponseDto> Tracks { get; set; } = [];
}