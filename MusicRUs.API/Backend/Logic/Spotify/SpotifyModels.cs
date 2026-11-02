using System.Collections.Generic;


// Models representing Spotify API entities.

namespace MusicRUs.API.Backend.Logic.Spotify;

public class SpotifyToken
{
    public string AccessToken { get; init; } = "";
    public int ExpiresIn { get; init; }
}

public class SpotifyImage
{
    public string Url { get; init; } = "";
}

public class SpotifyExternalUrls
{
    public string? Spotify { get; init; }
}

public class SpotifyPage<T>
{
    public List<T> Items { get; init; } = [];
    public string? Next { get; init; }
}

public class SpotifySearchResult
{
    public SpotifyPage<SpotifyArtist>? Artists { get; init; }
}

public class SpotifyArtistRef
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
    public SpotifyExternalUrls? ExternalUrls { get; init; }
}

public class SpotifyArtist : SpotifyArtistRef
{
    public List<string> Genres { get; init; } = [];
    public List<SpotifyImage> Images { get; init; } = [];
}

public class SpotifyAlbumRef
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
    public string AlbumType { get; init; } = "album";
    public string? ReleaseDate { get; init; }
    public int? TotalTracks { get; init; }
    public List<SpotifyImage> Images { get; init; } = [];
    public SpotifyExternalUrls? ExternalUrls { get; init; }
    public List<SpotifyArtistRef> Artists { get; init; } = [];
}

public class SpotifyAlbum : SpotifyAlbumRef
{
    public SpotifyPage<SpotifyTrackRef> Tracks { get; init; } = new();
}

public class SpotifyTrackRef
{
    public string? Id { get; init; }
    public string Name { get; init; } = "";
    public int? TrackNumber { get; init; }
    public int? DiscNumber { get; init; }
    public int? DurationMs { get; init; }
    public bool Explicit { get; init; }
    public SpotifyExternalUrls? ExternalUrls { get; init; }
    public List<SpotifyArtistRef> Artists { get; init; } = [];
}