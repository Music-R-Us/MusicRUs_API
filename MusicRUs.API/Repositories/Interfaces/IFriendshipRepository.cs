using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;

namespace MusicRUs.API.Repositories.Interfaces;

public interface IFriendshipRepository : IRepository<Friendship>
{
    Task<Friendship?> GetFsByRequesterAndAddresseeIdAsync(int requesterId, int addresseeId);
    Task<List<Friendship>> GetFsListByUserIdAsync(int userId);
    Task<List<Friendship>> GetPendingFsListByUserIdAsync(int userId);

}