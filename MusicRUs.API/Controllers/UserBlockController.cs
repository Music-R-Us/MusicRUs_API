using Microsoft.AspNetCore.Mvc;
using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.Backend.Logic.Jwt;
using Microsoft.AspNetCore.Authorization;
using MusicRUs.API.DTOs;

namespace MusicRUs.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UserBlockController : ControllerBase
{
    private readonly IUserBlockService _ubService;

    public UserBlockController(IUserBlockService ubService)
    {
        _ubService = ubService;
    }

    // Implement the endpoints for UserBlock-related operations

    // CREATE
    //##############################################################################
    // Block a user
    [HttpPost("Block/{blockedId:int}")]
    public async Task<ActionResult> BlockUser(int blockedId)
    {
        try
        {
            var blockerId = User.GetUserId();
            if (blockerId == null)
                return Unauthorized();

            await _ubService.BlockUserAsync(blockerId.Value, blockedId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    // READ
    //##############################################################################
    // Get all blocks by a specific User (blocker)
    [HttpGet("User")]
    public async Task<ActionResult<List<UserBlockResponseDto>>> GetBlocksByBlockerId()
    {
        var blockerId = User.GetUserId();
        if (blockerId == null)
            return Unauthorized();

        var blocks = await _ubService.GetBlocksByBlockerIdAsync(blockerId.Value);
        return Ok(blocks);
    }

    // DELETE
    //##############################################################################
    // Unblock a user
    [HttpDelete("Unblock/{blockedId:int}")]
    public async Task<ActionResult> UnblockUser(int blockedId)
    {
        try
        {
            var blockerId = User.GetUserId();
            if (blockerId == null)
                return Unauthorized();

            await _ubService.UnblockUserAsync(blockerId.Value, blockedId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

}
