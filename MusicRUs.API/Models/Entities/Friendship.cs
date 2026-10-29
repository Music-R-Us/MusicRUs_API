using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicRUs.API.Models.Entities;

public class Friendship : IHasTimestamps
{
    public int Id { get; set; }

    public int RequesterId { get; set; }
    public User Requester { get; set; } = null!;

    public int AddresseeId { get; set; }
    public User Addressee { get; set; } = null!;

    public FriendshipStatus Status { get; set; } = FriendshipStatus.Pending;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}