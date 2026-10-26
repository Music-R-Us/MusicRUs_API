using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicRUs.API.Models.Entities;

// Represents a track from the Spotify API.
public class Track : IHasTimestamps
{
    public int Id { get; set; }
    public string SpotifyId { get; set; } = null!; // Spotify's 22-character ID. Unique, so re-importing never creates duplicates.
    public string Title { get; set; } = null!; 
    public int AlbumId { get; set; }
    public Album Album { get; set; } = null!; // Navigation property to the album the track belongs to
    public int? TrackNumber { get; set; }
    public int DiscNumber { get; set; } = 1;   // Standard value in C# not DB
    public int? DurationMs { get; set; }
    public bool Explicit { get; set; }
    public string? SpotifyUrl { get; set; } // URL to the track on Spotify
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; } 

    public List<Artist> Artists { get; } = []; // List of artists associated with the track (many-to-many relationship)
}