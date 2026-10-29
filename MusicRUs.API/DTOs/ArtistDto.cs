using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace MusicRUs.API.DTOs;


// Represents a Data Transfer Object (DTO) for the Artist entity.
// In other words, a 'payload' for specific operations that is sent to the API.

public class ArtistResponseDto //DTO for returning artist information
{
    public int Id { get; set; }
    public string SpotifyId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public string? SpotifyUrl { get; set; }
}

public class ArtistWithAlbumsResponseDto : ArtistResponseDto // Single artist, include all albums
{
    [JsonPropertyOrder(1)]
    public List<AlbumIncludeDto> Albums { get; set; } = [];
}

public class ArtistIncludeDto //DTO for including artist information for albums
{
    public int Id { get; set; }
    public string SpotifyId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public string? SpotifyUrl { get; set; }
}