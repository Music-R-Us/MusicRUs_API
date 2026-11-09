namespace MusicRUs.API.Backend.Logic.Encryption;

// Defines the interface for the encryption service.

public interface IEncryptionService
{
    string Encrypt(string plaintext);

    string Decrypt(string encrypted);

    string Hash(string value);
}