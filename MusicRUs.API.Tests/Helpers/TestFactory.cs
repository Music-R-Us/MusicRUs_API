using System.Security.Cryptography;
using MusicRUs.API.Backend.Logic.Encryption;
using MusicRUs.API.Backend.Logic.Jwt;
using OtpNet;

namespace MusicRUs.API.Tests.Helpers;

// Builds the real services with test settings. Keys are random for every test,
// so no keys are written in the code or committed to the repository.
internal static class TestFactory
{
    public static EncryptionService CreateEncryptionService()
        => new(Microsoft.Extensions.Options.Options.Create(new EncryptionOptions
        {
            Key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),     // AES-256
            HmacKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
        }));

    // Same issuer, audience and lifetime as appsettings.json
    public static JwtOptions CreateJwtOptions() => new()
    {
        Issuer = "MusicRUs.API",
        Audience = "MusicRUs.App",
        SigningKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48)),
        ExpiresMinutes = 60
    };

    public static TokenService CreateTokenService(JwtOptions options)
        => new(Microsoft.Extensions.Options.Options.Create(options));

    // The code an authenticator app would show right now (or at another time)
    public static string TotpCode(string secret, DateTime? utc = null)
        => new Totp(Base32Encoding.ToBytes(secret)).ComputeTotp(utc ?? DateTime.UtcNow);

    // A 6-digit code that is not the correct one
    public static string WrongCode(string correctCode)
        => ((int.Parse(correctCode) + 1) % 1_000_000).ToString("D6");
}
