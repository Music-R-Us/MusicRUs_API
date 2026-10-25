using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicRUs.API.Models.Entities;
/// Represents a user in the system. Implements IHasTimestamps for automatic timestamp management. 
/// Contains properties for username, email, password hash, Google ID, and timestamps.
public class User : IHasTimestamps
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PasswordHash { get; set; }   // Empty for Google users
    public string? GoogleId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}