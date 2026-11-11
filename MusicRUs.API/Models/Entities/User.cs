using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicRUs.API.Models.Entities;
/// Represents a user in the system. Implements IHasTimestamps for automatic timestamp management. 
/// Contains properties for username, email, password hash, Google ID, and timestamps.
public class User : IHasTimestamps
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!; // Encrypted with AES-GCM ("nonce.cipher.tag")
    public string EmailHash {get; set;} = null!; // HMAC-SHA256 of the lowercased email, used for lookup
    public string? PasswordHash { get; set; }   // Empty for Google users
    public string? GoogleId { get; set; }
    public int FailedLoginAttempts { get; set; } // Reset on successful login
    public DateTime? LockedUntil { get; set; } // Null when the account is not locked
    public bool TwoFactorEnabled { get; set; }
    public string? TotpSecret { get; set; } // Encrypted with AES-GCM, null when 2FA is not set up
    public long? TotpLastUsedStep { get; set; } // Prevents the same 2FA code from being used twice
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ForumProfile? Profile { get; set; }
}