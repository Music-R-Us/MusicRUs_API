using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicRUs.API.Models.Entities;

// Represents a subscription of a user to an artist, including notification preferences and location filters.
public class ArtistSubscription : IHasTimestamps
{
    public int UserId { get; set; } // Foreign key
    public User User { get; set; } = null!;

    public int ArtistId { get; set; } // Foreign key
    public Artist Artist { get; set; } = null!;

    public string? CountryCode { get; set; } // Empty means "all" (UC-012)
    public string? Region { get; set; }
    public string? City { get; set; }
    
    public bool NotifyEmail { get; set; } = true;
    public bool NotifyFeed { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}