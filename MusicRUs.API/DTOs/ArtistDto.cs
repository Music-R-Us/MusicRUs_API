using System.ComponentModel.DataAnnotations;

namespace MusicRUs.API.DTOs;


// Represents a Data Transfer Object (DTO) for the Artist entity.
// In other words, a 'payloads' for specific operations that is sent to the API.
public class ArtistResponseDto //DTO for returning artist information
{
    public int Id { get; set; }
    public string SpotifyId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public string? SpotifyUrl { get; set; }
}