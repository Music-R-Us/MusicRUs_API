using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicRUs.API.Models.Entities;

// Represents a concert event for a specific artist, including venue and timing details.
public class Concert : IHasTimestamps
{
    public int Id { get; set; }

    public int ArtistId { get; set; }  // Foreign key to the associated artist
    public Artist Artist { get; set; } = null!; // Navigation property to the associated artist

    public string? ExternalId { get; set; } // External ID from the source system
    public string Name { get; set; } = string.Empty; // Name of the concert
    public string VenueName { get; set; } = string.Empty; // Name of the venue
    public string? City { get; set; } // City where the concert takes place
    public string? Region { get; set; } // Region or state of the concert location
    public string? CountryCode { get; set; }  // Country code of the concert location
    public DateTime StartsAt { get; set; } // Start time of the concert (UTC)
    public string? VenueTimeZone { get; set; } // Time zone of the venue
    public string? Url { get; set; }  // URL for the concert event

    public DateTime CreatedAt { get; set; } // Timestamp when the concert record was created
    public DateTime UpdatedAt { get; set; } // Timestamp when the concert record was last updated
}