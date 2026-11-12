using MusicRUs.API.DTOs;

namespace MusicRUs.API.Services.Interfaces;

public interface IForumProfileService
{
    Task<ForumProfileResponseDto?> CreateFpAsync (CreateForumProfileDto dto); // CREATE (after user creation)
    Task<ForumProfileResponseDto?> GetFpByUserIdAsync(int userId); // READ
    Task<ForumProfileResponseDto?> GetFpByCurrentUserIdAsync(int userId); // READ
    Task<IEnumerable<ForumProfileResponseDto>> SearchPublicFpByKeywordAsync(string keyword); // READ
    Task<ForumProfileResponseDto?> UpdateFpAsync(int userId, ForumProfileUpdatedDto dto); // UPDATE
}