using System.ComponentModel.DataAnnotations;

namespace MusicRUs.API.DTOs;

public class GoogleLoginDto
{
    [Required]
    public string IdToken { get; set; } = null!;
}
