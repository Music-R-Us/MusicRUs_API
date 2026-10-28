using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicRUs.API.Models.Entities;

// Represents a subscription of a user to an artist, including notification preferences and location filters.
public class ArtistSubscription : IHasTimestamps
{
    public int UserId { get; set; } // Foreign key to the User entity
    public User User { get; set; } = null!; // Navigation property to the User entity

    public int ArtistId { get; set; } // Foreign key to the Artist entity
    public Artist Artist { get; set; } = null!; // Navigation property to the Artist entity

    // Tom betyder "alle" (UC-012)
    public string? CountryCode { get; set; } // Empty means "all" (UC-012)
    public string? Region { get; set; } // Region or state of the concert location
    public string? City { get; set; } // City of the concert location
    
    public bool NotifyEmail { get; set; } = true; // Whether to notify the user via email
    public bool NotifyFeed { get; set; } = true; // Whether to notify the user via the feed

    public DateTime CreatedAt { get; set; } // Timestamp when the subscription was created
    public DateTime UpdatedAt { get; set; } // Timestamp when the subscription was last updated
}