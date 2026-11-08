using MusicRUs.API.Models.Entities;
using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.Repositories.Interfaces;
using MusicRUs.API.DTOs;

namespace MusicRUs.API.Services;

public class UserBlockService : IUserBlockService
{
    private readonly IUserBlockRepository _ubRepo;
    private readonly IUserRepository _userRepo;
    private readonly IFriendshipRepository _fsRepo;

    public UserBlockService(IUserBlockRepository ubRepo, IUserRepository userRepo, IFriendshipRepository fsRepo)
    {
        _ubRepo = ubRepo;
        _userRepo = userRepo;
        _fsRepo = fsRepo;
    }

    // Implement the methods defined in IUserBlockService interface

    public async Task BlockUserAsync(int blockerId, int blockedId) // CREATE
    {
        if(blockerId == blockedId)
            throw new ArgumentException("A user cannot block themselves.");
        
        var blockedUser = await _userRepo.GetByIdAsync(blockedId);
        if(blockedUser == null)
            throw new KeyNotFoundException("The user to be blocked does not exist.");

        if(await _ubRepo.GetBlockAsync(blockerId, blockedId) != null)
            throw new InvalidOperationException("The user is already blocked.");
        
        var userBlock = new UserBlock
        {
            BlockerId = blockerId,
            BlockedId = blockedId
        };

        await _ubRepo.AddAsync(userBlock);

        var fs = await _fsRepo.GetFsByRequesterAndAddresseeIdAsync(blockerId, blockedId) ?? await _fsRepo.GetFsByRequesterAndAddresseeIdAsync(blockedId, blockerId);
        if(fs != null)
            _fsRepo.Delete(fs);

        await _ubRepo.SaveChangesAsync();
    }

    public async Task<List<UserBlockResponseDto>> GetBlocksByBlockerIdAsync(int blockerId) // READ
    {
        var blocks = await _ubRepo.GetBlocksByBlockerIdAsync(blockerId);
        return blocks.Select(MapToDto).ToList();
    }

    public async Task UnblockUserAsync(int blockerId, int blockedId) // DELETE
    {
        var userBlock = await _ubRepo.GetBlockAsync(blockerId, blockedId);
        if(userBlock == null)
            throw new KeyNotFoundException("The block does not exist.");

        _ubRepo.Delete(userBlock);
        await _ubRepo.SaveChangesAsync();
    }
//#########################################

    // Helper method to map User entity to UserBlockResponseDto (MapToDto)
    private static UserBlockResponseDto MapToDto(UserBlock ub)
    {
        return new UserBlockResponseDto
        {
            BlockerId = ub.BlockerId,
            BlockedId = ub.BlockedId, 
            Blocked = new UserIncludeDto
            {
                Id = ub.Blocked.Id,
                Username = ub.Blocked.Username
            }
        };
    }
}