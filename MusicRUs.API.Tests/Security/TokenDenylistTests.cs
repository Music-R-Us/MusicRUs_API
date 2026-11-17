using Microsoft.Extensions.Caching.Memory;
using MusicRUs.API.Backend.Logic.Jwt;
using Xunit;

namespace MusicRUs.API.Tests.Security;

// Uses a real MemoryCache, the same as the API
public class TokenDenylistTests
{
    private readonly TokenDenylist _denylist = new(new MemoryCache(new MemoryCacheOptions()));

    [Fact]
    public void Logged_out_token_is_on_the_denylist_and_other_tokens_are_not()
    {
        _denylist.Revoke("jti-1", DateTimeOffset.UtcNow.AddMinutes(60));

        Assert.True(_denylist.IsRevoked("jti-1"));
        Assert.False(_denylist.IsRevoked("jti-2"));
    }

    [Fact]
    public void Entry_is_removed_when_the_token_would_have_expired_anyway()
    {
        _denylist.Revoke("jti-1", DateTimeOffset.UtcNow.AddMinutes(-1));

        Assert.False(_denylist.IsRevoked("jti-1")); // So the list never grows forever
    }
}
