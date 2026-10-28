using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicRUs.API.Models.Entities;

// Represents a forum post created by a user, optionally linked to a track, album, or artist, and associated with genres and comments.
public class ForumPost : IHasTimestamps
{
    public int Id { get; set; }

    public int UserId { get; set; } // Foreign key to the User entity
    public User User { get; set; } = null!; // Navigation property to the User entity

    public string Title { get; set; } = string.Empty; // Title of the forum post
    public string Content { get; set; } = string.Empty; // Content of the forum post

    // Valgfrit musiklink – højst ét kan vælges per entry
    public int? TrackId { get; set; } // Foreign key to the Track entity (optional)
    public Track? Track { get; set; } // Navigation property to the Track entity
    public int? AlbumId { get; set; } // Foreign key to the Album entity (optional)
    public Album? Album { get; set; } // Navigation property to the Album entity
    public int? ArtistId { get; set; } // Foreign key to the Artist entity (optional)
    public Artist? Artist { get; set; } // Navigation property to the Artist entity

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<Comment> Comments { get; set; } = []; // Comments associated with the forum post
    public List<Genre> Genres { get; set; } = []; // Genres associated with the forum post
}