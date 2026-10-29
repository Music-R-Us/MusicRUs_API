using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MusicRUs.API.DTOs;

// Represents a Data Transfer Object (DTO) for the ForumPost entity.
// In other words, a 'payload' for specific operations that is sent to the API. 
public class CreateForumPostDto // DTO for creating a new forum post
{
    [Required]
    public int UserId { get; set; }
    [Required]
    public string Title { get; set; } = null!; 
    [Required]
    public string Content { get; set; } = null!;
    public int? TrackId { get; set; }
    public int? AlbumId { get; set; }
    public int? ArtistId { get; set; }
    public List<int>? GenreIds { get; set; }  // Genre IDs associated with the forum post
}

public class ForumPostResponseDto // DTO for returning forum post data from the API
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public int? TrackId { get; set; }
    public int? AlbumId { get; set; }
    public int? ArtistId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<GenreIncludeDto>? Genres { get; set; } 
}

public class ForumPostWithCommentsDto : ForumPostResponseDto // DTO for including related comment data for a forum post
{
    [JsonPropertyOrder(1)]
    public List<CommentIncludeDto> Comments { get; set; } = [];
}

public class ForumPostUpdateDto // DTO for updating an existing forum post
{
    [Required]
    public string Title { get; set; } = null!;
    [Required]
    public string Content { get; set; } = null!;
    public int? TrackId { get; set; }
    public int? AlbumId { get; set; }
    public int? ArtistId { get; set; }
    public List<int>? GenreIds { get; set; }
}