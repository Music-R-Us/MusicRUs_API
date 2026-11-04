namespace MusicRUs.API.DTOs;

public class ConcertResponseDto
{
    public int Id { get; set; }
    public int ArtistId { get; set; }
    public string Name { get; set; } = null!;
    public string VenueName { get; set; } = null!;
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? CountryCode { get; set; }
    public DateTime StartsAt { get; set; } // UTC
    public string? VenueTimeZone { get; set; }
    public string? Url { get; set; }
}