using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicRUs.API.Models.Entities;

/// An artist pinned to a profile. DisplayOrder sets the order shown on the profile.
public class FavoriteArtist
{
    public int ProfileId { get; set; }
    public ForumProfile Profile { get; set; } = null!;
    public int ArtistId { get; set; }
    public Artist Artist { get; set; } = null!;
    public int DisplayOrder { get; set; }
}