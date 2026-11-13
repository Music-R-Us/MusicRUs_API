using System.ComponentModel.DataAnnotations;

namespace MusicRUs.API.DTOs;

// Represents a Data Transfer Object (DTO) for the Comment entity.
// In other words, a 'payload' for specific operations that is sent to the API.

public class CreateCommentDto // DTO for creating a new comment
{
    //[Required]
    //public int UserId { get; set; }
    //[Required]
    //public int PostId { get; set; }
    [Required]
    [MinLength(1)]
    [MaxLength(500)]
    public string Content { get; set; } = null!;
}

public class CommentResponseDto // DTO for returning comment data from the API
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public UserIncludeDto User { get; set; } = null!;
    public int PostId { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CommentUpdateDto // DTO for updating an existing comment
{
    [Required]
    [MinLength(1)]
    [MaxLength(500)]
    public string Content { get; set; } = null!;
}

public class CommentIncludeDto // DTO for including comments data for a ForumPost
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public UserIncludeDto User { get; set; } = null!;
    public int PostId { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}