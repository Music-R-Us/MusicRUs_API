using System.ComponentModel.DataAnnotations;


namespace MusicRUs.API.DTOs;


// Represents the data needed to set up two-factor authentication in an authenticator app
public class TwoFactorSetupDto
{
    public string Secret { get; set; } = null!;        // For manual entry in the authenticator app
    public string QrCodeDataUri { get; set; } = null!; // Show with <img src="...">
}

// Represents the data needed to verify a two-factor authentication code
public class TwoFactorCodeDto
{
    [Required]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "The code must be 6 digits.")]
    public string Code { get; set; } = null!;
}

// Returned by login when 2FA is enabled: the client must send a code to /api/2fa/verify
public class TwoFactorChallengeDto
{
    public bool RequiresTwoFactor { get; set; } = true;
    public string TwoFactorToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}

public class TwoFactorLoginDto
{
    [Required]
    public string TwoFactorToken { get; set; } = null!;

    [Required]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "The code must be 6 digits.")]
    public string Code { get; set; } = null!;
}