using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MusicRUs.API.DTOs;

namespace MusicRUs.API.Backend.Logic.Jwt;

// Service responsible for creating JWT authentication tokens.
public class TokenService : ITokenService
{
    private readonly JwtOptions _options;
    private readonly SigningCredentials _credentials;

    public TokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey)); // Create a symmetric security key from the signing key
        _credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); //HMAC SHA-256 signing credentials for the JWT
    }

    public AuthResponseDto CreateAuthResponse(UserResponseDto user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_options.ExpiresMinutes);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Expires = expiresAt,
            SigningCredentials = _credentials,
            Subject = new ClaimsIdentity(new[] // Create a claims identity for the user
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            })
        };

        return new AuthResponseDto // Return the authentication response DTO with the generated token and user details
        {
            AccessToken = new JsonWebTokenHandler().CreateToken(descriptor),
            ExpiresAt = expiresAt,
            User = user
        };
    }
}