using MusicRUs.API.Backend.Logic.Hashing;
using MusicRUs.API.Backend.Logic.TwoFactor;
using MusicRUs.API.DTOs;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Services;
using MusicRUs.API.Tests.Fakes;
using MusicRUs.API.Tests.Helpers;
using Xunit;

namespace MusicRUs.API.Tests.Security;

// Real UserService with real Argon2id and AES-GCM. Only the database is replaced by a fake.
public class LoginAndLockoutTests
{
    private const string Password = "Correct1!";
    private const string WrongPassword = "Wrong123!";

    private readonly FakeUserRepository _repo = new();
    private readonly UserService _service;

    public LoginAndLockoutTests()
    {
        var encryption = TestFactory.CreateEncryptionService();
        var twoFactor = new TwoFactorService(_repo, encryption, new TotpService());
        _service = new UserService(_repo, new PasswordHasher(), encryption, twoFactor);
    }

    // Signs up "mark" through the real sign-up code, so the password is hashed and the e-mail encrypted
    private async Task<User> CreateMarkAsync()
    {
        await _service.CreateUserAsync(new CreateUserDto { Username = "mark", Email = "mark@example.dk", Password = Password });
        return (await _repo.GetUserByUsernameAsync("mark"))!;
    }

    private Task<LoginResult> LoginAsync(string password)
        => _service.GetUserByUsernameAndPasswordAsync("mark", password);

    [Fact]
    public async Task Unknown_user_gets_the_same_answer_as_a_wrong_password()
    {
        await CreateMarkAsync();

        var unknownUser = await _service.GetUserByUsernameAndPasswordAsync("nobody", Password);
        var wrongPassword = await LoginAsync(WrongPassword);

        // Same answer in both cases, so it cannot be used to find out which usernames exist
        Assert.Equal(LoginStatus.InvalidCredentials, unknownUser.Status);
        Assert.Equal(LoginStatus.InvalidCredentials, wrongPassword.Status);
    }

    [Fact]
    public async Task Three_wrong_passwords_lock_the_account_for_15_minutes()
    {
        await CreateMarkAsync();
        var before = DateTime.UtcNow;

        var first = await LoginAsync(WrongPassword);
        var second = await LoginAsync(WrongPassword);
        var third = await LoginAsync(WrongPassword);

        Assert.Equal(LoginStatus.InvalidCredentials, first.Status);
        Assert.Equal(LoginStatus.InvalidCredentials, second.Status);
        Assert.Equal(LoginStatus.AccountLocked, third.Status);
        Assert.InRange(third.LockedUntil.GetValueOrDefault(), before.AddMinutes(14), before.AddMinutes(16));

        // While locked, even the correct password is refused
        Assert.Equal(LoginStatus.AccountLocked, (await LoginAsync(Password)).Status);
    }

    [Fact]
    public async Task Password_login_with_2FA_asks_for_a_code_and_does_not_reset_attempts()
    {
        var user = await CreateMarkAsync();
        user.TwoFactorEnabled = true;
        user.FailedLoginAttempts = 2;

        var result = await LoginAsync(Password);

        Assert.Equal(LoginStatus.TwoFactorRequired, result.Status);
        // If the counter was reset here, someone who knows the password could guess codes forever
        Assert.Equal(2, user.FailedLoginAttempts);
    }
}
