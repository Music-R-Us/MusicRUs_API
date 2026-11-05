using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace MusicRUs.API.Backend.Logic.Hashing;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 4;
    private const int MemorySize = 65536; // 64 MB
    private const int DegreeOfParallelism = 2;

    public HashedPassword Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = ComputeHash(password, salt);
        return HashedPassword.FromHash($"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}");
    }

    public bool Verify(string password, HashedPassword storedHash)
    {
        var parts = storedHash.Value.Split('.');
        if (parts.Length != 2) return false;

        byte[] salt;
        byte[] expectedHash;
        try
        {
            salt = Convert.FromBase64String(parts[0]);
            expectedHash = Convert.FromBase64String(parts[1]);
        }
        catch (FormatException)
        {
            return false;
        }

        var actualHash = ComputeHash(password, salt);
        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }

    private static byte[] ComputeHash(string password, byte[] salt)
    {
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            Iterations = Iterations,
            MemorySize = MemorySize,
            DegreeOfParallelism = DegreeOfParallelism
        };
        return argon2.GetBytes(HashSize);
    }
}
