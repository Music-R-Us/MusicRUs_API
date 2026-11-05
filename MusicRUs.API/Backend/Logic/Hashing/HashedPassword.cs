namespace MusicRUs.API.Backend.Logic.Hashing;

public sealed class HashedPassword
{
    public string Value { get; }

    private HashedPassword(string value) => Value = value;

    // Only called by IPasswordHasher implementations after hashing
    internal static HashedPassword FromHash(string hash) => new(hash);

    // Called in the service layer when the hashed value is loaded from the database
    internal static HashedPassword FromStoredHash(string storedHash) => new(storedHash);
}
