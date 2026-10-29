using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicRUs.API.Models.Entities;

// Represents a concert event for a specific artist, including venue and timing details.
public class Concert : IHasTimestamps
{
    public int Id { get; set; }

    public int ArtistId { get; set; }  // Foreign key
    public Artist Artist { get; set; } = null!;
    public string? ExternalId { get; set; } // External ID from the source system
    public string Name { get; set; } = string.Empty; 
    public string VenueName { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? CountryCode { get; set; }
    public DateTime StartsAt { get; set; }
    public string? VenueTimeZone { get; set; }
    public string? Url { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; } 
}