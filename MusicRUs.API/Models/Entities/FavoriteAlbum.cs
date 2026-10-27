using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicRUs.API.Models.Entities;

/// An album pinned to a profile. DisplayOrder sets the order shown on the profile.
public class FavoriteAlbum
{
    public int ProfileId { get; set; }
    public ForumProfile Profile { get; set; } = null!;
    public int AlbumId { get; set; }
    public Album Album { get; set; } = null!;
    public int DisplayOrder { get; set; }
}