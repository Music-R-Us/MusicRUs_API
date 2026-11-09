using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace MusicRUs.API.Backend.Logic.Encryption;

// Implements the encryption service using AES-GCM for encryption and HMAC-SHA256 for hashing.
public class EncryptionService : IEncryptionService
{
    private const int NonceSize = 12;
    private const int TagSize = 16;
    private readonly byte[] _key;
    private readonly byte[] _hmacKey;

    public EncryptionService(IOptions<EncryptionOptions> options)
    {
        _key = Convert.FromBase64String(options.Value.Key);
        _hmacKey = Convert.FromBase64String(options.Value.HmacKey);
    }

    // Encrypts the given plaintext using AES-GCM and returns the encrypted string in the format "nonce.cipher.tag".
    public string Encrypt(string plaintext)
    {
        var plainBytes = Encoding.UTF8.GetBytes(plaintext);
        var nonce = RandomNumberGenerator.GetBytes(NonceSize);
        var cipher = new byte[plainBytes.Length];
        var tag = new byte[TagSize];

        using var aes = new AesGcm(_key, TagSize);
        aes.Encrypt(nonce, plainBytes, cipher, tag);

        // Stored as "nonce.cipher.tag" in base64, same idea as the password hash format
        return $"{Convert.ToBase64String(nonce)}.{Convert.ToBase64String(cipher)}.{Convert.ToBase64String(tag)}";
    }

    // Decrypts the given encrypted string in the format "nonce.cipher.tag" using AES-GCM and returns the plaintext.
    public string Decrypt(string encrypted)
    {
        var parts = encrypted.Split('.');
        if (parts.Length != 3)
            throw new FormatException("Encrypted value has an invalid format.");

        var nonce = Convert.FromBase64String(parts[0]);
        var cipher = Convert.FromBase64String(parts[1]);
        var tag = Convert.FromBase64String(parts[2]);
        var plainBytes = new byte[cipher.Length];

        using var aes = new AesGcm(_key, TagSize);
        aes.Decrypt(nonce, cipher, tag, plainBytes); // Throws if the value has been tampered with
        return Encoding.UTF8.GetString(plainBytes);
    }

    // Computes the HMAC-SHA256 hash of the given value using the HMAC key and returns it as a hexadecimal string.
    public string Hash(string value)
    {
        var hash = HMACSHA256.HashData(_hmacKey, Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(hash);
    }
}