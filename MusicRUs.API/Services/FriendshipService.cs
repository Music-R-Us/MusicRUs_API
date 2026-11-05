using Microsoft.EntityFrameworkCore;
using MusicRUs.API.DTOs;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MusicRUs.API.Services;
public class FriendshipService : IFriendshipService
{
    private readonly IFriendshipRepository _fsRepo;

    public FriendshipService(IFriendshipRepository fsRepo)
    {
        _fsRepo = fsRepo;
    }

    // Implement the methods defined in IFriendshipService interface

    public async Task<FriendshipResponseDto> CreateFsAsync(int requesterId, int addresseeId) // CREATE
    {
        var existingFs = await _fsRepo.GetFsByRequesterAndAddresseeIdAsync(requesterId, addresseeId) ?? await _fsRepo.GetFsByRequesterAndAddresseeIdAsync(addresseeId, requesterId);
        if(existingFs != null)
            return null!;

        var fs = new Friendship
        {
            RequesterId = requesterId,
            AddresseeId = addresseeId,
            Status = FriendshipStatus.Pending
        };
        await _fsRepo.AddAsync(fs);
        await _fsRepo.SaveChangesAsync();

        var saved = await _fsRepo.GetByIdAsync(fs.Id);
        return MapToDto(saved!);
    }


    public async Task<List<FriendshipResponseDto>> GetFsListByUserIdAsync(int userId) // READ
    {
        var fsList = await _fsRepo.GetFsListByUserIdAsync(userId);
        return fsList.Select(MapToDto).ToList();
    }

    public async Task<List<FriendshipResponseDto>> GetPendingFsListByUserIdAsync(int userId) // READ
    {
        var fsList = await _fsRepo.GetPendingFsListByUserIdAsync(userId);
        return fsList.Select(MapToDto).ToList();
    }


    public async Task<FriendshipResponseDto> AcceptFsAsync(int id, int userId) // UPDATE
    {
        var fs = await _fsRepo.GetByIdAsync(id)
            ?? throw new ArgumentException("Friendship request not found.");
        
        if(fs.AddresseeId != userId)
            throw new UnauthorizedAccessException("You are not allowed to accept this friendship request.");

        if (fs.Status != FriendshipStatus.Pending)
            throw new InvalidOperationException("This request is no longer pending."); 

        
        fs.Status = FriendshipStatus.Accepted; // Mark friendship as accepted
        //_fsRepo.Update(fs);
        await _fsRepo.SaveChangesAsync();

        return MapToDto(fs);
    }


    public async Task<FriendshipResponseDto> RejectFsAsync(int id, int userId) // UPDATE
    {
        var fs = await _fsRepo.GetByIdAsync(id)
            ?? throw new ArgumentException("Friendship request not found.");

        if(fs.AddresseeId != userId)
            throw new UnauthorizedAccessException("You are not allowed to reject this friendship request."); 
        
        if (fs.Status != FriendshipStatus.Pending)
            throw new InvalidOperationException("This request is no longer pending.");

        fs.Status = FriendshipStatus.Rejected;
        //_fsRepo.Update(fs);
        await _fsRepo.SaveChangesAsync();

        return MapToDto(fs);
    }


    public async Task<bool> DeleteFsAsync(int requesterId, int addresseeId) // DELETE
    {
        var fs = await _fsRepo.GetFsByRequesterAndAddresseeIdAsync(requesterId, addresseeId) ?? await _fsRepo.GetFsByRequesterAndAddresseeIdAsync(addresseeId, requesterId);
        if(fs == null)
        {
            return false;
        }
        _fsRepo.Delete(fs);

        await _fsRepo.SaveChangesAsync();
        return true;
    }

//#########################################

    // Helper method to map Friendship entity to FriendshipResponseDto (MapToDto)
    private FriendshipResponseDto MapToDto(Friendship fp) => new()
    {
        Id = fp.Id,
        RequesterId = fp.RequesterId,
        AddresseeId = fp.AddresseeId,
        Status = fp.Status,
        Requester = new UserIncludeDto
        {
            Id = fp.Requester.Id,
            Username = fp.Requester.Username
        },
        Addressee = new UserIncludeDto
        {
            Id = fp.Addressee.Id,
            Username = fp.Addressee.Username
        }
    };
}