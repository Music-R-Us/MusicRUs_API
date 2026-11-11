using Google.Apis.Auth;
using Microsoft.Extensions.Options;

namespace MusicRUs.API.Backend.Logic.GoogleAuth;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly GoogleAuthOptions _options;
    private readonly ILogger<GoogleAuthService> _logger;

    public GoogleAuthService(IOptions<GoogleAuthOptions> options, ILogger<GoogleAuthService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<GoogleUserInfo?> VerifyIdTokenAsync(string idToken)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = [_options.ClientId]
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            if (!payload.EmailVerified)          // Only trust the e-mail if Google has verified that the user owns it
            {
                _logger.LogWarning("Google login rejected because the e-mail is not verified.");
                return null;
            }

            return new GoogleUserInfo
            {
                GoogleId = payload.Subject,
                Email = payload.Email,
                Name = payload.Name ?? payload.GivenName
            };
        }
        catch (InvalidJwtException ex)
        {
            _logger.LogWarning(ex, "Google ID token validation failed.");
            return null;
        }
    }
}