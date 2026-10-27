using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MusicRUs.API.Models.Entities;

// Represents a user's music collection, including the album, format, condition, and collection status
public class MusicCollection : IHasTimestamps
{
    public int Id { get; set; }

    public int UserId { get; set; } // The ID of the user who owns this collection item
    public User User { get; set; } = null!; // The user who owns this collection item

    public int AlbumId { get; set; } // The ID of the album associated with this collection item
    public Album Album { get; set; } = null!; // The album associated with this collection item

    public MediaFormat Format { get; set; } // Represents the format of the media item (e.g., vinyl, CD, cassette, digital)
    public MediaCondition? Condition { get; set; } // Nullable because condition may not be unknown for some items
    public CollectionStatus Status { get; set; } = CollectionStatus.Owned; // Default status is Owned

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}