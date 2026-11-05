using MusicRUs.API.DTOs;

namespace MusicRUs.API.Services;

// Why an update failed, so the controller can return the right status code
public enum UpdateUserError
{
    None,           // It went well
    NotFound,       // No user with that id
    WrongPassword,  // CurrentPassword did not match (or the user has no password)
    EmailInUse      // Another user already has the new e-mail
}

public record UpdateUserResult(UserResponseDto? User, UpdateUserError Error)
{
    public static UpdateUserResult Ok(UserResponseDto user) => new(user, UpdateUserError.None);
    public static UpdateUserResult Fail(UpdateUserError error) => new(null, error);
}