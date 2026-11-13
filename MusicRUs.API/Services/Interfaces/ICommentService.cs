using MusicRUs.API.DTOs;

namespace MusicRUs.API.Services.Interfaces
{
    public interface ICommentService
    {
        Task<CommentResponseDto> CreateCommentAsync(int postId, int userId, CreateCommentDto dto); // CREATE
        Task<CommentResponseDto> GetCommentByIdAsync(int id); // READ
        Task<CommentResponseDto> GetCommentByUserIdAsync(int userId); // READ
        Task<IEnumerable<CommentResponseDto>> GetAllCommentsByForumPostIdAsync(int postId); // READ
        Task<CommentResponseDto> UpdateCommentAsync(int id, int userId, CommentUpdateDto dto); // UPDATE
        Task DeleteCommentAsync(int Id, int userId); // DELETE
    }
}