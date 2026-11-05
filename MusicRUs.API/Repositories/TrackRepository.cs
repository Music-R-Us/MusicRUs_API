using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MusicRUs.API.Repositories;

public class TrackRepository : Repository<Track>, ITrackRepository
{
    public TrackRepository(DbContext context) : base(context) {}

    public async Task<IEnumerable<Track>> SearchTracksByKeywordAsync(string keyword)
        => await _dbSet
        .AsNoTracking()
        .AsSplitQuery()
        .Where(t => EF.Functions.ILike(t.Title, $"%{keyword}%"))
        .Include(t => t.Artists)
        .Include(t => t.Album)
        .ToListAsync();
}