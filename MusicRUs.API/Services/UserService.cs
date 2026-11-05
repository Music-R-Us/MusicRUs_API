using MusicRUs.API.DTOs;
using MusicRUs.API.Backend.Logic.GoogleAuth;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;
using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.Backend.Logic.Hashing;

namespace MusicRUs.API.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;

    private readonly IPasswordHasher _passwordHasher;

     public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepo = userRepository;
        _passwordHasher = passwordHasher;
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
            PasswordHash = _passwordHasher.Hash(dto.Password).Value,
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
        if (user?.PasswordHash is null || !_passwordHasher.Verify(password, HashedPassword.FromStoredHash(user.PasswordHash)))
            return null;
        return MapToDto(user);
    }

    public async Task<UserResponseDto?> GetUserByEmailAndPasswordAsync(string email, string password) // READ
    {
        var user = await _userRepo.GetUserByEmailAsync(email);
        if (user?.PasswordHash is null || !_passwordHasher.Verify(password, HashedPassword.FromStoredHash(user.PasswordHash)))
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

    public async Task<UpdateUserResult> UpdateEmailAsync(int id, UpdateEmailDto dto) // UPDATE
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user is null)
            return UpdateUserResult.Fail(UpdateUserError.NotFound);

        if (user.PasswordHash is null
            || !_passwordHasher.Verify(dto.CurrentPassword, HashedPassword.FromStoredHash(user.PasswordHash)))
            return UpdateUserResult.Fail(UpdateUserError.WrongPassword);

        var newEmail = dto.NewEmail.Trim();

        // Only check for duplicates if it is actually a different e-mail.
        // The column is citext, so "Mark@mark.dk" and "mark@mark.dk" count as the same
        var isSameEmail = string.Equals(user.Email, newEmail, StringComparison.OrdinalIgnoreCase);
        if (!isSameEmail && await _userRepo.EmailExistsAsync(newEmail))
            return UpdateUserResult.Fail(UpdateUserError.EmailInUse);

        user.Email = newEmail;
        user.UpdatedAt = DateTime.UtcNow;
        _userRepo.Update(user);
        await _userRepo.SaveChangesAsync();
        return UpdateUserResult.Ok(MapToDto(user));
    }

    public async Task<UpdateUserResult> UpdatePasswordAsync(int id, UpdatePasswordDto dto) // UPDATE
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user is null)
            return UpdateUserResult.Fail(UpdateUserError.NotFound);

        if (user.PasswordHash is null
            || !_passwordHasher.Verify(dto.CurrentPassword, HashedPassword.FromStoredHash(user.PasswordHash)))
            return UpdateUserResult.Fail(UpdateUserError.WrongPassword);

        user.PasswordHash = _passwordHasher.Hash(dto.NewPassword).Value;
        user.UpdatedAt = DateTime.UtcNow;
        _userRepo.Update(user);
        await _userRepo.SaveChangesAsync();
        return UpdateUserResult.Ok(MapToDto(user));
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