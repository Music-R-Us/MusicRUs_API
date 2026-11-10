
using System;

namespace MusicRUs.API.Backend.Logic.Jwt;
public interface ITokenDenylist
{
    void Revoke(string jti, DateTimeOffset expiresAt);
    bool IsRevoked(string jti);
}