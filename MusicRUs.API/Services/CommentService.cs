using MusicRUs.API.DTOs;
using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;

namespace MusicRUs.API.Services;
public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepo;
    private readonly IUserRepository _userRepo;
    private readonly IForumPostRepository _fpRepo;
    private readonly IUserBlockRepository _ubRepo;

    public CommentService(ICommentRepository commentRepo, IUserRepository userRepo, IForumPostRepository fpRepo, IUserBlockRepository ubRepo)
    {
        _commentRepo = commentRepo;
        _userRepo = userRepo;
        _fpRepo = fpRepo;
        _ubRepo = ubRepo;
    }

    // Implement the methods defined in ICommentService interface

    public async Task<CommentResponseDto> CreateCommentAsync(int postId, int userId, int? viewerId, CreateCommentDto dto) // CREATE
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        var post = await _fpRepo.GetByIdAsync(postId);
        if (post == null)
            throw new KeyNotFoundException("Forum post not found");

        // Check if the post author has blocked the viewer
        if(viewerId != null)
        {
            var isBlocked = await _ubRepo.IsBlockedEitherWayAsync(viewerId.Value, post.UserId);
            if(isBlocked)
                throw new KeyNotFoundException("Forum post not found");
        }

        var comment = new Comment // Map properties from dto to entity
        {
            PostId = postId,
            UserId = userId,
            Content = dto.Content
        };
        await _commentRepo.AddAsync(comment);
        await _commentRepo.SaveChangesAsync();
        return MapToDto(comment);
    }

    public async Task<CommentResponseDto> GetCommentByIdAsync(int id, int? viewerId) // READ
    {
        var comment = await _commentRepo.GetByIdAsync(id);
        if (comment == null)
            throw new KeyNotFoundException("Comment not found");

        if(viewerId != null && await _ubRepo.IsBlockedEitherWayAsync(viewerId.Value, comment.UserId))
            throw new KeyNotFoundException("Comment not found");

        return MapToDto(comment);
    }

    public async Task<CommentResponseDto> GetCommentByUserIdAsync(int userId, int? viewerId) // READ
    {
        var comment = await _commentRepo.GetByUserIdAsync(userId);
        if (comment == null)
            throw new KeyNotFoundException("Comment not found");

        if(viewerId != null && await _ubRepo.IsBlockedEitherWayAsync(viewerId.Value, comment.UserId))
            throw new KeyNotFoundException("Comment not found");

        return MapToDto(comment);
    }

    public async Task<IEnumerable<CommentResponseDto>> GetAllCommentsByForumPostIdAsync(int postId, int? viewerId) // READ
    {
        var post = await _fpRepo.GetByIdAsync(postId);
        if (post == null)
            throw new KeyNotFoundException("Forum post not found");

        // Get the list of blocked user IDs for the viewer, if viewerId is provided
        var blockedIds = viewerId.HasValue ? await _ubRepo.GetBlockedUserIdsAsync(viewerId.Value) : [];

        // If the post author has blocked the viewer, treat the post as not found
        if(blockedIds.Contains(post.UserId))
            throw new KeyNotFoundException("Forum post not found");

        var comments = await _commentRepo.GetAllCommentsByForumPostIdAsync(postId);
        return comments
            .Where(c => !blockedIds.Contains(c.UserId))
            .OrderBy(c => c.CreatedAt)
            .Select(MapToDto)
            .ToList();
    }

    public async Task<CommentResponseDto> UpdateCommentAsync(int id, int userId, CommentUpdateDto dto) // UPDATE
    {
        var comment = await _commentRepo.GetByIdAsync(id);
        if (comment == null)
            throw new KeyNotFoundException("Comment not found");

        if (comment.UserId != userId)
            throw new UnauthorizedAccessException("User not authorized to update this comment");

        comment.Content = dto.Content;

        _commentRepo.Update(comment);
        await _commentRepo.SaveChangesAsync();
        return MapToDto(comment);
    }

    public async Task DeleteCommentAsync(int id, int userId) // DELETE
    {
        var comment = await _commentRepo.GetByIdAsync(id);
        if (comment == null)
            throw new KeyNotFoundException("Comment not found");

        if (comment.UserId != userId)
            throw new UnauthorizedAccessException("User not authorized to delete this comment");

        _commentRepo.Delete(comment);
        await _commentRepo.SaveChangesAsync();
    }


    //#########################################

    // Helper method to map User entity to UserResponseDto (MapToDto)
    private static CommentResponseDto MapToDto(Comment comment)
    {
        return new CommentResponseDto
        {
            Id = comment.Id,
            PostId = comment.PostId,
            UserId = comment.UserId,
            User = new UserIncludeDto
            {
                Id = comment.User.Id,
                Username = comment.User.Username
            },
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };
    }
}