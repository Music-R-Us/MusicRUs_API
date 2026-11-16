using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.Backend.Logic.Jwt;
using MusicRUs.API.DTOs;

namespace MusicRUs.API.Controllers;

[ApiController]
[Authorize]
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
    [HttpPost("CreateFs/Addressee/{addresseeId:int}")]
    public async Task<ActionResult<FriendshipResponseDto>> CreateFs(int addresseeId)
    {
        try
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();
            
            var fs = await _fsService.CreateFsAsync(userId.Value, addresseeId);
            return Ok(fs);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    // READ
    //##############################################################################
    // Get pending friendship requests for the current user
    [HttpGet("PendingFs-By-AddresseeId")]
    public async Task<ActionResult<List<FriendshipResponseDto>>> GetPendingFsListByUserId()
    {
        var userId = User.GetUserId();
        if (userId == null)
            return Unauthorized();

        var fsList = await _fsService.GetPendingFsListByUserIdAsync(userId.Value);
        return Ok(fsList);
    }

    // Get all friendships for the current user
    [HttpGet("FsList-By-RequesterId")]
    public async Task<ActionResult<List<FriendshipResponseDto>>> GetFsListByUserId()
    {
        var userId = User.GetUserId();
        if (userId == null)
            return Unauthorized();

        var fsList = await _fsService.GetFsListByUserIdAsync(userId.Value);
        return Ok(fsList);
    }

    // UPDATE
    //##############################################################################
    // Accept friendship request
    [HttpPatch("AcceptFs/{id:int}/me")]
    public async Task<ActionResult<FriendshipResponseDto>> AcceptFs(int id)
    {
        try
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var fs = await _fsService.AcceptFsAsync(id, userId.Value);
            return Ok(fs);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    // Reject friendship request
    [HttpPatch("RejectFs/{id:int}/me")]
    public async Task<ActionResult<FriendshipResponseDto>> RejectFs(int id)
    {
        try
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var fs = await _fsService.RejectFsAsync(id, userId.Value);
            return Ok(fs);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }


    // DELETE
    //##############################################################################
    // Delete a friendship by ID
    [HttpDelete("DeleteFs/{id:int}")]
    public async Task<ActionResult> DeleteFs(int id)
    {
        try
        {
            var userId = User.GetUserId();
            if(userId == null)
                return Unauthorized();
            
            var fs = await _fsService.GetFsByIdAsync(id);

            // Only the 2 users in the friendship may delete it
            if (fs.RequesterId != userId && fs.AddresseeId != userId)
                return StatusCode(StatusCodes.Status403Forbidden, "You are not part of this friendship.");
                
            var requesterId = fs.RequesterId;
            var addresseeId = fs.AddresseeId;

            var success = await _fsService.DeleteFsAsync(requesterId, addresseeId);
            if (!success)
                return NotFound();

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}