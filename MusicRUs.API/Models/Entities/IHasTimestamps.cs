namespace MusicRUs.API.Models.Entities;


/// Interface for entities that have CreatedAt and UpdatedAt timestamps. 
public interface IHasTimestamps
{
    DateTime CreatedAt { get; set; } // Timestamp for when the entity was created
    DateTime UpdatedAt { get; set; } // Timestamp for when the entity was last updated
}