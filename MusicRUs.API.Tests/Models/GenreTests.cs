using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Tests.Models;

public class GenreTests
{
    [Fact]
    public void NewGenre_HasDefaultValues()
    {
        var genre = new Genre();

        Assert.Equal(0, genre.Id);
        Assert.Equal(string.Empty, genre.Name);
    }

    [Fact]
    public void NewGenre_HasInitializedEmptyCollections()
    {
        var genre = new Genre();

        Assert.NotNull(genre.Artists);
        Assert.NotNull(genre.Profiles);
        Assert.NotNull(genre.Posts);
        Assert.Empty(genre.Artists);
        Assert.Empty(genre.Profiles);
        Assert.Empty(genre.Posts);
    }

    [Fact]
    public void Collections_AreNotSharedBetweenInstances()
    {
        var first = new Genre();
        var second = new Genre();

        Assert.NotSame(first.Artists, second.Artists);
        Assert.NotSame(first.Profiles, second.Profiles);
        Assert.NotSame(first.Posts, second.Posts);
    }

    [Fact]
    public void Properties_CanBeSet()
    {
        var genre = new Genre { Id = 7, Name = "Jazz" };

        Assert.Equal(7, genre.Id);
        Assert.Equal("Jazz", genre.Name);
    }
}