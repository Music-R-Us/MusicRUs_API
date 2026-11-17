using MusicRUs.API.Backend.Logic.Encryption;
using MusicRUs.API.Backend.Logic.TwoFactor;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Services;
using MusicRUs.API.Tests.Fakes;
using MusicRUs.API.Tests.Helpers;
using Xunit;

namespace MusicRUs.API.Tests.Security;

// Real TwoFactorService, TotpService and EncryptionService. Only the database is replaced by a fake.
public class TwoFactorServiceTests
{
    private readonly FakeUserRepository _repo = new();
    private readonly EncryptionService _encryption = TestFactory.CreateEncryptionService();
    private readonly TotpService _totp = new();
    private readonly TwoFactorService _service;

    public TwoFactorServiceTests() => _service = new TwoFactorService(_repo, _encryption, _totp);

    // A user who has 2FA turned on, with the secret stored encrypted like in the database
    private User SeedUserWithTwoFactor(out string secret)
    {
        secret = _totp.GenerateSecret();
        var user = new User
        {
            Username = "mark",
            Email = _encryption.Encrypt("mark@example.dk"),
            EmailHash = _encryption.Hash("mark@example.dk"),
            TwoFactorEnabled = true,
            TotpSecret = _encryption.Encrypt(secret)
        };
        _repo.Seed(user);
        return user;
    }

    [Fact]
    public void Same_code_cannot_be_used_twice()
    {
        var user = SeedUserWithTwoFactor(out var secret);
        var code = TestFactory.TotpCode(secret);

        Assert.True(_service.VerifyAndConsumeCode(user, code));
        Assert.False(_service.VerifyAndConsumeCode(user, code)); // Replay inside the same 30-second window
    }

    [Fact]
    public async Task Disable_with_a_wrong_code_keeps_2FA_on()
    {
        var user = SeedUserWithTwoFactor(out var secret);
        var wrongCode = TestFactory.WrongCode(TestFactory.TotpCode(secret));

        var error = await _service.DisableAsync(user.Id, wrongCode);

        // Someone with a stolen token cannot turn 2FA off without the authenticator app
        Assert.Equal(TwoFactorError.InvalidCode, error);
        Assert.True(user.TwoFactorEnabled);
        Assert.NotNull(user.TotpSecret);
    }
}
