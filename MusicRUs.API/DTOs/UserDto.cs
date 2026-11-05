using System.ComponentModel.DataAnnotations;

namespace MusicRUs.API.DTOs;

// Represents a Data Transfer Object (DTO) for the User entity.
// In other words, a 'payloads' for specific operations that is sent to the API. 

public class CreateUserDto // DTO for creating a new user
{
    [Required]
    public string Username { get; set; } = null!;

    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(8)]
    [RegularExpression(PasswordRules.Pattern, ErrorMessage = PasswordRules.Message)]
    public string Password { get; set; } = null!;
}

public class UserResponseDto // DTO for returning user information in API responses
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class UserIncludeDto
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
}

public class UsernameLoginDto // DTO for username-based login
{
    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}

public class EmailLoginDto // DTO for email-based login
{
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}

public class UpdateUsernameDto // DTO for updating username
{
    [Required]
    public string NewUsername { get; set; } = null!;
}

public class UpdateEmailDto // DTO for updating email
{
    [Required] 
    [EmailAddress] 
    public string CurrentEmail { get; set; } = null!;
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string NewEmail { get; set; } = null!;

    [Required]
    public string CurrentPassword { get; set; } = null!; // for identity confirmation.
}

public class UpdatePasswordDto // DTO for updating password
{
    [Required]
    [MinLength(8)]
    [RegularExpression(PasswordRules.Pattern, ErrorMessage = PasswordRules.Message)]
    public string NewPassword { get; set; } = null!;

    [Required]
    public string CurrentPassword { get; set; } = null!;
}

public static class PasswordRules
{
    public const string Pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#_])[A-Za-z\d@$!%*?&#_]{8,}$";

    public const string Message = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.";
}