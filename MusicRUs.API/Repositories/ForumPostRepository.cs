using Microsoft.EntityFrameworkCore;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;

namespace MusicRUs.API.Repositories;

public class ForumPostRepository : Repository<ForumPost>, IForumPostRepository
{
    public ForumPostRepository(DbContext context) : base(context) {}

        private IQueryable<ForumPost> IncludeRelatedEntities() // Includes related entities for forum posts
        => _dbSet
        .Include(fp => fp.Genres)
        .Include(fp => fp.Album)
        .Include(fp => fp.Track)
        .Include(fp => fp.Artist);

        //#######################################################

    // Retrieves forum post by ID, including comments.
    public override async Task<ForumPost?> GetByIdAsync(int id)
    => await IncludeRelatedEntities()
        .Include(fp => fp.Comments)
        .ThenInclude(c => c.User)
        .FirstOrDefaultAsync(fp => fp.Id == id);

    public override async Task<IEnumerable<ForumPost>> GetAllAsync()
    => await IncludeRelatedEntities()
        .ToListAsync();

    public async Task<IEnumerable<ForumPost>> GetForumPostsByUserIdAsync(int userId)
    => await IncludeRelatedEntities()
        .Where(fp => fp.UserId == userId)
        .ToListAsync();

    public async Task<IEnumerable<ForumPost>> GetForumPostsByTrackIdAsync(int trackId)
    => await IncludeRelatedEntities()
        .Where(fp => fp.TrackId == trackId)
        .ToListAsync();

    public async Task<IEnumerable<ForumPost>> GetForumPostsByAlbumIdAsync(int albumId)
    => await IncludeRelatedEntities()
        .Where(fp => fp.AlbumId == albumId)
        .ToListAsync();

    public async Task<IEnumerable<ForumPost>> GetForumPostsByArtistIdAsync(int artistId)
    => await IncludeRelatedEntities()
        .Where(fp => fp.ArtistId == artistId)
        .ToListAsync();

    public async Task<IEnumerable<ForumPost>> GetForumPostsByGenreIdAsync(int genreId)
    => await IncludeRelatedEntities()
        .Where(fp => fp.Genres.Any(g => g.Id == genreId))
        .ToListAsync();

    public async Task<IEnumerable<ForumPost>> SearchForumPostsByKeywordAsync(string keyword)
    => await SearchAsync(fp =>  EF.Functions.ILike(fp.Title, $"%{keyword}%") || EF.Functions.ILike(fp.Content, $"%{keyword}%"));
}