using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.DTOs;

namespace MusicRUs.API.Services.Interfaces;

public interface IFriendshipService
{
    Task<FriendshipResponseDto> CreateFsAsync(int requesterId, int addresseeId); // CREATE
    //Task<FriendshipResponseDto> GetFsByRequesterAndAddresseeIdAsync(int requesterId, int addresseeId); // READ
    Task<List<FriendshipResponseDto>> GetFsListByUserIdAsync(int userId); // READ
    Task<List<FriendshipResponseDto>> GetPendingFsListByUserIdAsync(int userId); // READ
    Task<FriendshipResponseDto> AcceptFsAsync(int id, int userId); // UPDATE
    Task<FriendshipResponseDto> RejectFsAsync(int id, int userId); // UPDATE
    Task<bool> DeleteFsAsync(int requesterId, int addresseeId); // DELETE
}