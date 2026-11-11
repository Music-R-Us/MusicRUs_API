using MusicRUs.API.DTOs;
using MusicRUs.API.Backend.Logic.GoogleAuth;
using MusicRUs.API.Services;

namespace MusicRUs.API.Services.Interfaces;

public interface IUserService
{
    Task<UserResponseDto?> CreateUserAsync(CreateUserDto dto); // CREATE
    
    // GOOGLE LOGIN
    Task<UpdateUserResult> GoogleLoginAsync(GoogleUserInfo googleUser); // CREATE or READ
    Task<UserResponseDto?> GetUserByIdAsync(int id); // READ
    Task<UserResponseDto?> GetUserByUsernameAndPasswordAsync(string username, string password); // READ
    Task<UserResponseDto?> GetUserByEmailAndPasswordAsync(string email, string password); // READ
    Task<IEnumerable<UserResponseDto>> SearchUsersByKeywordAsync(string keyword); // READ
    Task<UserResponseDto?> UpdateUsernameAsync(int id, UpdateUsernameDto dto); // UPDATE
    Task<UpdateUserResult> UpdateEmailAsync(int id, UpdateEmailDto dto); // UPDATE
    Task<UpdateUserResult> UpdatePasswordAsync(int id, UpdatePasswordDto dto); // UPDATE
    Task<bool> DeleteUserAsync(int id); // DELETE
}