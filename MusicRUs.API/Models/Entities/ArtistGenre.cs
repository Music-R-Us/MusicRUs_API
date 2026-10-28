using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicRUs.API.Models.Entities;

// Represents the many-to-many relationship between artists and genres.
public class ArtistGenre
{
    public int ArtistId { get; set; } // The ID of the artist
    public int GenreId { get; set; } // The ID of the genre
}