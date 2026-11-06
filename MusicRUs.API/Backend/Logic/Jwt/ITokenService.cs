
using MusicRUs.API.DTOs;
namespace MusicRUs.API.Backend.Logic.Jwt
{
    public interface ITokenService
    {
        AuthResponseDto CreateAuthResponse(UserResponseDto user); // Creates an authentication response for the given user
    }
}