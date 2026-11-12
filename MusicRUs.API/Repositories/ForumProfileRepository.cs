using Microsoft.EntityFrameworkCore;
using MusicRUs.API.Data;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;

namespace MusicRUs.API.Repositories;

public class ForumProfileRepository : Repository<ForumProfile>, IForumProfileRepository
{
    public ForumProfileRepository(DbContext context) : base(context) {}

    public async Task<ForumProfile?> GetFpByUserIdAsync(int userId)
        => await _dbSet.FirstOrDefaultAsync(fp => fp.UserId == userId);

    public async Task<ForumProfile?> GetWithFavoritesAsync(int userId)
        => await _dbSet
            .Include(fp => fp.FavoriteArtists).ThenInclude(fa => fa.Artist)
            .Include(fp => fp.FavoriteAlbums).ThenInclude(fa => fa.Album)
            .Include(fp => fp.Genres)
            .FirstOrDefaultAsync(fp => fp.UserId == userId);
    
    public async Task<IEnumerable<ForumProfile>> SearchPublicFpByKeywordAsync(string keyword)
        => await SearchAsync(fp => EF.Functions.ILike(fp.DisplayName!, $"%{keyword}%") && fp.IsPublic == true);
}