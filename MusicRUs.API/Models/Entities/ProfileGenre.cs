using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MusicRUs.API.Models.Entities;

// Represents the many-to-many relationship between forum profiles and genres.
public class ProfileGenre
{
    public int ProfileId { get; set; } // The ID of the profile
    public int GenreId { get; set; } // The ID of the genre
}