using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicRUs.API.Models.Entities;

// A user's public forum profile. Exactly one per user, deleted with the user.
public class ForumProfile : IHasTimestamps
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Location { get; set; }
    public string? WebsiteUrl { get; set; }

    // Default set in C#, not with HasDefaultValue: EF never sends 'false' for a bool
    // with a database default, so the profile could never be made private.
    public bool IsPublic { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<FavoriteArtist> FavoriteArtists { get; } = [];
    public List<FavoriteAlbum> FavoriteAlbums { get; } = [];
}