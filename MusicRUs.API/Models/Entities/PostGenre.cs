using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicRUs.API.Models.Entities;

// Represents the many-to-many relationship between forum posts and genres.
public class PostGenre
{
    public int PostId { get; set; } // Foreign key
    public int GenreId { get; set; } // Foreign key
}