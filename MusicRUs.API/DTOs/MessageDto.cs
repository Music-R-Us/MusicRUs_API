using System.ComponentModel.DataAnnotations;

namespace MusicRUs.API.DTOs;

// Represents a Data Transfer Object (DTO) for the Message entity.
// In other words, a 'payloads' for specific operations that is sent to the API. 

public class CreateMessageDto //DTO for creating a new message
{
    //[Required]
    //public int SenderId { get; set; }

    //[Required]
    //public int RecipientId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Content { get; set; } = string.Empty;
}

public class MessageResponseDto //DTO for returning message details
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public UserIncludeDto Sender { get; set; } = null!;
    public int RecipientId { get; set; }
    public UserIncludeDto Recipient { get; set; } = null!;
    public string Content { get; set; } = string.Empty;
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class MessageUpdateDto //DTO for updating an existing message
{
    public DateTime? ReadAt { get; set; }
}