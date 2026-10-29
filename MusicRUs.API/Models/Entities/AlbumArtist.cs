using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicRUs.API.Models.Entities;

// Represents the many-to-many relationship between albums and artists. 
public class AlbumArtist
{
    public int AlbumId { get; set; } // Foreign key
    public int ArtistId { get; set; } // Foreign key
}
