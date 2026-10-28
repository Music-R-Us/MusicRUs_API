using Microsoft.EntityFrameworkCore;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;
using MusicRUs.API.Data;

namespace MusicRUs.API.Repositories;

public class MusicCollectionRepository : Repository<MusicCollection>, IMusicCollectionRepository
{
    public MusicCollectionRepository(DbContext context) : base(context) {}

    public async Task<IEnumerable<MusicCollection>> GetCollectionByUserIdAsync(int userId)
        => await _dbSet
            .Where(mc => mc.UserId == userId)
            //.Include(mc => mc.Album) // Include related albums in the collection
            //.ThenInclude(a => a.Artists) // Include related artist for each album
            .ToListAsync();

    public async Task<MusicCollection?> GetCollectionItemAsync(int userId, int collectionId)
        => await _dbSet.FirstOrDefaultAsync(mc => mc.UserId == userId && mc.Id == collectionId);

    public async Task<bool> EntryExistAsync(int userId, int albumId, MediaFormat format)
        => await _dbSet.AnyAsync(mc => mc.UserId == userId && mc.AlbumId == albumId && mc.Format == format);
}