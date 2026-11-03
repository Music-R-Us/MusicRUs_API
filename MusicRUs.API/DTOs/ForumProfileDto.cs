using System.ComponentModel.DataAnnotations;

namespace MusicRUs.API.DTOs;

// Represents a Data Transfer Object (DTO) for the User entity.
// In other words, a 'payloads' for specific operations that is sent to the API.

public class CreateForumProfileDto // DTO for creating a new forum profile
{
    [Required]
    public int UserId { get; set;}
}

public class ForumProfileResponseDto // Dto for returning forum profile data in API response
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; } 
    public string? Location { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? PrimaryColor { get; set; }
    public string? AccentColor { get; set; }
    public bool? IsPublic { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<ArtistIncludeDto>? FavoriteArtists { get; set; } 
    public List<AlbumIncludeDto>? FavoriteAlbums { get; set; } 
    public List<GenreIncludeDto>? Genres { get; set; } 
}

public class ForumProfileUpdatedDto // DTO for updating forum profile data
{
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Location { get; set; }
    public string? WebsiteUrl { get; set; }

    [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Color must be in the format #RRGGBB.")]
    public string? PrimaryColor { get; set; }

    [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Color must be in the format #RRGGBB.")]
    public string? AccentColor { get; set; }
    public bool? IsPublic { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<int>? FavoriteArtistIds { get; set; } 
    public List<int>? FavoriteAlbumIds { get; set; } 
    public List<int>? GenreIds { get; set; } 
}