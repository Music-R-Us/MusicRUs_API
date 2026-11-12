using MusicRUs.API.DTOs;
namespace MusicRUs.API.Backend.Logic.Jwt
{
    public interface ITokenService
    {
        AuthResponseDto CreateAuthResponse(UserResponseDto user);      // Creates an authentication response for the given user
        TwoFactorChallengeDto CreateTwoFactorChallenge(int userId);    // Short-lived token for the 2FA step of login
        Task<int?> ValidateTwoFactorTokenAsync(string token);          // Returns the user id if the 2FA token is valid
    }
}