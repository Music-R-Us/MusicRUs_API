using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicRUs.API.Models.Entities;

// Represents a music genre, which can be associated with multiple artists and forum profiles.
public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // The name of the genre   
 
    public List<Artist> Artists { get; set; } = []; // The list of artists associated with this genre
    public List<ForumProfile> Profiles { get; set; } = []; // The list of forum profiles associated with this genre
}