using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.DTOs;

namespace MusicRUs.API.Controllers;

[ApiController]
//[Authorize]
[Route("api/[controller]")]
public class FriendshipController : ControllerBase
{
    private readonly IFriendshipService _fsService;

    public FriendshipController(IFriendshipService fsService)
    {
        _fsService = fsService;
    }

    // Implement the endpoints for Friendship-related operations

    // CREATE
    //##############################################################################
    // Create Friendship between requestor and addressee
    [HttpPost("CreateFs/{requesterId:int}/{addresseeId:int}")]
    public async Task<ActionResult<FriendshipResponseDto>> CreateFs(int requesterId, int addresseeId)
    {
        var fs = await _fsService.CreateFsAsync(requesterId, addresseeId);
        if (fs == null)
            return BadRequest();

        return Ok(fs);
    }

    // READ
    //##############################################################################
    // Get pending friendship requests for the current user
    [HttpGet("PendingFs-By-AddresseeId/{userId:int}")]
    public async Task<ActionResult<List<FriendshipResponseDto>>> GetPendingFsListByUserId(int userId)
    {
        var fsList = await _fsService.GetPendingFsListByUserIdAsync(userId);
        return Ok(fsList);
    }

    // Get all friendships for the current user
    [HttpGet("FsList-By-UserId/{userId:int}")]
    public async Task<ActionResult<List<FriendshipResponseDto>>> GetFsListByUserId(int userId)
    {
        var fsList = await _fsService.GetFsListByUserIdAsync(userId);
        return Ok(fsList);
    }

    // UPDATE
    //##############################################################################
    // Accept friendship request
    [HttpPatch("AcceptFs/{id:int}/{userId:int}")]
    public async Task<ActionResult<FriendshipResponseDto>> AcceptFs(int id, int userId)
    {
        try
        {
            var fs = await _fsService.AcceptFsAsync(id, userId);
            if (fs == null)
                return NotFound();

            return Ok(fs);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Reject friendship request
    [HttpPatch("RejectFs/{id:int}/{userId:int}")]
    public async Task<ActionResult<FriendshipResponseDto>> RejectFs(int id, int userId)
    {
        try
        {
            var fs = await _fsService.RejectFsAsync(id, userId);
            if (fs == null)
                return NotFound();

            return Ok(fs);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    // DELETE
    //##############################################################################
    // Delete a friendship by ID
    [HttpDelete("DeleteFs/{requesterId:int}/{addresseeId:int}")]
    public async Task<ActionResult> DeleteFs(int requesterId, int addresseeId)
    {
        var success = await _fsService.DeleteFsAsync(requesterId, addresseeId);
        if (!success)
            return NotFound();

        return NoContent();
    }
}