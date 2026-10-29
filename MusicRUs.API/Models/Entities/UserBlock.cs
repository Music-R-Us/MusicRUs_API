using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicRUs.API.Models.Entities;

public class UserBlock : IHasTimestamps
{
    public int BlockerId { get; set; }
    public User Blocker { get; set; } = null!;

    public int BlockedId { get; set; }
    public User Blocked { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}