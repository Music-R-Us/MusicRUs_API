using System.ComponentModel.DataAnnotations;

namespace MusicRUs.API.DTOs
{
    public class AuthResponseDto // Response DTO for authentication results
    {
        public string AccessToken { get; set; } = null!; 

        public DateTime ExpiresAt { get; set; }

        public UserResponseDto User { get; set; } = null!; // User details associated with the authentication response
     }
}