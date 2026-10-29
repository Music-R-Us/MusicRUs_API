using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MusicRUs.API.Models.Entities;

// Represents the many-to-many relationship between forum profiles and genres.
public class ProfileGenre
{
    public int ProfileId { get; set; }
    public int GenreId { get; set; }
}