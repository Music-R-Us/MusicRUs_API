using MusicRUs.API.DTOs;

namespace MusicRUs.API.Services.Interfaces
{
    public interface ICommentService
    {
        Task<CommentResponseDto> CreateCommentAsync(int postId, CreateCommentDto dto); // CREATE
        Task<CommentResponseDto> GetCommentByIdAsync(int id); // READ
        Task<CommentResponseDto> GetCommentByUserIdAsync(int userId); // READ
        Task<IEnumerable<CommentResponseDto>> GetAllCommentsByForumPostIdAsync(int postId); // READ
        Task<CommentResponseDto> UpdateCommentAsync(int id,CommentUpdateDto dto); // UPDATE
        Task<bool> DeleteCommentAsync(int commentId); // DELETE
    }
}