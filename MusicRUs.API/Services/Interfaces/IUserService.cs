using MusicRUs.API.DTOs;

namespace MusicRUs.API.Services.Interfaces;

public interface IUserService
{
    Task<UserResponseDto?> CreateUserAsync(CreateUserDto dto); // CREATE
    Task<UserResponseDto?> GetUserByIdAsync(int id); // READ
    Task<UserResponseDto?> GetUserByUsernameAndPasswordAsync(string username, string password); // READ
    Task<UserResponseDto?> GetUserByEmailAndPasswordAsync(string email, string password); // READ
    Task<IEnumerable<UserResponseDto>> SearchUsersByKeywordAsync(string keyword); // READ
    //Task<UserResponseDto?> UpdateUsernameAsync(int id, UpdateUsernameDto dto); // UPDATE
    //Task<UserResponseDto?> UpdateEmailAsync(int id, UpdateEmailDto dto); // UPDATE
    //Task<UserResponseDto?> UpdatePasswordAsync(int id, UpdatePasswordDto dto); // UPDATE
    Task<bool> DeleteUserAsync(int id); // DELETE
}