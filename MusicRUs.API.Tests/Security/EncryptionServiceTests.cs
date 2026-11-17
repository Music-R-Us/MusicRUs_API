using System.Security.Cryptography;
using MusicRUs.API.Backend.Logic.Encryption;
using MusicRUs.API.Tests.Helpers;
using Xunit;

namespace MusicRUs.API.Tests.Security;

public class EncryptionServiceTests
{
    private const string Email = "mark@example.dk";
    private readonly EncryptionService _encryption = TestFactory.CreateEncryptionService();

    [Theory]
    [InlineData("mark@example.dk")]
    [InlineData("æøå.ÆØÅ@musicrus.dk")] // UTF-8 and a dot, like the "nonce.cipher.tag" separator
    public void Decrypt_returns_the_original_text(string plaintext)
    {
        var encrypted = _encryption.Encrypt(plaintext);

        Assert.Equal(plaintext, _encryption.Decrypt(encrypted));
    }

    [Fact]
    public void Same_email_is_encrypted_differently_every_time()
    {
        var first = _encryption.Encrypt(Email);
        var second = _encryption.Encrypt(Email);

        Assert.NotEqual(first, second);      // New random nonce every time
        Assert.DoesNotContain(Email, first); // Never stored in plain text
    }

    [Fact]
    public void Decrypt_throws_when_the_ciphertext_has_been_changed()
    {
        var parts = _encryption.Encrypt(Email).Split('.');
        var cipher = Convert.FromBase64String(parts[1]);
        cipher[0] ^= 0x01; // Flip one bit
        var tampered = $"{parts[0]}.{Convert.ToBase64String(cipher)}.{parts[2]}";

        // The GCM tag no longer matches, so AES-GCM refuses to decrypt
        Assert.ThrowsAny<CryptographicException>(() => _encryption.Decrypt(tampered));
    }

    [Fact]
    public void Hash_is_the_same_for_the_same_email_so_it_can_be_looked_up()
    {
        var hash = _encryption.Hash(Email);

        Assert.Equal(hash, _encryption.Hash(Email));
        Assert.NotEqual(hash, _encryption.Hash("nick@example.dk"));
        Assert.Matches("^[0-9A-F]{64}$", hash); // HMAC-SHA256 = 32 bytes = 64 hex characters
    }
}
