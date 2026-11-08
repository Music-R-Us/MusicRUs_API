using System.ComponentModel.DataAnnotations;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.DTOs;

public class UserBlockResponseDto
{
    [Required]
    public int BlockerId { get; set; }

    [Required]
    public int BlockedId { get; set; }
    [Required]
    public UserIncludeDto Blocked { get; set; } = null!;
}