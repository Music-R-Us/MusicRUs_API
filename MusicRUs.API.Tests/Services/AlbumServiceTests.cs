using Microsoft.Extensions.Logging.Abstractions;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;
using MusicRUs.API.Services;
using MusicRUs.API.Services.Interfaces;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MusicRUs.API.Tests.Services;

public class AlbumServiceTests
{
    private readonly IAlbumRepository _repo = Substitute.For<IAlbumRepository>();
    private readonly ICatalogImportService _catalog = Substitute.For<ICatalogImportService>();
    private readonly AlbumService _sut;

    public AlbumServiceTests()
    {
        _sut = new AlbumService(_repo, _catalog, NullLogger<AlbumService>.Instance);
    }

    // Test data helpers
    private static Album CreateAlbum(int id = 1, string spotifyId = "album-spotify-1", string title = "Test Album")
    {
        var album = new Album
        {
            Id = id,
            SpotifyId = spotifyId,
            Title = title,
            AlbumType = AlbumType.Album,
            ReleaseDate = "2019-05-17",
            TotalTracks = 12,
            ImageUrl = "https://img/album",
            SpotifyUrl = "https://open.spotify.com/album/1"
        };
        album.Artists.Add(new Artist
        {
            Id = 10,
            SpotifyId = "artist-spotify-10",
            Name = "Test Artist",
            ImageUrl = "https://img/artist",
            SpotifyUrl = "https://open.spotify.com/artist/10"
        });
        return album;
    }

    private static Track CreateTrack(int id, int discNumber, int trackNumber) => new()
    {
        Id = id,
        SpotifyId = $"track-spotify-{id}",
        Title = $"Track {id}",
        DiscNumber = discNumber,
        TrackNumber = trackNumber
    };

    // The two exception types the service treats as "Spotify is unavailable"
    private static Exception CreateSpotifyError(Type exceptionType)
        => (Exception)Activator.CreateInstance(exceptionType)!;

    // GetAlbumByIdAsync
    //##############################################################################
    [Fact]
    public async Task GetAlbumByIdAsync_ExistingId_ReturnsDetailsDto()
    {
        _repo.GetByIdAsync(1).Returns(CreateAlbum());

        var result = await _sut.GetAlbumByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Album", result.Title);
    }

    [Fact]
    public async Task GetAlbumByIdAsync_UnknownId_ThrowsKeyNotFoundException()
    {
        _repo.GetByIdAsync(999).Returns((Album?)null);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetAlbumByIdAsync(999));

        Assert.Contains("999", ex.Message);
    }

    [Fact]
    public async Task GetAlbumByIdAsync_ImportsTracksBeforeReadingFromDatabase()
    {
        _repo.GetByIdAsync(1).Returns(CreateAlbum());

        await _sut.GetAlbumByIdAsync(1);

        Received.InOrder(() =>
        {
            _catalog.ImportAlbumTracksAsync(1);
            _repo.GetByIdAsync(1);
        });
    }

    [Theory]
    [InlineData(typeof(HttpRequestException))]
    [InlineData(typeof(TaskCanceledException))]
    public async Task GetAlbumByIdAsync_SpotifyFails_StillReturnsAlbumFromDatabase(Type exceptionType)
    {
        _catalog.ImportAlbumTracksAsync(1).ThrowsAsync(CreateSpotifyError(exceptionType));
        _repo.GetByIdAsync(1).Returns(CreateAlbum());

        var result = await _sut.GetAlbumByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetAlbumByIdAsync_UnexpectedImportError_IsNotSwallowed()
    {
        _catalog.ImportAlbumTracksAsync(1).ThrowsAsync(new InvalidOperationException("Boom"));

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.GetAlbumByIdAsync(1));
    }

    [Fact]
    public async Task GetAlbumByIdAsync_TracksAreSortedByDiscThenTrackNumber()
    {
        var album = CreateAlbum();
        album.Tracks.Add(CreateTrack(id: 1, discNumber: 2, trackNumber: 1));
        album.Tracks.Add(CreateTrack(id: 2, discNumber: 1, trackNumber: 2));
        album.Tracks.Add(CreateTrack(id: 3, discNumber: 1, trackNumber: 1));
        _repo.GetByIdAsync(1).Returns(album);

        var result = await _sut.GetAlbumByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(new[] { 3, 2, 1 }, result.Tracks.Select(t => t.Id));
    }

    // GetAlbumBySpotifyIdAsync
    //##############################################################################
    [Fact]
    public async Task GetAlbumBySpotifyIdAsync_AlbumWithTracksInDatabase_DoesNotImport()
    {
        var album = CreateAlbum();
        album.Tracks.Add(CreateTrack(1, 1, 1));
        _repo.GetAlbumWithDetailsBySpotifyIdAsync("album-spotify-1").Returns(album);

        var result = await _sut.GetAlbumBySpotifyIdAsync("album-spotify-1");

        Assert.NotNull(result);
        await _catalog.DidNotReceive().ImportAlbumAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task GetAlbumBySpotifyIdAsync_AlbumMissing_ImportsAndReturnsImportedAlbum()
    {
        var imported = CreateAlbum();
        imported.Tracks.Add(CreateTrack(1, 1, 1));
        // First call: not in database. Second call (after import): found.
        _repo.GetAlbumWithDetailsBySpotifyIdAsync("album-spotify-1").Returns((Album?)null, imported);

        var result = await _sut.GetAlbumBySpotifyIdAsync("album-spotify-1");

        Assert.NotNull(result);
        Assert.Single(result.Tracks);
        await _catalog.Received(1).ImportAlbumAsync("album-spotify-1");
    }

    [Fact]
    public async Task GetAlbumBySpotifyIdAsync_AlbumWithoutTracks_TriggersImport()
    {
        var withoutTracks = CreateAlbum();
        var withTracks = CreateAlbum();
        withTracks.Tracks.Add(CreateTrack(1, 1, 1));
        _repo.GetAlbumWithDetailsBySpotifyIdAsync("album-spotify-1").Returns(withoutTracks, withTracks);

        var result = await _sut.GetAlbumBySpotifyIdAsync("album-spotify-1");

        Assert.NotNull(result);
        Assert.Single(result.Tracks);
        await _catalog.Received(1).ImportAlbumAsync("album-spotify-1");
    }

    [Fact]
    public async Task GetAlbumBySpotifyIdAsync_StillMissingAfterImport_ThrowsKeyNotFoundException()
    {
        _repo.GetAlbumWithDetailsBySpotifyIdAsync("unknown").Returns((Album?)null);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetAlbumBySpotifyIdAsync("unknown"));

        Assert.Contains("unknown", ex.Message);
        await _catalog.Received(1).ImportAlbumAsync("unknown"); // It did try Spotify first
    }

    [Theory]
    [InlineData(typeof(HttpRequestException))]
    [InlineData(typeof(TaskCanceledException))]
    public async Task GetAlbumBySpotifyIdAsync_ImportFails_ReturnsAlbumWithoutTracks(Type exceptionType)
    {
        _repo.GetAlbumWithDetailsBySpotifyIdAsync("album-spotify-1").Returns(CreateAlbum());
        _catalog.ImportAlbumAsync("album-spotify-1").ThrowsAsync(CreateSpotifyError(exceptionType));

        var result = await _sut.GetAlbumBySpotifyIdAsync("album-spotify-1");

        Assert.NotNull(result);
        Assert.Empty(result.Tracks);
    }

    // GetAlbumByTitleAsync
    //##############################################################################
    [Fact]
    public async Task GetAlbumByTitleAsync_ExistingTitle_MapsAllFields()
    {
        _repo.GetAlbumWithDetailsByTitleAsync("Test Album").Returns(CreateAlbum());

        var result = await _sut.GetAlbumByTitleAsync("Test Album");

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("album-spotify-1", result.SpotifyId);
        Assert.Equal("Test Album", result.Title);
        Assert.Equal(AlbumType.Album, result.AlbumType);
        Assert.Equal("2019-05-17", result.ReleaseDate);
        Assert.Equal(12, result.TotalTracks);
        Assert.Equal("https://img/album", result.ImageUrl);
        Assert.Equal("https://open.spotify.com/album/1", result.SpotifyUrl);

        var artist = Assert.Single(result.Artists);
        Assert.Equal(10, artist.Id);
        Assert.Equal("artist-spotify-10", artist.SpotifyId);
        Assert.Equal("Test Artist", artist.Name);
    }

    [Fact]
    public async Task GetAlbumByTitleAsync_UnknownTitle_ThrowsKeyNotFoundException()
    {
        _repo.GetAlbumWithDetailsByTitleAsync("Unknown").Returns((Album?)null);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetAlbumByTitleAsync("Unknown"));

        Assert.Contains("Unknown", ex.Message);
    }

    // GetAlbumsByArtistIdAsync
    //##############################################################################
    [Fact]
    public async Task GetAlbumsByArtistIdAsync_ReturnsMappedAlbums()
    {
        _repo.GetAllAlbumsByArtistIdAsync(10).Returns(new List<Album>
        {
            CreateAlbum(1, "album-spotify-1", "First"),
            CreateAlbum(2, "album-spotify-2", "Second")
        });

        var result = (await _sut.GetAlbumsByArtistIdAsync(10)).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(new[] { "First", "Second" }, result.Select(a => a.Title));
    }

    [Fact]
    public async Task GetAlbumsByArtistIdAsync_NoAlbums_ReturnsEmpty()
    {
        _repo.GetAllAlbumsByArtistIdAsync(10).Returns(new List<Album>());

        var result = await _sut.GetAlbumsByArtistIdAsync(10);

        Assert.Empty(result);
    }

    // SearchAlbumsByKeywordAsync
    //##############################################################################
    [Fact]
    public async Task SearchAlbumsByKeywordAsync_SpotifyWorks_ReturnsSpotifyResultsAndSkipsDatabase()
    {
        _catalog.SearchAlbumsAsync("rock").Returns(new List<Album> { CreateAlbum(title: "From Spotify") });

        var result = (await _sut.SearchAlbumsByKeywordAsync("rock")).ToList();

        Assert.Equal("From Spotify", Assert.Single(result).Title);
        await _repo.DidNotReceive().SearchAlbumsByKeywordAsync(Arg.Any<string>());
    }

    [Theory]
    [InlineData(typeof(HttpRequestException))]
    [InlineData(typeof(TaskCanceledException))]
    public async Task SearchAlbumsByKeywordAsync_SpotifyFails_FallsBackToDatabase(Type exceptionType)
    {
        _catalog.SearchAlbumsAsync("rock").ThrowsAsync(CreateSpotifyError(exceptionType));
        _repo.SearchAlbumsByKeywordAsync("rock").Returns(new List<Album> { CreateAlbum(title: "From Database") });

        var result = (await _sut.SearchAlbumsByKeywordAsync("rock")).ToList();

        Assert.Equal("From Database", Assert.Single(result).Title);
    }

    [Fact]
    public async Task SearchAlbumsByKeywordAsync_UnexpectedError_IsNotSwallowed()
    {
        _catalog.SearchAlbumsAsync("rock").ThrowsAsync(new InvalidOperationException("Boom"));

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.SearchAlbumsByKeywordAsync("rock"));
        await _repo.DidNotReceive().SearchAlbumsByKeywordAsync(Arg.Any<string>());
    }
}