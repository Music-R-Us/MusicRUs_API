using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using MusicRUs.API.Data;

namespace MusicRUs.API.Repositories;

public class GenreRepository : Repository<Genre>, IGenreRepository
{
    public GenreRepository(DbContext context) : base(context) {}

    public override async Task<Genre?> GetByStringAsync(string name)
        => await _dbSet.FirstOrDefaultAsync(g => g.Name == name);
}