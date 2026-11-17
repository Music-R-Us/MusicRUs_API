using MusicRUs.API.DTOs;

namespace MusicRUs.API.Services.Interfaces
{
    public interface ICommentService
    {
        Task<CommentResponseDto> CreateCommentAsync(int postId, int userId, int? viewerId, CreateCommentDto dto); // CREATE
        Task<CommentResponseDto> GetCommentByIdAsync(int id, int? viewerId); // READ
        Task<CommentResponseDto> GetCommentByUserIdAsync(int userId, int? viewerId); // READ
        Task<IEnumerable<CommentResponseDto>> GetAllCommentsByForumPostIdAsync(int postId, int? viewerId); // READ
        Task<CommentResponseDto> UpdateCommentAsync(int id, int userId, CommentUpdateDto dto); // UPDATE
        Task DeleteCommentAsync(int Id, int userId); // DELETE
    }
}