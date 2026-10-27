using MusicRUs.API.DTOs;
using MusicRUs.API.Backend.Logic.GoogleAuth;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;
using MusicRUs.API.Services.Interfaces;

namespace MusicRUs.API.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;

    public UserService(IUserRepository userRepository)
    {
        _userRepo = userRepository;
    }

    // Implement the methods defined in IUserService interface
    public async Task<UserResponseDto?> CreateUserAsync(CreateUserDto dto) // CREATE
    {
        // Check if a user with the same email or username already exists
        if (await _userRepo.EmailExistsAsync(dto.Email) || await _userRepo.UsernameExistsAsync(dto.Username))
        {
            return null; // User with the same email or username already exists
        }

        // Create new User entity
        var user = new User // Map properties from dto to entity
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = dto.Password,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _userRepo.AddAsync(user);
        await _userRepo.SaveChangesAsync();
        return MapToDto(user);
    }

    public async Task<UserResponseDto?> GoogleLoginAsync(GoogleUserInfo googleUser) // CREATE, READ or UPDATE
    {
        // Check if user exists and return existing user
        var user = await _userRepo.GetByGoogleIdAsync(googleUser.GoogleId);
        if (user is not null) 
        {
            return MapToDto(user); 
        }

        // Check if user exists via email and return existing user
        user = await _userRepo.GetUserByEmailAsync(googleUser.Email);
        if (user is not null)
        {
            user.GoogleId = googleUser.GoogleId; // Link Google ID to existing user
            await _userRepo.SaveChangesAsync();
            return MapToDto(user);
        }

        // Create new user if not found
        user = new User
        {
            Username = googleUser.Name,
            Email = googleUser.Email,
            PasswordHash = null, // No password for Google login
            GoogleId = googleUser.GoogleId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _userRepo.AddAsync(user);
        await _userRepo.SaveChangesAsync();
        return MapToDto(user);
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(int id) // READ
    {
        var user = await _userRepo.GetByIdAsync(id);
        return user is null ? null : MapToDto(user);
    }

    public async Task<UserResponseDto?> GetUserByUsernameAndPasswordAsync(string username, string password) // READ
    {
        var user = await _userRepo.GetUserByUsernameAsync(username);
        if (user is null || user.PasswordHash != password)
        {
            return null; // User not found or password mismatch
        }
        return MapToDto(user);
    }

    public async Task<UserResponseDto?> GetUserByEmailAndPasswordAsync(string email, string password) // READ
    {
        var user = await _userRepo.GetUserByEmailAsync(email);
        if (user is null || user.PasswordHash != password)
        {
            return null; // User not found or password mismatch
        }
        return MapToDto(user);
    }

    public async Task<IEnumerable<UserResponseDto>> SearchUsersByKeywordAsync(string keyword) // READ
    {
        var users = await _userRepo.SearchUsersByKeywordAsync(keyword);
        return users.Select(MapToDto);
    }

    public async Task<UserResponseDto?> UpdateUsernameAsync(int id, UpdateUsernameDto dto) // UPDATE
    {
        var user = await _userRepo.GetByIdAsync(id); 
        if (user is null)
        {
            return null; // User not found
        }
        user.Username = dto.NewUsername;
        user.UpdatedAt = DateTime.UtcNow;
        _userRepo.Update(user);
        await _userRepo.SaveChangesAsync();
        return MapToDto(user);
    }

    public async Task<UserResponseDto?> UpdateEmailAsync(int id, UpdateEmailDto dto) // UPDATE
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user is null)
        {
            return null; // User not found
        }
        user.Email = dto.NewEmail;
        user.UpdatedAt = DateTime.UtcNow;
        _userRepo.Update(user);
        await _userRepo.SaveChangesAsync();
        return MapToDto(user);
    }

    public async Task<UserResponseDto?> UpdatePasswordAsync(int id, UpdatePasswordDto dto) // UPDATE
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user is null)
        {
            return null; // User not found
        }
        user.PasswordHash = dto.NewPassword;
        user.UpdatedAt = DateTime.UtcNow;
        _userRepo.Update(user);
        await _userRepo.SaveChangesAsync();
        return MapToDto(user);
    }

    public async Task<bool> DeleteUserAsync(int id) // DELETE
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user is null)
        {
            return false; // User not found
        }
        else
        {
            _userRepo.Delete(user);
            await _userRepo.SaveChangesAsync();
            return true;
        }
    }

//#########################################

    // Helper method to map User entity to UserResponseDto (MapToDto)
    private static UserResponseDto MapToDto(User user) 
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}