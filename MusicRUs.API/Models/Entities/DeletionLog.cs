using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicRUs.API.Models.Entities;

public class DeletionLog
{
     public int Id { get; set; }
     public string TableName { get; set; } = null!;
     public string EntityId { get; set; } = null!;
     public int? DeletedByUserId { get; set; } // Deliberately not a FK to avoid cascade issues
     public DateTime DeletedAt { get; set; }
}