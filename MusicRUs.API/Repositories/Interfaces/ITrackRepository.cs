using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Repositories.Interfaces;

public interface ITrackRepository : IRepository<Track>
{
    Task<IEnumerable<Track>> SearchTracksByKeywordAsync(string keyword);
}