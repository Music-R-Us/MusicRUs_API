using MusicRUs.API.Models.Entities;
namespace MusicRUs.API.Repositories.Interfaces;

public interface IGenreRepository : IRepository<Genre>
{
    Task<List<Genre>> GetGenresByIdsAsync(IEnumerable<int> ids);
}