using MusicRUs.API.DTOs;
using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;

namespace MusicRUs.API.Services;
public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepo;

    public CommentService(ICommentRepository commentRepo)
    {
        _commentRepo = commentRepo;
    }

    // Implement the methods defined in ICommentService interface

    public async Task<CommentResponseDto> CreateCommentAsync(int postId, CreateCommentDto dto) // CREATE
    {
        var comment = new Comment // Map properties from dto to entity
        {
            PostId = dto.PostId,
            UserId = dto.UserId,
            Content = dto.Content
        };
        await _commentRepo.AddAsync(comment);
        await _commentRepo.SaveChangesAsync();
        return MapToDto(comment);
    }

    public async Task<CommentResponseDto> GetCommentByIdAsync(int id) // READ
    {
        var comment = await _commentRepo.GetByIdAsync(id);
        if (comment == null)
            return null!; 

        return MapToDto(comment);
    }

    public async Task<CommentResponseDto> GetCommentByUserIdAsync(int userId) // READ
    {
        var comment = await _commentRepo.GetByUserIdAsync(userId);
        if (comment == null)
            return null!;

        return MapToDto(comment);
    }

    public async Task<IEnumerable<CommentResponseDto>> GetCommentsByForumPostIdAsync(int postId) // READ
    {
        var comments = await _commentRepo.GetCommentsByForumPostIdAsync(postId);
        if (comments == null || !comments.Any())
            return null!;

        return comments.Select(MapToDto).ToList();
    }

    public async Task<CommentResponseDto> UpdateCommentAsync(int id, CommentUpdateDto dto) // UPDATE
    {
        var comment = await _commentRepo.GetByIdAsync(id);
        if (comment == null)
            return null!;

        comment.Content = dto.Content;

        _commentRepo.Update(comment);
        await _commentRepo.SaveChangesAsync();
        return MapToDto(comment);
    }

    public async Task<bool> DeleteCommentAsync(int id) // DELETE
    {
        var comment = await _commentRepo.GetByIdAsync(id);
        if (comment == null)
            return false;

        _commentRepo.Delete(comment);
        await _commentRepo.SaveChangesAsync();
        return true;
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
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };
    }
}