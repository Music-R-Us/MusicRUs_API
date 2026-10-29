using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MusicRUs.API.DTOs;

// Represents a Data Transfer Object (DTO) for the ForumPost entity.
// In other words, a 'payload' for specific operations that is sent to the API. 
public class GenreResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}

public class GenreIncludeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}