using MusicRUs.API.DTOs;

namespace MusicRUs.API.Services;

// Why a 2FA operation failed, so the controller can return the right status code
public enum TwoFactorError
{
    None,           // It went well
    NotFound,       // No user with that id
    AlreadyEnabled, // 2FA is already turned on
    NotSetUp,       // Setup has not been run, or 2FA is not turned on
    InvalidCode     // Wrong, expired or already used code
}

public record TwoFactorSetupResult(TwoFactorSetupDto? Setup, TwoFactorError Error);