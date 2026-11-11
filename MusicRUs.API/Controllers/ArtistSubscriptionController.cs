using Microsoft.AspNetCore.Mvc;
using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.DTOs;

namespace MusicRUs.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArtistSubscriptionController : ControllerBase
{
    private readonly IArtistSubscriptionService _asService;

    public ArtistSubscriptionController(IArtistSubscriptionService asService)
    {
        _asService = asService;
    }

    // Implement the endpoints for ArtistSubscription related operations

    // CREATE
    //##############################################################################
    // Create a new artist subscription
    [HttpPost("subscribe/{userId:int}/{artistId:int}")]
    public async Task<ActionResult<ArtistSubscriptionResponseDto>> Subscribe(int userId, int artistId, [FromBody] CreateArtistSubscriptionDto dto)
    {
        try
        {
            var subscription = await _asService.SubscribeAsync(userId, artistId, dto);
            return CreatedAtAction(nameof(GetSubscriptionByUserAndArtist), new { UserId = userId, ArtistId = artistId }, subscription);
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
    // Get artist subscription by userId and artistId
    [HttpGet("user/{userId:int}/artist/{artistId:int}")]
    public async Task<ActionResult<ArtistSubscriptionResponseDto>> GetSubscriptionByUserAndArtist(int userId, int artistId)
    {
        try
        {
            var subscription = await _asService.GetSubscriptionByUserAndArtistAsync(userId, artistId);
            return Ok(subscription);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // Get all artist subscriptions for a user
    [HttpGet("user/{userId:int}")]
    public async Task<ActionResult<IEnumerable<ArtistSubscriptionResponseDto>>> GetAllSubscriptionsByUser(int userId)
    {
        var result = await _asService.GetAllSubscriptionsByUserAsync(userId);
        return Ok(result);
    }

    // UPDATE
    //##############################################################################
    // Update an artist subscription
    [HttpPut("user/{userId:int}/artist/{artistId:int}")]
    public async Task<ActionResult<ArtistSubscriptionResponseDto>> UpdateSubscription(int userId, int artistId, [FromBody] ArtistSubscriptionUpdateDto dto)
    {
        try
        {
            var subscription = await _asService.UpdateSubscriptionAsync(userId, artistId, dto);
            return Ok(subscription);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // DELETE
    //##############################################################################
    // Delete an artist subscription
    [HttpDelete("user/{userId:int}/artist/{artistId:int}")]
    public async Task<IActionResult> DeleteSubscription(int userId, int artistId)
    {
        try
        {
            await _asService.DeleteSubscriptionAsync(userId, artistId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}