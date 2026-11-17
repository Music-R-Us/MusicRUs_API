using MusicRUs.API.Backend.Logic.TwoFactor;
using MusicRUs.API.Tests.Helpers;
using Xunit;

namespace MusicRUs.API.Tests.Security;

public class TotpServiceTests
{
    private readonly TotpService _totp = new();

    [Fact]
    public void Current_code_is_accepted()
    {
        var secret = _totp.GenerateSecret();

        Assert.True(_totp.VerifyCode(secret, TestFactory.TotpCode(secret), out var timeStep));

        var currentStep = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 30;
        Assert.InRange(timeStep, currentStep - 1, currentStep + 1); // Used for replay protection
    }

    [Fact]
    public void Code_from_5_minutes_ago_is_rejected()
    {
        var secret = _totp.GenerateSecret();
        var oldCode = TestFactory.TotpCode(secret, DateTime.UtcNow.AddMinutes(-5));

        Assert.False(_totp.VerifyCode(secret, oldCode, out _));
    }
}
