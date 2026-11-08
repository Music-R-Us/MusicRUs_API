using Microsoft.AspNetCore.Mvc;
using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.DTOs;

namespace MusicRUs.API.Controllers;

[ApiController]
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
    [HttpPost("Block/{blockerId:int}/{blockedId:int}")]
    public async Task<ActionResult> BlockUser(int blockerId, int blockedId)
    {
        try
        {
            await _ubService.BlockUserAsync(blockerId, blockedId);
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
    [HttpGet("User/{blockerId:int}")]
    public async Task<ActionResult<List<UserBlockResponseDto>>> GetBlocksByBlockerId(int blockerId)
    {
        var blocks = await _ubService.GetBlocksByBlockerIdAsync(blockerId);
        return Ok(blocks);
    }

    // DELETE
    //##############################################################################
    // Unblock a user
    [HttpDelete("Unblock/{blockerId:int}/{blockedId:int}")]
    public async Task<ActionResult> UnblockUser(int blockerId, int blockedId)
    {
        try
        {
            await _ubService.UnblockUserAsync(blockerId, blockedId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

}
