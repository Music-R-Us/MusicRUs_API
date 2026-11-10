using Microsoft.Extensions.Caching.Memory;

namespace MusicRUs.API.Backend.Logic.Jwt;

public class TokenDenylist : ITokenDenylist
{
    private readonly IMemoryCache _cache;

    public TokenDenylist(IMemoryCache cache) => _cache = cache;

    // The entry disappears when the token would have expired anyway, so the list never grows forever
    public void Revoke(string jti, DateTimeOffset expiresAt)
        => _cache.Set($"revoked-jti:{jti}", true, expiresAt);

    public bool IsRevoked(string jti)
        => _cache.TryGetValue($"revoked-jti:{jti}", out _);
}