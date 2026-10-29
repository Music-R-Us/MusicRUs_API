namespace MusicRUs.API.Models.Entities;


// Interface for entities that have CreatedAt and UpdatedAt timestamps.

public interface IHasTimestamps
{
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}