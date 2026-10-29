using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicRUs.API.Models.Entities;

public class Message : IHasTimestamps
{
    public int Id { get; set; }

    public int SenderId { get; set; }
    public User Sender { get; set; } = null!;

    public int RecipientId { get; set; }
    public User Recipient { get; set; } = null!;

    public string Content { get; set; } = string.Empty;
    public DateTime? ReadAt { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}