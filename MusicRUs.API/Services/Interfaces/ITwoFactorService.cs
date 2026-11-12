using MusicRUs.API.Models.Entities;
using MusicRUs.API.Services;

namespace MusicRUs.API.Services.Interfaces;

public interface ITwoFactorService
{
    Task<TwoFactorSetupResult> SetupAsync(int userId);       // Create a secret and return the QR code
    Task<TwoFactorError> EnableAsync(int userId, string code);  // Turn on 2FA when the first code is correct
    Task<TwoFactorError> DisableAsync(int userId, string code); // Turn off 2FA with a valid code
    bool VerifyAndConsumeCode(User user, string code);       // Used by login in step E
}