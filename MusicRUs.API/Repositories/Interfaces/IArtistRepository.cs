using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Repositories.Interfaces;

public interface IArtistRepository : IReadRepository<Artist>
{
    //Task<Artist?> GetArtistWithAlbumsByIdAsync(int id);
    //Task<Artist?> GetArtistWithAlbumsByNameAsync(string name);
}