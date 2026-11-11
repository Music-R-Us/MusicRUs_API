using OtpNet;
using QRCoder;

namespace MusicRUs.API.Backend.Logic.TwoFactor;

public class TotpService : ITotpService
{
    private const string Issuer = "MusicRUs";

    // 20 random bytes (160 bits) as recommended by RFC 4226, Base32-encoded for authenticator apps
    public string GenerateSecret()
        => Base32Encoding.ToString(KeyGeneration.GenerateRandomKey(20));

    // QR code as a PNG data URI, so the frontend can show it with <img src="...">
    public string GetQrCodeDataUri(string secret, string username)
    {
        var label = Uri.EscapeDataString($"{Issuer}:{username}");
        var uri = $"otpauth://totp/{label}?secret={secret}&issuer={Issuer}&digits=6&period=30";

        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(data);
        return $"data:image/png;base64,{Convert.ToBase64String(qrCode.GetGraphic(5))}";
    }

    // Accepts the current code and the ones just before and after, in case the clocks differ slightly
    public bool VerifyCode(string secret, string code, out long timeStep)
    {
        var totp = new Totp(Base32Encoding.ToBytes(secret));
        return totp.VerifyTotp(code.Trim(), out timeStep, VerificationWindow.RfcSpecifiedNetworkDelay);
    }
}