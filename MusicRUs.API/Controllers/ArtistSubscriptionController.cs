using Microsoft.AspNetCore.Mvc;
using MusicRUs.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using MusicRUs.API.Backend.Logic.Jwt;
using MusicRUs.API.DTOs;

namespace MusicRUs.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
    [HttpPost("user/me/artist/{artistId:int}")]
    public async Task<ActionResult<ArtistSubscriptionResponseDto>> Subscribe(int artistId, [FromBody] CreateArtistSubscriptionDto dto)
    {
        try
        {
            var userId = User.GetUserId();
            if(userId == null)
                return Unauthorized();

            var subscription = await _asService.SubscribeAsync(userId.Value, artistId, dto);
            return CreatedAtAction(nameof(GetSubscriptionByUserAndArtist), new { artistId }, subscription);
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
    [HttpGet("user/me/artist/{artistId:int}")]
    public async Task<ActionResult<ArtistSubscriptionResponseDto>> GetSubscriptionByUserAndArtist(int artistId)
    {
        try
        {
            var userId = User.GetUserId();
            if(userId == null)
                return Unauthorized();

            var subscription = await _asService.GetSubscriptionByUserAndArtistAsync(userId.Value, artistId);
            return Ok(subscription);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // Get all artist subscriptions for a user
    [HttpGet("user/me")]
    public async Task<ActionResult<List<ArtistSubscriptionResponseDto>>> GetAllSubscriptionsByUser()
    {
        var userId = User.GetUserId();
        if(userId == null)
            return Unauthorized();

        var subscriptions = await _asService.GetAllSubscriptionsByUserAsync(userId.Value);
        return Ok(subscriptions);
    }

    // UPDATE
    //##############################################################################
    // Update an artist subscription
    [HttpPut("user/me/artist/{artistId:int}")]
    public async Task<ActionResult<ArtistSubscriptionResponseDto>> UpdateSubscription(int artistId, [FromBody] ArtistSubscriptionUpdateDto dto)
    {
        try
        {
            var userId = User.GetUserId();
            if(userId == null)
                return Unauthorized();

            var subscription = await _asService.UpdateSubscriptionAsync(userId.Value, artistId, dto);
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
    [HttpDelete("user/me/artist/{artistId:int}")]
    public async Task<IActionResult> DeleteSubscription(int artistId)
    {
        try
        {
            var userId = User.GetUserId();
            if(userId == null)
                return Unauthorized();

            await _asService.DeleteSubscriptionAsync(userId.Value, artistId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}