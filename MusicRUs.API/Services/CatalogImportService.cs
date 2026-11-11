using Microsoft.Extensions.Caching.Memory;
using MusicRUs.API.Backend.Logic.Spotify;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;
using MusicRUs.API.Services.Interfaces;

namespace MusicRUs.API.Services;

// Fetches from Spotify and saves to the database (upsert on spotify_id)
public class CatalogImportService : ICatalogImportService
{
    private static readonly TimeSpan AlbumRefreshInterval = TimeSpan.FromHours(24);

    private readonly ISpotifyClient _spotify;
    private readonly ICatalogRepository _catalog;
    private readonly IMemoryCache _cache;

    // Rows handled in this request, so nothing is created twice before SaveChanges
    private readonly Dictionary<string, Artist> _artists = new();
    private readonly Dictionary<string, Album> _albums = new();
    private readonly Dictionary<string, Genre> _genres = new(StringComparer.OrdinalIgnoreCase);

    public CatalogImportService(ISpotifyClient spotify, ICatalogRepository catalog, IMemoryCache cache)
    {
        _spotify = spotify;
        _catalog = catalog;
        _cache = cache;
    }

    public async Task<IReadOnlyList<Artist>> SearchArtistsAsync(string query)
    {
        var found = await _spotify.SearchArtistsAsync(query);
        var artists = new List<Artist>();
        foreach (var spotifyArtist in found)
            artists.Add(await UpsertArtistAsync(spotifyArtist));

        await _catalog.SaveChangesAsync();
        return artists;
    }

    public async Task<IReadOnlyList<Album>> SearchAlbumsAsync(string query)
    {
        var found = await _spotify.SearchAlbumsAsync(query);
        var albums = new List<Album>();
        foreach (var spotifyAlbum in found)
            albums.Add(await UpsertAlbumAsync(spotifyAlbum));

        await _catalog.SaveChangesAsync();
        return albums;
    }

    public async Task ImportArtistAlbumsAsync(int artistId)
    {
        var cacheKey = $"spotify:artist-albums:{artistId}";
        if (_cache.TryGetValue(cacheKey, out _))
            return;

        var artist = await _catalog.GetArtistByIdAsync(artistId);
        if (artist is null)
            return;

        var spotifyArtist = await _spotify.GetArtistAsync(artist.SpotifyId);
        if (spotifyArtist is not null)
            await UpsertArtistAsync(spotifyArtist);

        var spotifyAlbums = await _spotify.GetArtistAlbumsAsync(artist.SpotifyId);
        foreach (var existing in await _catalog.GetAlbumsBySpotifyIdsAsync(spotifyAlbums.Select(a => a.Id)))
            _albums[existing.SpotifyId] = existing;

        // Albums not found above are new, so create them without asking the database again
        foreach (var spotifyAlbum in spotifyAlbums.Where(a => !_albums.ContainsKey(a.Id)))
        {
            var album = new Album { SpotifyId = spotifyAlbum.Id };
            _catalog.Add(album);
            _albums[spotifyAlbum.Id] = album;
        }

        foreach (var spotifyAlbum in spotifyAlbums)
            await UpsertAlbumAsync(spotifyAlbum);

        await _catalog.SaveChangesAsync();
        _cache.Set(cacheKey, true, AlbumRefreshInterval);
    }

    public async Task ImportAlbumTracksAsync(int albumId)
    {
        var album = await _catalog.GetAlbumByIdAsync(albumId);
        if (album is null || album.Tracks.Count > 0)
            return;

        var spotifyAlbum = await _spotify.GetAlbumAsync(album.SpotifyId);
        if (spotifyAlbum is null)
            return;

        _albums[album.SpotifyId] = album;
        await UpsertAlbumAsync(spotifyAlbum);
        await AddTracksAsync(album, spotifyAlbum);
        await _catalog.SaveChangesAsync();
    }

    public async Task<int?> ImportAlbumAsync(string spotifyId)
    {
        var spotifyAlbum = await _spotify.GetAlbumAsync(spotifyId);
        if (spotifyAlbum is null)
            return null;

        var album = await UpsertAlbumAsync(spotifyAlbum);
        if (album.Tracks.Count == 0)
            await AddTracksAsync(album, spotifyAlbum);

        await _catalog.SaveChangesAsync();
        return album.Id;
    }

    private async Task<Artist> UpsertArtistAsync(SpotifyArtist source)
    {
        var artist = await FindOrCreateArtistAsync(source);
        artist.ImageUrl = source.Images.FirstOrDefault()?.Url ?? artist.ImageUrl;

        foreach (var name in source.Genres)
        {
            if (!artist.Genres.Any(g => string.Equals(g.Name, name, StringComparison.OrdinalIgnoreCase)))
                artist.Genres.Add(await FindOrCreateGenreAsync(name));
        }
        return artist;
    }

    private async Task<Artist> FindOrCreateArtistAsync(SpotifyArtistRef source)
    {
        if (!_artists.TryGetValue(source.Id, out var artist))
        {
            artist = await _catalog.GetArtistBySpotifyIdAsync(source.Id);
            if (artist is null)
            {
                artist = new Artist { SpotifyId = source.Id };
                _catalog.Add(artist);
            }
            _artists[source.Id] = artist;
        }

        artist.Name = Cut(source.Name, 255);
        artist.SpotifyUrl = source.ExternalUrls?.Spotify ?? artist.SpotifyUrl;
        return artist;
    }

    private async Task<Genre> FindOrCreateGenreAsync(string name)
    {
        if (!_genres.TryGetValue(name, out var genre))
        {
            genre = await _catalog.GetGenreByNameAsync(name);
            if (genre is null)
            {
                genre = new Genre { Name = name };
                _catalog.Add(genre);
            }
            _genres[name] = genre;
        }
        return genre;
    }

    private async Task<Album> UpsertAlbumAsync(SpotifyAlbumRef source)
    {
        if (!_albums.TryGetValue(source.Id, out var album))
        {
            album = await _catalog.GetAlbumBySpotifyIdAsync(source.Id);
            if (album is null)
            {
                album = new Album { SpotifyId = source.Id };
                _catalog.Add(album);
            }
            _albums[source.Id] = album;
        }

        album.Title = Cut(source.Name, 255);
        album.AlbumType = Enum.TryParse<AlbumType>(source.AlbumType, ignoreCase: true, out var type)
            ? type
            : AlbumType.Album;
        album.ReleaseDate = source.ReleaseDate;
        album.TotalTracks = source.TotalTracks;
        album.ImageUrl = source.Images.FirstOrDefault()?.Url ?? album.ImageUrl;
        album.SpotifyUrl = source.ExternalUrls?.Spotify ?? album.SpotifyUrl;

        foreach (var artistRef in source.Artists)
        {
            var artist = await FindOrCreateArtistAsync(artistRef);
            if (!album.Artists.Contains(artist))
                album.Artists.Add(artist);
        }
        return album;
    }

    private async Task AddTracksAsync(Album album, SpotifyAlbum source)
    {
        foreach (var item in source.Tracks.Items)
        {
            if (item.Id is null)
                continue;

            var track = new Track
            {
                SpotifyId = item.Id,
                Title = item.Name,
                TrackNumber = item.TrackNumber,
                DiscNumber = item.DiscNumber ?? 1,
                DurationMs = item.DurationMs,
                Explicit = item.Explicit,
                SpotifyUrl = item.ExternalUrls?.Spotify
            };

            foreach (var artistRef in item.Artists)
            {
                var artist = await FindOrCreateArtistAsync(artistRef);
                if (!track.Artists.Contains(artist))
                    track.Artists.Add(artist);
            }

            album.Tracks.Add(track);
        }
    }

    private static string Cut(string value, int max) => value.Length <= max ? value : value[..max];
}