using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Tests.Models;

public class AlbumTests
{
    [Fact]
    public void NewAlbum_HasInitializedEmptyCollections()
    {
        var album = new Album();

        Assert.NotNull(album.Tracks);
        Assert.NotNull(album.Artists);
        Assert.Empty(album.Tracks);
        Assert.Empty(album.Artists);
    }

    [Fact]
    public void NewAlbum_OptionalFieldsAreNull()
    {
        var album = new Album();

        Assert.Null(album.ReleaseDate);
        Assert.Null(album.TotalTracks);
        Assert.Null(album.ImageUrl);
        Assert.Null(album.SpotifyUrl);
    }

    [Fact]
    public void NewAlbum_DefaultAlbumTypeIsAlbum()
    {
        var album = new Album();

        Assert.Equal(AlbumType.Album, album.AlbumType);
    }

    [Fact]
    public void Collections_AreNotSharedBetweenInstances()
    {
        var first = new Album();
        var second = new Album();

        Assert.NotSame(first.Tracks, second.Tracks);
        Assert.NotSame(first.Artists, second.Artists);
    }

    [Fact]
    public void TracksAndArtists_CanBeAddedTo()
    {
        var album = new Album();

        album.Tracks.Add(new Track { Id = 1, Title = "Intro" });
        album.Artists.Add(new Artist { Id = 1, Name = "Test Artist" });

        Assert.Single(album.Tracks);
        Assert.Single(album.Artists);
    }

    [Fact]
    public void Album_ImplementsIHasTimestamps()
    {
        Assert.IsAssignableFrom<IHasTimestamps>(new Album());
    }
}