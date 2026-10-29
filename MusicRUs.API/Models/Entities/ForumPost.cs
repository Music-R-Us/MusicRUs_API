using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicRUs.API.Models.Entities;

// Represents a forum post created by a user, optionally linked to a track, album, or artist, and associated with genres and comments.
public class ForumPost : IHasTimestamps
{
    public int Id { get; set; }

    public int UserId { get; set; } // Foreign key
    public User User { get; set; } = null!;

    public string Title { get; set; } = string.Empty; 
    public string Content { get; set; } = string.Empty;

    public int? TrackId { get; set; } // Foreign key to the Track entity (optional)
    public Track? Track { get; set; }
    public int? AlbumId { get; set; } // Foreign key to the Album entity (optional)
    public Album? Album { get; set; }
    public int? ArtistId { get; set; } // Foreign key to the Artist entity (optional)
    public Artist? Artist { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<Comment> Comments { get; set; } = [];
    public List<Genre> Genres { get; set; } = [];
}