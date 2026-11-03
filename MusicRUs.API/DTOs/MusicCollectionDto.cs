using System.ComponentModel.DataAnnotations;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.DTOs;

// Represents Data Transfer Objects (DTOs) for the MusicCollection entity.
// In other words, 'payloads' for specific operations that is sent to the API.

public class CreateOwnedMusicCollectionDto // DTO for adding an album to a user's collection
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int AlbumId { get; set; }

    [Required] 
    [EnumDataType(typeof(MediaFormat))] // EnumDataType ensures the value is valid (in this case, a valid MediaFormat)
    public MediaFormat Format { get; set; }

    [EnumDataType(typeof(MediaCondition))]
    public MediaCondition? Condition { get; set; } 

    //[EnumDataType(typeof(CollectionStatus))]
    //public CollectionStatus Status { get; set; } = CollectionStatus.Owned;
}

public class CreateWantedMusicCollectionDto // DTO for adding an album to a user's wanted collection
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int AlbumId { get; set; }

    [Required] 
    [EnumDataType(typeof(MediaFormat))] // EnumDataType ensures the value is valid (in this case, a valid MediaFormat)
    public MediaFormat Format { get; set; }

    [EnumDataType(typeof(MediaCondition))]
    public MediaCondition? Condition { get; set; } 

    //[EnumDataType(typeof(CollectionStatus))]
    //public CollectionStatus Status { get; set; } = CollectionStatus.Wanted;
}

public class MusicCollectionResponseDto // DTO for returning collection item data in API responses
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int AlbumId { get; set; }
    public MediaFormat Format { get; set; }
    public MediaCondition? Condition { get; set; }
    public CollectionStatus Status { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public AlbumResponseDto? Album { get; set; }
}

public class UpdateMusicCollectionDto // DTO for updating an existing collection item
{
    [EnumDataType(typeof(MediaFormat))]
    public MediaFormat? Format { get; set; }
    [EnumDataType(typeof(MediaCondition))]
    public MediaCondition? Condition { get; set; } 
    [EnumDataType(typeof(CollectionStatus))]
    public CollectionStatus? Status { get; set; }
}