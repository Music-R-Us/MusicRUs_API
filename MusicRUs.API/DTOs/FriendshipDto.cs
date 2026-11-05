using MusicRUs.API.DTOs;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.DTOs;

public class CreateFriendshipDto
{
    public int RequesterId { get; set; }
    public int AddresseeId { get; set; }
}

public class FriendshipResponseDto
{
    public int Id { get; set; }
    public int RequesterId { get; set; }
    public int AddresseeId { get; set; }
    public FriendshipStatus Status { get; set; }
    public UserIncludeDto Requester { get; set; } = null!;
    public UserIncludeDto Addressee { get; set; } = null!;
}