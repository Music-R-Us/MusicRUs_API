using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicRUs.API.Models.Entities;
/// Represents an album in the system from the Spotify API. 
public class Album : IHasTimestamps
{
    public int Id { get; set; }
    public string SpotifyId { get; set; } = null!; //Spotify's 22-character ID. Unique, so re-importing never creates duplicates.
    public string Title { get; set; } = null!;
    public AlbumType AlbumType { get; set; }
    public string? ReleaseDate { get; set; }   // Can be differently formatted: "2019", "2019-05" eller "2019-05-17"
    public int? TotalTracks { get; set; }
    public string? ImageUrl { get; set; }
    public string? SpotifyUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<Track> Tracks { get; } = []; // List of tracks in the album
    public List<Artist> Artists { get; } = []; // List of artists associated with the album
}