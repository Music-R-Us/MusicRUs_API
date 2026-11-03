using MusicRUs.API.DTOs;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;
using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.Backend.Helpers;

namespace MusicRUs.API.Services;

public class ForumProfileService : IForumProfileService
{
    private readonly IForumProfileRepository _fpRepo;
    private readonly IArtistRepository _artistRepo;
    private readonly IAlbumRepository _albumRepo;
    private readonly IGenreRepository _genreRepo;

    public ForumProfileService(
        IForumProfileRepository fpRepository, 
        IArtistRepository artistRepository, 
        IAlbumRepository albumRepository, 
        IGenreRepository genreRepository)
    {
        _fpRepo = fpRepository;
        _artistRepo = artistRepository;
        _albumRepo = albumRepository;
        _genreRepo = genreRepository;
    }

    public async Task<ForumProfileResponseDto?> CreateFpAsync(CreateForumProfileDto dto) // CREATE
    {
        var existingProfile = await _fpRepo.GetFpByUserIdAsync(dto.UserId);
        if (existingProfile != null)
            return null;
        
        var fp = new ForumProfile
        {
            UserId = dto.UserId
        };

        await _fpRepo.AddAsync(fp);
        await _fpRepo.SaveChangesAsync();
        return MapToDto(fp);
    }

    public async Task<ForumProfileResponseDto?> GetFpByUserIdAsync(int userId) // READ
    {
        var fp = await _fpRepo.GetWithFavoritesAsync(userId);
        if (fp == null)
            return null;
        
        return MapToDto(fp);
    }

    // Search profiles by displayname keyword
    public async Task<IEnumerable<ForumProfileResponseDto>> SearchFpByKeywordAsync(string keyword) // READ
    {
        var fp = await _fpRepo.SearchFpByKeywordAsync(keyword);
        return fp.Select(MapToDto);
    }

    public async Task<ForumProfileResponseDto?> UpdateFpAsync(int userId, ForumProfileUpdatedDto dto) // UPDATE
    {
        var fp = await _fpRepo.GetWithFavoritesAsync(userId);
        if(fp == null)
            return null;
        
        fp.DisplayName = dto.DisplayName ?? fp.DisplayName; // Update fields if provided, else keep existing values
        fp.Bio = dto.Bio ?? fp.Bio;
        fp.AvatarUrl = dto.AvatarUrl ?? fp.AvatarUrl;
        fp.Location = dto.Location ?? fp.Location;
        fp.WebsiteUrl = dto.WebsiteUrl ?? fp.WebsiteUrl;
        fp.PrimaryColor = dto.PrimaryColor?.ToUpperInvariant() ?? fp.PrimaryColor;
        fp.AccentColor = dto.AccentColor?.ToUpperInvariant() ?? fp.AccentColor;
        fp.IsPublic = dto.IsPublic ?? fp.IsPublic;
        
        if (dto.FavoriteArtistIds != null)
        {
            await RelationHelper.GetAllOrThrowAsync(_artistRepo, dto.FavoriteArtistIds, "artist");
            RelationHelper.SyncOrdered(fp.FavoriteArtists, dto.FavoriteArtistIds, 
            fa => fa.ArtistId, 
            id => new FavoriteArtist {ArtistId = id},
            (fa, order) => fa.DisplayOrder = order);
        }

        if (dto.FavoriteAlbumIds != null)
        {
            await RelationHelper.GetAllOrThrowAsync(_albumRepo, dto.FavoriteAlbumIds, "album");
            RelationHelper.SyncOrdered(fp.FavoriteAlbums, dto.FavoriteAlbumIds,
            fa => fa.AlbumId,
            id => new FavoriteAlbum { AlbumId = id },
            (fa, order) => fa.DisplayOrder = order);
        }

        if (dto.GenreIds != null)
        {
            var genres = await RelationHelper.GetAllOrThrowAsync(_genreRepo, dto.GenreIds, "genre");
            fp.Genres.Clear();
            fp.Genres.AddRange(genres);
        }

        _fpRepo.Update(fp);
        await _fpRepo.SaveChangesAsync();
        return MapToDto(fp);
    }

//#########################################

    // Helper method to map ForumProfile entity to ForumProfileResponseDto (MapToDto)
    private static ForumProfileResponseDto MapToDto(ForumProfile fp) => new() 
    {
        Id = fp.Id,
        UserId = fp.UserId,
        DisplayName = fp.DisplayName,
        Bio = fp.Bio,
        AvatarUrl = fp.AvatarUrl,
        Location = fp.Location,
        WebsiteUrl = fp.WebsiteUrl,
        PrimaryColor = fp.PrimaryColor,
        AccentColor = fp.AccentColor,
        IsPublic = fp.IsPublic,
        CreatedAt = fp.CreatedAt,
        UpdatedAt = fp.UpdatedAt,
        FavoriteArtists = fp.FavoriteArtists
            .OrderBy(fa => fa.DisplayOrder)
            .Select(fArt => new ArtistIncludeDto {
                Id = fArt.ArtistId, 
                Name = fArt.Artist.Name, 
                ImageUrl = fArt.Artist.ImageUrl, 
                SpotifyId = fArt.Artist.SpotifyId, 
                SpotifyUrl = fArt.Artist.SpotifyUrl
            } 
            ).ToList(),
        FavoriteAlbums = fp.FavoriteAlbums
            .OrderBy(fa => fa.DisplayOrder)
            .Select(fAlb => new AlbumIncludeDto {
                Id = fAlb.AlbumId, 
                Title = fAlb.Album.Title, 
                ImageUrl = fAlb.Album.ImageUrl, 
                SpotifyId = fAlb.Album.SpotifyId, 
                SpotifyUrl = fAlb.Album.SpotifyUrl
                }
            ).ToList(),
        Genres = fp.Genres
            .Select(g => new GenreIncludeDto {
                Id = g.Id, 
                Name = g.Name
                }
            ).ToList()
    };
}