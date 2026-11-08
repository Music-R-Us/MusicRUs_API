using MusicRUs.API.Models.Entities;
using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.DTOs;

namespace MusicRUs.API.Services.Interfaces;

public interface IUserBlockService
{
    Task BlockUserAsync(int blockerId, int blockedId); // CREATE
    Task<List<UserBlockResponseDto>> GetBlocksByBlockerIdAsync(int blockerId); // READ
    Task UnblockUserAsync(int blockerId, int blockedId); // DELETE
}