using Microsoft.AspNetCore.Mvc;
using MusicRUs.API.Controllers;
using MusicRUs.API.DTOs;
using MusicRUs.API.Services.Interfaces;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MusicRUs.API.Tests.Controllers;

public class AlbumControllerTests
{
    private readonly IAlbumService _service = Substitute.For<IAlbumService>();
    private readonly AlbumController _sut;

    public AlbumControllerTests()
    {
        _sut = new AlbumController(_service);
    }

    private static AlbumDetailsResponseDto CreateDetailsDto(int id = 1, string title = "Test Album") => new()
    {
        Id = id,
        SpotifyId = $"album-spotify-{id}",
        Title = title
    };

    private static AlbumResponseDto CreateDto(int id) => new()
    {
        Id = id,
        SpotifyId = $"album-spotify-{id}",
        Title = $"Album {id}"
    };

    // GET api/album/{id}
    //##############################################################################
    [Fact]
    public async Task GetAlbumByIdAsync_ExistingId_ReturnsOkWithAlbum()
    {
        _service.GetAlbumByIdAsync(1).Returns(CreateDetailsDto());

        var result = await _sut.GetAlbumByIdAsync(1);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var album = Assert.IsType<AlbumDetailsResponseDto>(ok.Value);
        Assert.Equal(1, album.Id);
    }

    [Fact]
    public async Task GetAlbumByIdAsync_ServiceThrowsKeyNotFound_ReturnsNotFoundWithMessage()
    {
        _service.GetAlbumByIdAsync(999)
            .ThrowsAsync(new KeyNotFoundException("Album with ID 999 not found."));

        var result = await _sut.GetAlbumByIdAsync(999);

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("Album with ID 999 not found.", notFound.Value);
    }

    // GET api/album/spotify/{spotifyId}
    //##############################################################################
    [Fact]
    public async Task GetAlbumBySpotifyIdAsync_Existing_ReturnsOkWithAlbum()
    {
        _service.GetAlbumBySpotifyIdAsync("album-spotify-1").Returns(CreateDetailsDto());

        var result = await _sut.GetAlbumBySpotifyIdAsync("album-spotify-1");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsType<AlbumDetailsResponseDto>(ok.Value);
    }

    [Fact]
    public async Task GetAlbumBySpotifyIdAsync_ServiceThrowsKeyNotFound_ReturnsNotFoundWithMessage()
    {
        _service.GetAlbumBySpotifyIdAsync("unknown")
            .ThrowsAsync(new KeyNotFoundException("Album with Spotify ID unknown not found."));

        var result = await _sut.GetAlbumBySpotifyIdAsync("unknown");

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("Album with Spotify ID unknown not found.", notFound.Value);
    }

    // GET api/album/by-title?title=
    //##############################################################################
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetAlbumByTitleAsync_EmptyTitle_ReturnsBadRequestAndSkipsService(string title)
    {
        var result = await _sut.GetAlbumByTitleAsync(title);

        Assert.IsType<BadRequestObjectResult>(result.Result);
        await _service.DidNotReceive().GetAlbumByTitleAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task GetAlbumByTitleAsync_Existing_ReturnsOkWithAlbum()
    {
        _service.GetAlbumByTitleAsync("Test Album").Returns(CreateDetailsDto());

        var result = await _sut.GetAlbumByTitleAsync("Test Album");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var album = Assert.IsType<AlbumDetailsResponseDto>(ok.Value);
        Assert.Equal("Test Album", album.Title);
    }

    [Fact]
    public async Task GetAlbumByTitleAsync_ServiceThrowsKeyNotFound_ReturnsNotFoundWithMessage()
    {
        _service.GetAlbumByTitleAsync("Unknown")
            .ThrowsAsync(new KeyNotFoundException("Album with title 'Unknown' not found."));

        var result = await _sut.GetAlbumByTitleAsync("Unknown");

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("Album with title 'Unknown' not found.", notFound.Value);
    }

    // GET api/album/artist/{artistId}
    //##############################################################################
    [Fact]
    public async Task GetAlbumsByArtistIdAsync_HasAlbums_ReturnsOkWithAlbums()
    {
        _service.GetAlbumsByArtistIdAsync(10).Returns(new List<AlbumResponseDto> { CreateDto(1), CreateDto(2) });

        var result = await _sut.GetAlbumsByArtistIdAsync(10);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var albums = Assert.IsAssignableFrom<IEnumerable<AlbumResponseDto>>(ok.Value);
        Assert.Equal(2, albums.Count());
    }

    [Fact]
    public async Task GetAlbumsByArtistIdAsync_NoAlbums_ReturnsOkWithEmptyList()
    {
        _service.GetAlbumsByArtistIdAsync(10).Returns(new List<AlbumResponseDto>());

        var result = await _sut.GetAlbumsByArtistIdAsync(10);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var albums = Assert.IsAssignableFrom<IEnumerable<AlbumResponseDto>>(ok.Value);
        Assert.Empty(albums);
    }

    // GET api/album/search?keyword=
    //##############################################################################
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SearchAlbumsByKeywordAsync_EmptyKeyword_ReturnsBadRequestAndSkipsService(string keyword)
    {
        var result = await _sut.SearchAlbumsByKeywordAsync(keyword);

        Assert.IsType<BadRequestObjectResult>(result.Result);
        await _service.DidNotReceive().SearchAlbumsByKeywordAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task SearchAlbumsByKeywordAsync_ValidKeyword_ReturnsOkWithResults()
    {
        _service.SearchAlbumsByKeywordAsync("rock").Returns(new List<AlbumResponseDto> { CreateDto(1) });

        var result = await _sut.SearchAlbumsByKeywordAsync("rock");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var albums = Assert.IsAssignableFrom<IEnumerable<AlbumResponseDto>>(ok.Value);
        Assert.Single(albums);
    }
}