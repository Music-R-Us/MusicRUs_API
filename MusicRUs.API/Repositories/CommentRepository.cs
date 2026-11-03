using Microsoft.EntityFrameworkCore;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;

namespace MusicRUs.API.Repositories;

public class CommentRepository : Repository<Comment>, ICommentRepository
{
    public CommentRepository(DbContext context) : base(context) { }

    public async Task<Comment?> GetByUserIdAsync(int userId)
        => await _dbSet.FirstOrDefaultAsync(c => c.UserId == userId);
        
    public async Task<IEnumerable<Comment>?> GetCommentsByUserIdAsync(int userId)
        => await _dbSet.Where(c => c.UserId == userId).ToListAsync();

    public async Task<IEnumerable<Comment>?> GetCommentsByForumPostIdAsync(int postId)
    => await _dbSet.Where(c => c.PostId == postId).ToListAsync();
}