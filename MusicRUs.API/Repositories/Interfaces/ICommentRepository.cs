using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Repositories.Interfaces;

public interface ICommentRepository : IRepository<Comment>
{
    Task<Comment?> GetByUserIdAsync(int userId);
    Task<IEnumerable<Comment>?> GetCommentsByUserIdAsync(int userId);
    Task<IEnumerable<Comment>?> GetCommentsByForumPostIdAsync(int forumPostId);
}