using Microsoft.AspNetCore.Mvc;
using MusicRUs.API.Controllers;
using MusicRUs.API.DTOs;
using MusicRUs.API.Services.Interfaces;
using NSubstitute;

namespace MusicRUs.API.Tests.Controllers;

public class GenreControllerTests
{
    private readonly IGenreService _service = Substitute.For<IGenreService>();
    private readonly GenreController _sut;

    public GenreControllerTests()
    {
        _sut = new GenreController(_service);
    }

    // GET api/genre
    [Fact]
    public async Task GetAllGenresAsync_ReturnsOkWithGenres()
    {
        _service.GetAllGenresAsync().Returns(new List<GenreResponseDto>
        {
            new() { Id = 1, Name = "Rock" },
            new() { Id = 2, Name = "Jazz" }
        });

        var result = await _sut.GetAllGenresAsync();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var genres = Assert.IsAssignableFrom<IEnumerable<GenreResponseDto>>(ok.Value);
        Assert.Equal(2, genres.Count());
    }

    [Fact]
    public async Task GetAllGenresAsync_NoGenres_ReturnsOkWithEmptyList()
    {
        _service.GetAllGenresAsync().Returns(new List<GenreResponseDto>());

        var result = await _sut.GetAllGenresAsync();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var genres = Assert.IsAssignableFrom<IEnumerable<GenreResponseDto>>(ok.Value);
        Assert.Empty(genres);
    }

    // GET api/genre/{id}
    [Fact]
    public async Task GetGenreByIdAsync_ExistingId_ReturnsOkWithGenre()
    {
        _service.GetGenreByIdAsync(1).Returns(new GenreResponseDto { Id = 1, Name = "Rock" });

        var result = await _sut.GetGenreByIdAsync(1);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var genre = Assert.IsType<GenreResponseDto>(ok.Value);
        Assert.Equal("Rock", genre.Name);
    }

    [Fact]
    public async Task GetGenreByIdAsync_UnknownId_ReturnsNotFound()
    {
        _service.GetGenreByIdAsync(999).Returns((GenreResponseDto?)null);

        var result = await _sut.GetGenreByIdAsync(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    // GET api/genre/by-name?name=
    [Fact]
    public async Task GetGenreByNameAsync_ExistingName_ReturnsOkWithGenre()
    {
        _service.GetGenreByNameAsync("Jazz").Returns(new GenreResponseDto { Id = 2, Name = "Jazz" });

        var result = await _sut.GetGenreByNameAsync("Jazz");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var genre = Assert.IsType<GenreResponseDto>(ok.Value);
        Assert.Equal(2, genre.Id);
    }

    [Fact]
    public async Task GetGenreByNameAsync_UnknownName_ReturnsNotFound()
    {
        _service.GetGenreByNameAsync("Polka").Returns((GenreResponseDto?)null);

        var result = await _sut.GetGenreByNameAsync("Polka");

        Assert.IsType<NotFoundResult>(result.Result);
    }
}