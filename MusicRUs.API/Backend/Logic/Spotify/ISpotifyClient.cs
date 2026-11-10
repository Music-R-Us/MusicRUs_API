namespace MusicRUs.API.Backend.Logic.Spotify;

// Interface for Spotify client to interact with Spotify API.
public interface ISpotifyClient
{
    Task<IReadOnlyList<SpotifyArtist>> SearchArtistsAsync(string query);
    Task<IReadOnlyList<SpotifyAlbum>> SearchAlbumsAsync(string query);
    Task<SpotifyArtist?> GetArtistAsync(string spotifyId);
    Task<IReadOnlyList<SpotifyAlbumRef>> GetArtistAlbumsAsync(string spotifyId);
    Task<SpotifyAlbum?> GetAlbumAsync(string spotifyId);
}