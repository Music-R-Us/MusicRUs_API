using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Repositories.Interfaces;

public interface ITrackRepository : IReadRepository<Track>
{
    Task<IEnumerable<Track>> SearchTracksByKeywordAsync(string keyword);
}