using System.ComponentModel.DataAnnotations;

namespace MusicRUs.API.DTOs;

public class CreateArtistSubscriptionDto
{
    [MaxLength(2)] 
    [RegularExpression("^[A-Z]{2}$", ErrorMessage = "Country code must be two uppercase letters")]
    public string? CountryCode { get; set; }
    [MaxLength(100)]
    public string? Region { get; set; }
    [MaxLength(100)]
    public string? City { get; set; }
}

public class ArtistSubscriptionResponseDto
{
    public int UserId { get; set; }
    public int ArtistId { get; set; }
    public ArtistIncludeDto Artist { get; set; } = null!;
    public string? CountryCode { get; set; }
    public string? Region { get; set; }
    public string? City { get; set; }
    public bool NotifyEmail { get; set; }
    public bool NotifyFeed { get; set; }
}

public class ArtistSubscriptionUpdateDto
{
    [MaxLength(2)] 
    public string? CountryCode { get; set; }
    [MaxLength(100)]
    public string? Region { get; set; }
    [MaxLength(100)]
    public string? City { get; set; }
    public bool? NotifyEmail { get; set; }
    public bool? NotifyFeed { get; set; }
}