using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace MusicRUs.API.Backend.Logic.Spotify;

public class SpotifyClient : ISpotifyClient
{
    private const string TokenCacheKey = "spotify:access-token";
    private const string Market = "DK";
    private const int MaxAlbumPages = 5;

    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;
    private readonly SpotifyOptions _options;

    public SpotifyClient(HttpClient http, IMemoryCache cache, IOptions<SpotifyOptions> options)
    {
        _http = http;
        _cache = cache;
        _options = options.Value;
    }

    public async Task<IReadOnlyList<SpotifyArtist>> SearchArtistsAsync(string query)
    {
        var result = await GetAsync<SpotifySearchResult>(
            $"v1/search?type=artist&limit=10&q={Uri.EscapeDataString(query)}");
        return result?.Artists?.Items ?? [];
    }

    public async Task<IReadOnlyList<SpotifyAlbum>> SearchAlbumsAsync(string query)
    {
        var result = await GetAsync<SpotifySearchResult>(
            $"v1/search?type=album&market={Market}&limit=10&q={Uri.EscapeDataString(query)}");
        return result?.Albums?.Items ?? [];
    }

    public Task<SpotifyArtist?> GetArtistAsync(string spotifyId)
        => GetAsync<SpotifyArtist>($"v1/artists/{spotifyId}");

    public async Task<IReadOnlyList<SpotifyAlbumRef>> GetArtistAlbumsAsync(string spotifyId)
    {
        var albums = new List<SpotifyAlbumRef>();
        string? url = $"v1/artists/{spotifyId}/albums?include_groups=album,single,compilation&market={Market}&limit=20";

        for (var page = 0; url is not null && page < MaxAlbumPages; page++)
        {
            var result = await GetAsync<SpotifyPage<SpotifyAlbumRef>>(url);
            if (result is null)
                break;
            albums.AddRange(result.Items);
            url = result.Next;
        }
        return albums;
    }

    public async Task<SpotifyAlbum?> GetAlbumAsync(string spotifyId)
    {
        var album = await GetAsync<SpotifyAlbum>($"v1/albums/{spotifyId}?market={Market}");
        var url = album?.Tracks.Next;

        while (album is not null && url is not null)
        {
            var page = await GetAsync<SpotifyPage<SpotifyTrackRef>>(url);
            if (page is null)
                break;
            album.Tracks.Items.AddRange(page.Items);
            url = page.Next;
        }
        return album;
    }

    private async Task<T?> GetAsync<T>(string url) where T : class
    {
        for (var attempt = 1; ; attempt++)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessTokenAsync());
            using var response = await _http.SendAsync(request);

            if (response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.BadRequest)
                return null;

            if (attempt == 1 && response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _cache.Remove(TokenCacheKey);
                continue;
            }

            if (attempt == 1 && response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                var wait = response.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(1);
                await Task.Delay(wait < TimeSpan.FromSeconds(5) ? wait : TimeSpan.FromSeconds(5));
                continue;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(Json);
        }
    }

    private async Task<string> GetAccessTokenAsync()
    {
        if (_cache.TryGetValue(TokenCacheKey, out string? cached) && cached is not null)
            return cached;

        var credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{_options.ClientId}:{_options.ClientSecret}"));

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials"
            })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        using var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var token = await response.Content.ReadFromJsonAsync<SpotifyToken>(Json)
            ?? throw new HttpRequestException("Spotify returned no token.");

        // Renew one minute before it expires
        _cache.Set(TokenCacheKey, token.AccessToken, TimeSpan.FromSeconds(Math.Max(60, token.ExpiresIn - 60)));
        return token.AccessToken;
    }
}