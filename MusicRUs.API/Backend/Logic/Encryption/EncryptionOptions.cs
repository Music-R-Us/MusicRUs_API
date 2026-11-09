namespace MusicRUs.API.Backend.Logic.Encryption
{

    // Represents the encryption options for the application.
    public class EncryptionOptions
    {
        public const string SectionName = "Encryption";
        public string Key { get; set; } = null!;
        public string HmacKey { get; set; } = null!;

        public static int KeyLength(string? base64) // Returns the length of the key in bytes after base64 decoding
        {
            var buffer = new byte[128];
            return Convert.TryFromBase64String(base64 ?? "", buffer, out var written) ? written : 0;
        }
    }
}