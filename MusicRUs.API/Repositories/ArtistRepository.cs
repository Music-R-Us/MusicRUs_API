using Microsoft.EntityFrameworkCore;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;
using System.Linq.Expressions;

namespace MusicRUs.API.Repositories;

public class ArtistRepository : Repository<Artist>, IArtistRepository
{
    public ArtistRepository(DbContext context) : base(context)  {}

    // Private method for including albums in the query
    private IQueryable<Artist> IncludeAlbums() => _dbSet
        .AsNoTracking() // Read-only 
        .AsSplitQuery() 
        .Include(a => a.Albums);

    //##############################

    public override async Task<Artist?> GetByIdAsync(int id) // (Override) Get artist by ID with albums included
        => await IncludeAlbums().FirstOrDefaultAsync(a => a.Id == id);

    public override async Task<Artist?> GetByStringAsync(string value) // (Override) Get artist by string value with albums included
        => await IncludeAlbums().FirstOrDefaultAsync(a => a.Name == value);

    public override async Task<IEnumerable<Artist>> SearchAsync(Expression<Func<Artist, bool>> predicate)
        => await IncludeAlbums().Where(predicate).ToListAsync();
}