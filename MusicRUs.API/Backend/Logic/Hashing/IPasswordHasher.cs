namespace MusicRUs.API.Backend.Logic.Hashing;

public interface IPasswordHasher
{
    HashedPassword Hash(string password);
    bool Verify(string password, HashedPassword storedHash);
}
