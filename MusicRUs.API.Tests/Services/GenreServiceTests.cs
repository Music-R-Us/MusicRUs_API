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
    public async Task GetGenreByIdAsync_UnknownId_ThrowsKeyNotFoundException()
    {
        _repo.GetByIdAsync(999).Returns((Genre?)null);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetGenreByIdAsync(999));

        Assert.Contains("999", ex.Message);
    }

    [Fact]
    public async Task GetGenreByIdAsync_CallsRepositoryOnceWithId()
    {
        _repo.GetByIdAsync(3).Returns(new Genre { Id = 3, Name = "Hip Hop" });

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
    public async Task GetGenreByNameAsync_UnknownName_ThrowsKeyNotFoundException()
    {
        _repo.GetByStringAsync("Polka").Returns((Genre?)null);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetGenreByNameAsync("Polka"));

        Assert.Contains("Polka", ex.Message);
    }

    [Fact]
    public async Task GetGenreByNameAsync_CallsRepositoryOnceWithName()
    {
        _repo.GetByStringAsync("Rock").Returns(new Genre { Id = 1, Name = "Rock" });

        await _sut.GetGenreByNameAsync("Rock");

        await _repo.Received(1).GetByStringAsync("Rock");
    }
}