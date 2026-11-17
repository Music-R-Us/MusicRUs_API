using System.Text;
using System.Text.Json.Nodes;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MusicRUs.API.Backend.Logic.Jwt;
using MusicRUs.API.DTOs;
using MusicRUs.API.Tests.Helpers;
using Xunit;

namespace MusicRUs.API.Tests.Security;

public class TokenServiceTests
{
    private static readonly UserResponseDto Mark = new() { Id = 1, Username = "mark", Email = "mark@example.dk" };

    private readonly JwtOptions _options = TestFactory.CreateJwtOptions();
    private readonly JsonWebTokenHandler _handler = new();
    private readonly TokenService _tokens;

    public TokenServiceTests() => _tokens = TestFactory.CreateTokenService(_options);

    // The same rules as AddJwtBearer in Program.cs
    private TokenValidationParameters ApiValidationParameters() => new()
    {
        ValidateIssuer = true,
        ValidIssuer = _options.Issuer,
        ValidateAudience = true,
        ValidAudience = _options.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(30)
    };

    [Fact]
    public async Task AccessToken_with_a_changed_user_id_is_rejected()
    {
        var token = _tokens.CreateAuthResponse(Mark).AccessToken;
        var parts = token.Split('.');
        var payload = JsonNode.Parse(Base64UrlEncoder.Decode(parts[1]))!;
        payload["sub"] = "2"; // An attacker tries to become user 2 but keeps the old signature
        var forged = $"{parts[0]}.{Base64UrlEncoder.Encode(payload.ToJsonString())}.{parts[2]}";

        var original = await _handler.ValidateTokenAsync(token, ApiValidationParameters());
        var result = await _handler.ValidateTokenAsync(forged, ApiValidationParameters());

        Assert.True(original.IsValid); // Control: the real token is accepted
        Assert.False(result.IsValid);  // The signature no longer matches the payload
    }

    [Fact]
    public async Task TwoFactorToken_cannot_be_used_to_call_the_API()
    {
        var challenge = _tokens.CreateTwoFactorChallenge(1);

        var asTwoFactorToken = await _tokens.ValidateTwoFactorTokenAsync(challenge.TwoFactorToken);
        var asAccessToken = await _handler.ValidateTokenAsync(challenge.TwoFactorToken, ApiValidationParameters());

        Assert.Equal(1, asTwoFactorToken);  // Valid for the 2FA step...
        Assert.False(asAccessToken.IsValid); // ...but not for the API, because the audience is "MusicRUs.App.2fa"
    }
}
