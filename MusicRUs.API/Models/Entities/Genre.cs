using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicRUs.API.Models.Entities;

// Represents a music genre, which can be associated with multiple artists and forum profiles.
public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
 
    public List<Artist> Artists { get; set; } = [];
    public List<ForumProfile> Profiles { get; set; } = [];
    public List<ForumPost> Posts { get; set; } = [];
}