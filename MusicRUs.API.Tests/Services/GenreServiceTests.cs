using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;
using MusicRUs.API.Services;
using NSubstitute;

namespace MusicRUs.API.Tests.Services;

public class GenreServiceTests
{
    private readonly IGenreRepository _repo = Substitute.For<IGenreRepository>();
    private readonly GenreService _sut;

    public GenreServiceTests()
    {
        _sut = new GenreService(_repo);
    }

    // GetAllGenresAsync
    [Fact]
    public async Task GetAllGenresAsync_ReturnsAllGenresMappedToDto()
    {
        _repo.GetAllAsync().Returns(new List<Genre>
        {
            new() { Id = 1, Name = "Rock" },
            new() { Id = 2, Name = "Jazz" },
            new() { Id = 3, Name = "Hip Hop" }
        });

        var result = (await _sut.GetAllGenresAsync()).ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal(new[] { 1, 2, 3 }, result.Select(g => g.Id));
        Assert.Equal(new[] { "Rock", "Jazz", "Hip Hop" }, result.Select(g => g.Name));
    }

    [Fact]
    public async Task GetAllGenresAsync_NoGenres_ReturnsEmpty()
    {
        _repo.GetAllAsync().Returns(new List<Genre>());

        var result = await _sut.GetAllGenresAsync();

        Assert.Empty(result);
    }

    // GetGenreByIdAsync
    [Fact]
    public async Task GetGenreByIdAsync_ExistingId_ReturnsDto()
    {
        _repo.GetByIdAsync(2).Returns(new Genre { Id = 2, Name = "Jazz" });

        var result = await _sut.GetGenreByIdAsync(2);

        Assert.NotNull(result);
        Assert.Equal(2, result.Id);
        Assert.Equal("Jazz", result.Name);
    }

    [Fact]
    public async Task GetGenreByIdAsync_UnknownId_ReturnsNull()
    {
        _repo.GetByIdAsync(999).Returns((Genre?)null);

        var result = await _sut.GetGenreByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetGenreByIdAsync_CallsRepositoryOnceWithId()
    {
        await _sut.GetGenreByIdAsync(3);

        await _repo.Received(1).GetByIdAsync(3);
    }

    // GetGenreByNameAsync
    [Fact]
    public async Task GetGenreByNameAsync_ExistingName_ReturnsDto()
    {
        _repo.GetByStringAsync("Hip Hop").Returns(new Genre { Id = 3, Name = "Hip Hop" });

        var result = await _sut.GetGenreByNameAsync("Hip Hop");

        Assert.NotNull(result);
        Assert.Equal(3, result.Id);
        Assert.Equal("Hip Hop", result.Name);
    }

    [Fact]
    public async Task GetGenreByNameAsync_UnknownName_ReturnsNull()
    {
        _repo.GetByStringAsync("Polka").Returns((Genre?)null);

        var result = await _sut.GetGenreByNameAsync("Polka");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetGenreByNameAsync_CallsRepositoryOnceWithName()
    {
        await _sut.GetGenreByNameAsync("Rock");

        await _repo.Received(1).GetByStringAsync("Rock");
    }
}