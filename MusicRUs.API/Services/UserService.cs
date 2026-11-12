using MusicRUs.API.DTOs;
using MusicRUs.API.Backend.Logic.GoogleAuth;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;
using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.Backend.Logic.Hashing;
using MusicRUs.API.Backend.Logic.Encryption;

namespace MusicRUs.API.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;

    private readonly IPasswordHasher _passwordHasher;
    private readonly IEncryptionService _encryptionService;
    private const int MaxFailedLoginAttempts = 3;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);
    private readonly ITwoFactorService _twoFactorService;

     public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, IEncryptionService encryptionService, ITwoFactorService twoFactorService)
    {
        _userRepo = userRepository;
        _passwordHasher = passwordHasher;
        _encryptionService = encryptionService;
        _twoFactorService = twoFactorService;
    }

    // Implement the methods defined in IUserService interface
    public async Task<UserResponseDto?> CreateUserAsync(CreateUserDto dto) // CREATE
    {
        var emailHash = HashEmail(dto.Email);
        if (await _userRepo.EmailExistsAsync(emailHash) || await _userRepo.UsernameExistsAsync(dto.Username))
        {
            return null; // User with the same email or username already exists
        }

        // Create new User entity
        var user = new User // Map properties from dto to entity
        {
            Username = dto.Username,
            Email = _encryptionService.Encrypt(dto.Email.Trim()),
            EmailHash = emailHash,
            PasswordHash = _passwordHasher.Hash(dto.Password).Value,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _userRepo.AddAsync(user);
        await _userRepo.SaveChangesAsync();
        return MapToDto(user);
    }

    public async Task<UpdateUserResult> GoogleLoginAsync(GoogleUserInfo googleUser)
    {
        // Check if user exists and return existing user
        var user = await _userRepo.GetByGoogleIdAsync(googleUser.GoogleId);
        if (user is not null)
        {
            return UpdateUserResult.Ok(MapToDto(user));
        }

        // Never link automatically by e-mail: anyone can sign up with someone else's e-mail
        // without verifying it, so linking would let an attacker share the victim's account
        user = await _userRepo.GetUserByEmailAsync(HashEmail(googleUser.Email));
        if (user is not null)
        {
            return UpdateUserResult.Fail(UpdateUserError.EmailInUse);
        }

        // Create new user if not found
        // Google names are not unique, so find a free username (fx "Mark1")
        var baseName = string.IsNullOrWhiteSpace(googleUser.Name) ? "user" : googleUser.Name.Trim();
        var username = baseName;
        var suffix = 1;
        while (await _userRepo.UsernameExistsAsync(username))
            username = $"{baseName}{suffix++}";

        user = new User
        {
            Username = username,
            Email = _encryptionService.Encrypt(googleUser.Email.Trim()),
            EmailHash = HashEmail(googleUser.Email),
            PasswordHash = null, // No password for Google login
            GoogleId = googleUser.GoogleId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _userRepo.AddAsync(user);
        await _userRepo.SaveChangesAsync();
        return UpdateUserResult.Ok(MapToDto(user));
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(int id) // READ
    {
        var user = await _userRepo.GetByIdAsync(id);
        return user is null ? null : MapToDto(user);
    }

    public async Task<LoginResult> GetUserByUsernameAndPasswordAsync(string username, string password) // READ
        => await LoginWithPasswordAsync(await _userRepo.GetUserByUsernameAsync(username), password);

    public async Task<LoginResult> GetUserByEmailAndPasswordAsync(string email, string password) // READ
        => await LoginWithPasswordAsync(await _userRepo.GetUserByEmailAsync(HashEmail(email)), password);

    public async Task<LoginResult> VerifyTwoFactorLoginAsync(int userId, string code) // READ
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user is null || !user.TwoFactorEnabled)
            return new LoginResult(LoginStatus.InvalidCredentials);

        if (user.LockedUntil > DateTime.UtcNow)
            return new LoginResult(LoginStatus.AccountLocked, LockedUntil: user.LockedUntil);

        // Wrong 2fa codes counts the same lockout as wrong passwords
        if (!_twoFactorService.VerifyAndConsumeCode(user, code))
        {
            await RegisterFailedLoginAsync(user);
            return user.LockedUntil > DateTime.UtcNow
                ? new LoginResult(LoginStatus.AccountLocked, LockedUntil: user.LockedUntil)
                : new LoginResult(LoginStatus.InvalidCredentials);
        }

        user.FailedLoginAttempts = 0;
        user.LockedUntil = null;
        _userRepo.Update(user);
        await _userRepo.SaveChangesAsync();
        return new LoginResult(LoginStatus.Success, MapToDto(user));
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
        var newHash = HashEmail(newEmail);

        // Only check for duplicates if it is actually a different e-mail.
        // The hash is made from the lowercased e-mail, so "Mark@mark.dk" and "mark@mark.dk" give the same hash
        var isSameEmail = newHash == user.EmailHash;
        if (!isSameEmail && await _userRepo.EmailExistsAsync(newHash))
            return UpdateUserResult.Fail(UpdateUserError.EmailInUse);

        user.Email = _encryptionService.Encrypt(newEmail);
        user.EmailHash = newHash;
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
    private UserResponseDto MapToDto(User user) 
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = _encryptionService.Decrypt(user.Email),
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            TwoFactorEnabled = user.TwoFactorEnabled
        };
    }

    private string HashEmail(string email) => _encryptionService.Hash(email.Trim().ToLowerInvariant()); // Helper method to hash emails consistently

        // Shared login logic for username and e-mail login
    private async Task<LoginResult> LoginWithPasswordAsync(User? user, string password)
    {
        if (user?.PasswordHash is null)
            return new LoginResult(LoginStatus.InvalidCredentials);

        // Check the lock before the password: a locked account costs no Argon2 and cannot be guessed further
        if (user.LockedUntil > DateTime.UtcNow)
            return new LoginResult(LoginStatus.AccountLocked, LockedUntil: user.LockedUntil);

        if (!_passwordHasher.Verify(password, HashedPassword.FromStoredHash(user.PasswordHash)))
        {
            await RegisterFailedLoginAsync(user);
            return user.LockedUntil > DateTime.UtcNow
                ? new LoginResult(LoginStatus.AccountLocked, LockedUntil: user.LockedUntil)
                : new LoginResult(LoginStatus.InvalidCredentials);
        }

        if (user.TwoFactorEnabled)
            return new LoginResult(LoginStatus.TwoFactorRequired, MapToDto(user));


        await ResetFailedLoginsAsync(user);
        return new LoginResult(LoginStatus.Success, MapToDto(user));
    }

    private async Task RegisterFailedLoginAsync(User user)
    {
        user.FailedLoginAttempts++;
        if (user.FailedLoginAttempts >= MaxFailedLoginAttempts)
        {
            user.LockedUntil = DateTime.UtcNow.Add(LockoutDuration);
            user.FailedLoginAttempts = 0;
            // TODO: send an e-mail to the user when the mail service is ready
        }
        _userRepo.Update(user);
        await _userRepo.SaveChangesAsync();
    }

    private async Task ResetFailedLoginsAsync(User user)
    {
        if (user.FailedLoginAttempts == 0 && user.LockedUntil is null)
            return; // Nothing to reset, so skip the database write

        user.FailedLoginAttempts = 0;
        user.LockedUntil = null;
        _userRepo.Update(user);
        await _userRepo.SaveChangesAsync();
}
}