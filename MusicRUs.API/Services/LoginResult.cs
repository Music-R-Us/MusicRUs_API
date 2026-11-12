using MusicRUs.API.DTOs;

namespace MusicRUs.API.Services;

public enum LoginStatus
{
    Success,
    InvalidCredentials,
    AccountLocked,
    TwoFactorRequired,
    Error
}

public record LoginResult(LoginStatus Status, UserResponseDto? User = null, DateTime? LockedUntil = null);