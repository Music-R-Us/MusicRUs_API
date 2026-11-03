using MusicRUs.API.DTOs;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.Repositories.Interfaces;
using MusicRUs.API.Backend.Helpers;

namespace MusicRUs.API.Services;

public class ForumPostService : IForumPostService
{
    private readonly IForumPostRepository _fpRepo;
    private readonly IGenreRepository _genreRepo;

    public ForumPostService(IForumPostRepository fpRepo, IGenreRepository genreRepo) 
    {
        _fpRepo = fpRepo;  // Dependency Inversion for ForumPost repository (Dependency Inversion Principle)
        _genreRepo = genreRepo;  // Dependency Inversion for Genre repository (Dependency Inversion Principle)
    }

    // Implement the methods defined in IForumPost interface

    public async Task<ForumPostResponseDto> CreateForumPostAsync(CreateForumPostDto dto) // CREATE
    {
        // Create new ForumPost entity
        var fp = new ForumPost // Map properties from dto to entity
        {
            UserId = dto.UserId,
            Title = dto.Title,
            Content = dto.Content,
            TrackId = dto.TrackId,
            AlbumId = dto.AlbumId,
            ArtistId = dto.ArtistId
        };

        // Ensure that the post links to only one track, album, or artist
        var linkCount = new[] { fp.TrackId, fp.AlbumId, fp.ArtistId }.Count(x => x != null);
        if(linkCount > 1)
            throw new ArgumentException("A post can only link to one of track, album, or artist.");

        if (dto.GenreIds is {Count: > 0}) // If any genre IDs provided
        {
            var genres = await RelationHelper.GetAllOrThrowAsync(_genreRepo, dto.GenreIds, "genre");
            fp.Genres = genres;
        }
        
        await _fpRepo.AddAsync(fp);
        await _fpRepo.SaveChangesAsync();
        return MapToDto(fp);
    }

    public async Task<ForumPostWithCommentsDto> GetForumPostByIdAsync(int id) // READ
    {
        var fp = await _fpRepo.GetByIdAsync(id);
        if(fp == null)
            return null!;

        return MapToDtoWithComments(fp);
    }

    public async Task<IEnumerable<ForumPostResponseDto>> GetForumPostsByUserIdAsync(int userId) // READ
    {
        var fps = await _fpRepo.GetForumPostsByUserIdAsync(userId);
        if(fps == null)
            return null!;

        return fps.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<ForumPostResponseDto>> GetForumPostsByTrackIdAsync(int trackId) // READ
    {
        var fps = await _fpRepo.GetForumPostsByTrackIdAsync(trackId);
        if(fps == null)
            return null!;

        return fps.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<ForumPostResponseDto>> GetForumPostsByAlbumIdAsync(int albumId) // READ
    {
        var fps = await _fpRepo.GetForumPostsByAlbumIdAsync(albumId);
        if(fps == null)
            return null!;

        return fps.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<ForumPostResponseDto>> GetForumPostsByArtistIdAsync(int artistId) // READ
    {
        var fps = await _fpRepo.GetForumPostsByArtistIdAsync(artistId);
        if(fps == null)
            return null!;

        return fps.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<ForumPostResponseDto>> GetAllForumPostsAsync() // READ
    {
        var fps = await _fpRepo.GetAllAsync();
        if(fps == null)
            return null!;

        return fps.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<ForumPostResponseDto>> SearchForumPostsByKeywordAsync(string keyword) // READ
    {
        var fps = await _fpRepo.SearchForumPostsByKeywordAsync(keyword);
        if(fps == null)
            return null!;

        return fps.Select(MapToDto).ToList();
    }

    public async Task<ForumPostResponseDto> UpdateForumPostAsync(int id, ForumPostUpdateDto dto) // UPDATE
    {
        var fp = await _fpRepo.GetByIdAsync(id);
        if(fp == null)
            return null!;

        fp.Title = dto.Title ?? fp.Title; // Update title if provided
        fp.Content = dto.Content ?? fp.Content;
        fp.TrackId = dto.TrackId ?? fp.TrackId;
        fp.AlbumId = dto.AlbumId ?? fp.AlbumId;
        fp.ArtistId = dto.ArtistId ?? fp.ArtistId;

        // Ensure that the post links to only one track, album, or artist
        var linkCount = new[] { fp.TrackId, fp.AlbumId, fp.ArtistId }.Count(x => x != null);
        if(linkCount > 1)
            throw new ArgumentException("A post can only link to one of track, album, or artist.");

        if(dto.GenreIds != null)
            fp.Genres = await RelationHelper.GetAllOrThrowAsync(_genreRepo, dto.GenreIds, "genre");

        _fpRepo.Update(fp);
        await _fpRepo.SaveChangesAsync();
        return MapToDto(fp);
    }

    public async Task<bool> DeleteForumPostAsync(int id) // DELETE
    {
        var fp = await _fpRepo.GetByIdAsync(id);
        if(fp == null)
            return false;
        
        _fpRepo.Delete(fp);
        await _fpRepo.SaveChangesAsync();
        return true;
    }

    //#########################################

    // Helper mapper ForumPost entity to ForumPostResponseDto (MapToDto)
    private ForumPostResponseDto MapToDto(ForumPost fp)
    {
        return new ForumPostResponseDto
        {
            Id = fp.Id,
            UserId = fp.UserId,
            Title = fp.Title,
            Content = fp.Content,
            TrackId = fp.TrackId,
            AlbumId = fp.AlbumId,
            ArtistId = fp.ArtistId,
            CreatedAt = fp.CreatedAt, 
            UpdatedAt = fp.UpdatedAt,
            Genres = fp.Genres
                .Select(g => new GenreIncludeDto {Id = g.Id, Name = g.Name})
                .ToList()
        };
    }

    private ForumPostWithCommentsDto MapToDtoWithComments(ForumPost fp)
    {
        return new ForumPostWithCommentsDto
        {
            Id = fp.Id,
            UserId = fp.UserId,
            Title = fp.Title,
            Content = fp.Content,
            TrackId = fp.TrackId,
            AlbumId = fp.AlbumId,
            ArtistId = fp.ArtistId,
            CreatedAt = fp.CreatedAt,
            UpdatedAt = fp.UpdatedAt,
            Genres = fp.Genres
                .Select(g => new GenreIncludeDto { Id = g.Id, Name = g.Name })
                .ToList(),
            Comments = fp.Comments
                .Select(c => new CommentIncludeDto { Id = c.Id, UserId = c.UserId, Content = c.Content, CreatedAt = c.CreatedAt })
                .ToList()
        };
    }
}