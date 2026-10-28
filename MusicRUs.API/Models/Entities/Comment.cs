using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MusicRUs.API.Models.Entities;

// Represents a comment made by a user on a forum post.
public class Comment : IHasTimestamps
{
    public int Id { get; set; }

    public int PostId { get; set; } // Foreign key to the ForumPost entity
    public ForumPost Post { get; set; } = null!; // Navigation property to the ForumPost entity

    public int UserId { get; set; } // Foreign key to the User entity
    public User User { get; set; } = null!; // Navigation property to the User entity

    public string Content { get; set; } = string.Empty; // Content of the comment

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}