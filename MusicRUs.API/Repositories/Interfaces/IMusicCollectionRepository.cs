using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Repositories.Interfaces;

public interface IMusicCollectionRepository : IRepository<MusicCollection>
{
    Task<IEnumerable<MusicCollection>> GetCollectionByUserIdAsync(int userId); // Get all collection items for a specific user
    Task<MusicCollection?> GetCollectionItemAsync(int userId, int collectionId); // Get a specific collection item for a user
    Task<bool> EntryExistAsync(int userId, int albumId, MediaFormat format); // Check if a specific entry exists in the user's collection
}