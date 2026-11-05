

namespace MusicRUs.API.DTOs;


// Represents a Data Transfer Object (DTO) for the Track entity.
// In other words, a 'payload' for specific operations that is sent to the API.
public class TrackResponseDto //DTO for returning track information
{
    public int Id { get; set; }
    public string SpotifyId { get; set; } = null!;
    public string Title { get; set; } = null!;
    public int? TrackNumber { get; set; }
    public int DiscNumber { get; set; } 
    public int? DurationMs { get; set; } // Duration in milliseconds
    public bool Explicit { get; set; }
    public string? SpotifyUrl { get; set; }
    public List<ArtistIncludeDto> Artists { get; set; } = [];
    public AlbumIncludeDto Album { get; set; } = null!;
}

public class TrackIncludeDto // DTO for including related track information
{
    public int Id { get; set; }
    public string SpotifyId { get; set; } = null!;
    public string Title { get; set; } = null!;
    public int? TrackNumber { get; set; }
    public int DiscNumber { get; set; } 
    public int? DurationMs { get; set; } // Duration in milliseconds
    public bool Explicit { get; set; }
    public string? SpotifyUrl { get; set; }
    public List<ArtistIncludeDto> Artists { get; set; } = [];
}