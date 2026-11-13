using Microsoft.EntityFrameworkCore;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;

namespace MusicRUs.API.Repositories;

public class CommentRepository : Repository<Comment>, ICommentRepository
{
    public CommentRepository(DbContext context) : base(context) { }

    public override async Task<Comment?> GetByIdAsync(int id)
        => await _dbSet.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Comment?> GetByUserIdAsync(int userId)
        => await _dbSet.Include(c => c.User).FirstOrDefaultAsync(c => c.UserId == userId);

    public async Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(int userId)
        => await _dbSet.Where(c => c.UserId == userId).ToListAsync();

    public async Task<IEnumerable<Comment>> GetAllCommentsByForumPostIdAsync(int postId)
        => await _dbSet
            .Where(c => c.PostId == postId)
            .Include(c => c.User)
            .ToListAsync();
}