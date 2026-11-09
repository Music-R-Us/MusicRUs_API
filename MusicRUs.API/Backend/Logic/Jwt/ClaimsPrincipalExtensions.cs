
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
namespace MusicRUs.API.Backend.Logic.Jwt
{

    // Extension methods for ClaimsPrincipal to extract JWT claims.
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetUserId(this ClaimsPrincipal user) // Retrieves the user ID from the JWT claims.
        {
            var sub = user.FindFirstValue(JwtRegisteredClaimNames.Sub);
            return int.TryParse(sub, out var id) ? id : null;
        }
    }
}