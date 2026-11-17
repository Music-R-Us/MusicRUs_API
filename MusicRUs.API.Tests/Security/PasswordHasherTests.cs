using MusicRUs.API.Backend.Logic.Hashing;
using Xunit;

namespace MusicRUs.API.Tests.Security;

// Argon2id is slow on purpose (64 MB, 4 iterations), so these tests take a few seconds
public class PasswordHasherTests
{
    private const string Password = "Correct1!";
    private readonly PasswordHasher _hasher = new();

    [Fact]
    public void Verify_returns_true_for_the_correct_password()
    {
        var hash = _hasher.Hash(Password);

        Assert.True(_hasher.Verify(Password, hash));
    }

    [Theory]
    [InlineData("Wrong123!")]
    [InlineData("correct1!")]   // Different casing
    [InlineData("Correct1! ")]  // Extra space
    public void Verify_returns_false_for_a_wrong_password(string attempt)
    {
        var hash = _hasher.Hash(Password);

        Assert.False(_hasher.Verify(attempt, hash));
    }

    [Fact]
    public void Same_password_gets_a_new_salt_and_hash_every_time()
    {
        var first = _hasher.Hash(Password);
        var second = _hasher.Hash(Password);

        Assert.NotEqual(first.Value, second.Value); // Two users with the same password get different hashes
        Assert.True(_hasher.Verify(Password, first));
        Assert.True(_hasher.Verify(Password, second));
    }
}
