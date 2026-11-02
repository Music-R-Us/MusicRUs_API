namespace MusicRUs.API.Backend.Logic.Spotify;

public class SpotifyOptions
{
    public const string SectionName = "Spotify";
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
}