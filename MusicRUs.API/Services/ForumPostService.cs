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
    private readonly IUserRepository _userRepo;
    private readonly IUserBlockRepository _ubRepo;

    public ForumPostService(IForumPostRepository fpRepo, IGenreRepository genreRepo, IUserRepository userRepo, IUserBlockRepository ubRepo) 
    {
        _fpRepo = fpRepo;  // Dependency Inversion for ForumPost repository (Dependency Inversion Principle)
        _genreRepo = genreRepo;  // Dependency Inversion for Genre repository (Dependency Inversion Principle)
        _userRepo = userRepo; // Dependency Inversion for User repository (Dependency Inversion Principle)
        _ubRepo = ubRepo; // Dependency Inversion for UserBlock repository (Dependency Inversion Principle)
    }

    // Implement the methods defined in IForumPost interface

    public async Task<ForumPostResponseDto> CreateForumPostAsync(int userId, CreateForumPostDto dto) // CREATE
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if(user == null)
            throw new KeyNotFoundException("User not found.");

        // Create new ForumPost entity
        var fp = new ForumPost // Map properties from dto to entity
        {
            UserId = userId,
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
        
        var saved = await _fpRepo.GetByIdAsync(fp.Id); // Retrieve updated row with related entities
        return MapToDto(saved!); // Map the saved entity to DTO and return
    }

    public async Task<ForumPostWithCommentsDto> GetForumPostByIdAsync(int id, int? viewerId) // READ
    {
        var fp = await _fpRepo.GetByIdAsync(id);
        if(fp == null)
            throw new KeyNotFoundException("Forum post not found.");

        // Get blocked user IDs for the viewer, if viewerId is provided
        var blockedIds = viewerId.HasValue ? await _ubRepo.GetBlockedUserIdsAsync(viewerId.Value) : [];

        // Post as not found if blocked
        if(blockedIds.Contains(fp.UserId))
            throw new KeyNotFoundException("Forum post not found.");

        return MapToDtoWithComments(fp, blockedIds);
    }

    public async Task<IEnumerable<ForumPostResponseDto>> GetForumPostsByUserIdAsync(int userId, int? viewerId) // READ
    {
        if(viewerId != null && await _ubRepo.IsBlockedEitherWayAsync(viewerId.Value, userId))
            throw new KeyNotFoundException("User not found.");

        var user = await _userRepo.GetByIdAsync(userId);
        if(user == null)
            throw new KeyNotFoundException("User not found.");
        
        var fps = await _fpRepo.GetForumPostsByUserIdAsync(user.Id);

        return fps.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<ForumPostResponseDto>> GetForumPostsByTrackIdAsync(int trackId, int? viewerId) // READ
    {
        var fps = await _fpRepo.GetForumPostsByTrackIdAsync(trackId);
        fps = await FilterBlockedAsync(fps, viewerId); 
        return fps.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<ForumPostResponseDto>> GetForumPostsByAlbumIdAsync(int albumId, int? viewerId) // READ
    {
        var fps = await _fpRepo.GetForumPostsByAlbumIdAsync(albumId);
        fps = await FilterBlockedAsync(fps, viewerId);
        return fps.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<ForumPostResponseDto>> GetForumPostsByArtistIdAsync(int artistId, int? viewerId) // READ
    {
        var fps = await _fpRepo.GetForumPostsByArtistIdAsync(artistId);
        fps = await FilterBlockedAsync(fps, viewerId);
        return fps.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<ForumPostResponseDto>> GetForumPostsByGenreIdAsync(int genreId, int? viewerId) // READ
    {
        var fps = await _fpRepo.GetForumPostsByGenreIdAsync(genreId);
        fps = await FilterBlockedAsync(fps, viewerId);
        return fps.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<ForumPostResponseDto>> GetAllForumPostsAsync(int? viewerId) // READ
    {
        var fps = await _fpRepo.GetAllAsync();
        fps = await FilterBlockedAsync(fps, viewerId);
        return fps.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<ForumPostResponseDto>> SearchForumPostsByKeywordAsync(string keyword, int? viewerId) // READ
    {
        var fps = await _fpRepo.SearchForumPostsByKeywordAsync(keyword);
        fps = await FilterBlockedAsync(fps, viewerId);
        return fps.Select(MapToDto).ToList();
    }

    public async Task<ForumPostResponseDto> UpdateForumPostAsync(int id, int userId, ForumPostUpdateDto dto) // UPDATE
    {
        var fp = await _fpRepo.GetByIdAsync(id);
        if(fp == null)
            throw new KeyNotFoundException("Forum post not found.");
        
        if(fp.UserId != userId)
            throw new UnauthorizedAccessException("User is not authorized to update this post.");

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

        var saved = await _fpRepo.GetByIdAsync(fp.Id); // Retrieve updated row with related entities
        return MapToDto(saved!); // Map the saved entity to DTO and return
    }

    public async Task DeleteForumPostAsync(int id, int userId) // DELETE
    {

        var fp = await _fpRepo.GetByIdAsync(id);
        if(fp == null)
            throw new KeyNotFoundException("Forum post not found.");
        
        if(fp.UserId != userId)
            throw new UnauthorizedAccessException("User is not authorized to delete this post.");

        _fpRepo.Delete(fp);
        await _fpRepo.SaveChangesAsync();
    }

    //#########################################

    // Helper method to filter out posts from blocked users for the viewer
    private async Task<IEnumerable<ForumPost>> FilterBlockedAsync(IEnumerable<ForumPost> posts, int? viewerId)
    {
        if(viewerId == null)
            return posts;

        var blockedIds = await _ubRepo.GetBlockedUserIdsAsync(viewerId.Value);
        return posts.Where(p => !blockedIds.Contains(p.UserId));
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
            Track = fp.Track is null ? null : new TrackIncludeDto { Id = fp.Track.Id, Title = fp.Track.Title },
            Album = fp.Album is null ? null : new AlbumIncludeDto { Id = fp.Album.Id, Title = fp.Album.Title , ImageUrl = fp.Album.ImageUrl},
            Artist = fp.Artist is null ? null : new ArtistIncludeDto { Id = fp.Artist.Id, Name = fp.Artist.Name , ImageUrl = fp.Artist.ImageUrl},
            CreatedAt = fp.CreatedAt, 
            UpdatedAt = fp.UpdatedAt,
            Genres = fp.Genres
                .Select(g => new GenreIncludeDto {Id = g.Id, Name = g.Name})
                .ToList()
        };
    }

    private ForumPostWithCommentsDto MapToDtoWithComments(ForumPost fp, List<int> blockedIds)
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
            Track = fp.Track is null ? null : new TrackIncludeDto { Id = fp.Track.Id, Title = fp.Track.Title },
            Album = fp.Album is null ? null : new AlbumIncludeDto { Id = fp.Album.Id, Title = fp.Album.Title , ImageUrl = fp.Album.ImageUrl},
            Artist = fp.Artist is null ? null : new ArtistIncludeDto { Id = fp.Artist.Id, Name = fp.Artist.Name , ImageUrl = fp.Artist.ImageUrl},
            CreatedAt = fp.CreatedAt,
            UpdatedAt = fp.UpdatedAt,
            Genres = fp.Genres
                .Select(g => new GenreIncludeDto { Id = g.Id, Name = g.Name })
                .ToList(),
            Comments = fp.Comments
                .Where(c => !blockedIds.Contains(c.UserId))
                .Select(c => new CommentIncludeDto { 
                    Id = c.Id, 
                    PostId = c.PostId,
                    UserId = c.UserId, 
                    User = new UserIncludeDto { Id = c.User.Id, Username = c.User.Username },
                    Content = c.Content, 
                    CreatedAt = c.CreatedAt, 
                    UpdatedAt = c.UpdatedAt })
                .ToList()
        };
    }
}