using Microsoft.EntityFrameworkCore;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;

namespace MusicRUs.API.Repositories;

public class ForumPostRepository : Repository<ForumPost>, IForumPostRepository
{
    public ForumPostRepository(DbContext context) : base(context) {}

    // Retrieves forum post by ID, including comments.
    public override async Task<ForumPost?> GetByIdAsync(int id)
    => await _dbSet
        .Include(fp => fp.Comments)
        .Include(fp => fp.Genres)
        .FirstOrDefaultAsync(fp => fp.Id == id);

    public async Task<IEnumerable<ForumPost>?> GetForumPostsByUserIdAsync(int userId)
    => await _dbSet
        .Where(fp => fp.UserId == userId)
        .ToListAsync();

    public async Task<IEnumerable<ForumPost>?> GetForumPostsByTrackIdAsync(int trackId)
    => await _dbSet
        .Include(fp => fp.Genres)
        .Where(fp => fp.TrackId == trackId)
        .ToListAsync();

    public async Task<IEnumerable<ForumPost>?> GetForumPostsByAlbumIdAsync(int albumId)
    => await _dbSet
        .Include(fp => fp.Genres)
        .Where(fp => fp.AlbumId == albumId)
        .ToListAsync();

    public async Task<IEnumerable<ForumPost>?> GetForumPostsByArtistIdAsync(int artistId)
    => await _dbSet
        .Include(fp => fp.Genres)
        .Where(fp => fp.ArtistId == artistId)
        .ToListAsync();

    public async Task<IEnumerable<ForumPost>?> GetForumPostsByGenreIdAsync(int genreId)
    => await _dbSet
        .Include(fp => fp.Genres)
        .Where(fp => fp.Genres.Any(g => g.Id == genreId))
        .ToListAsync();

    public async Task<IEnumerable<ForumPost>?> SearchForumPostsByKeywordAsync(string keyword)
    => await SearchAsync(fp =>  EF.Functions.ILike(fp.Title, $"%{keyword}%") || EF.Functions.ILike(fp.Content, $"%{keyword}%"));
}