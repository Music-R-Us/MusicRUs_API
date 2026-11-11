namespace MusicRUs.API.Backend.Logic.TwoFactor;

public interface ITotpService
{
    string GenerateSecret();
    string GetQrCodeDataUri(string secret, string username);
    bool VerifyCode(string secret, string code, out long timeStep);
}