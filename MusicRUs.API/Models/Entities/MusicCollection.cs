using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MusicRUs.API.Models.Entities;

// Represents a user's music collection, including the album, format, condition, and collection status
public class MusicCollection : IHasTimestamps
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!; // The user who owns this collection item

    public int AlbumId { get; set; }
    public Album Album { get; set; } = null!;

    public MediaFormat Format { get; set; }
    public MediaCondition? Condition { get; set; } // Nullable because condition may not be unknown for some items
    public CollectionStatus Status { get; set; } = CollectionStatus.Owned;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}