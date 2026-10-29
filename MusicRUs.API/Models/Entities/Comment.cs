using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MusicRUs.API.Models.Entities;

// Represents a comment made by a user on a forum post.
public class Comment : IHasTimestamps
{
    public int Id { get; set; }

    public int PostId { get; set; } // Foreign key
    public ForumPost Post { get; set; } = null!;

    public int UserId { get; set; } // Foreign key
    public User User { get; set; } = null!;

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}