using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicRUs.API.Models.Entities;

// Represents the many-to-many relationship between tracks and artists.
public class TrackArtist
{
    public int TrackId { get; set; } // Foreign key
    public int ArtistId { get; set; } // Foreign key
}