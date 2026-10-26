namespace MusicRUs.API.Backend.Logic.GoogleAuth;

public interface IGoogleAuthService
{
    Task<GoogleUserInfo?> VerifyIdTokenAsync(string idToken);
}
