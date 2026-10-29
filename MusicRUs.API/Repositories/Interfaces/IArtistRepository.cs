using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Repositories.Interfaces;

public interface IArtistRepository : IRepository<Artist>
{
    //Task<Artist?> GetArtistWithAlbumsByIdAsync(int id);
    //Task<Artist?> GetArtistWithAlbumsByNameAsync(string name);
}