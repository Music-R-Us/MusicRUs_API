using Microsoft.AspNetCore.Mvc;
using MusicRUs.API.DTOs;
using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.Backend.Logic.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace MusicRUs.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MusicCollectionController : ControllerBase
{
    private readonly IMusicCollectionService _mcService;
    private readonly IAlbumService _albumService;

    public MusicCollectionController(IMusicCollectionService mcService, IAlbumService albumService)
    {
        _mcService = mcService;
        _albumService = albumService;
    }


    // Implement the endpoints for collection-related operations

    // CREATE
    //##############################################################################
    // Add Owned Collection Item
    [HttpPost("owned")]
    public async Task<ActionResult<MusicCollectionResponseDto>> AddToOwnedCollection([FromBody] CreateOwnedMusicCollectionDto dto)
    {
        try
        {
            var userId = User.GetUserId();
            if(userId == null)
                return Unauthorized();
            
            var item = await _mcService.AddToOwnedCollectionAsync(userId.Value, dto);
            return Ok(item);
        }
        catch (InvalidOperationException)
        {
            var album = await _albumService.GetAlbumByIdAsync(dto.AlbumId);
            return Conflict($"{album?.Title} ({dto.Format}) already exists in your collection");
        }
    }

    // Add Wanted Collection Item
    [HttpPost("wanted")]
    public async Task<ActionResult<MusicCollectionResponseDto>> AddToWantedCollection([FromBody] CreateWantedMusicCollectionDto dto)
    {
        try
        {
            var userId = User.GetUserId();
            if(userId == null)
                return Unauthorized();

            var item = await _mcService.AddToWantedCollectionAsync(userId.Value, dto);
            return Ok(item);
        }
        catch (InvalidOperationException)
        {
            var album = await _albumService.GetAlbumByIdAsync(dto.AlbumId);
            return Conflict($"{album?.Title} ({dto.Format}) already exists in your wanted-list");
        }
    }

    // READ
    //##############################################################################
    // Get All Owned Collection items by userId
    [HttpGet("owned/{userId:int}")]
    public async Task<ActionResult<List<MusicCollectionResponseDto>>> GetAllOwnedCollectionItemsByUserId(int userId)
    {
        var items = await _mcService.GetOwnedCollectionByUserIdAsync(userId);
        return Ok(items);
    }
    
    // Get All Wanted Collection items by userId
    [HttpGet("wanted/{userId:int}")]
    public async Task<ActionResult<List<MusicCollectionResponseDto>>> GetAllWantedCollectionItemsByUserId(int userId)
    {
        var items = await _mcService.GetWantedCollectionByUserIdAsync(userId);
        return Ok(items);
    }

    // Get Collection Item by Id
    [HttpGet("{collectionId:int}")]
    public async Task<ActionResult<MusicCollectionResponseDto>> GetCollectionItem(int collectionId)
    {
        try
        {
            var userId = User.GetUserId();
            if(userId == null)
                return Unauthorized();

            var item = await _mcService.GetCollectionItemAsync(userId.Value, collectionId);
            return Ok(item);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // DELETE
    //##############################################################################
    // Delete Collection Item by Id
    [HttpDelete("{collectionId:int}")]
    public async Task<ActionResult> DeleteCollectionItem(int collectionId)
    {
        try
        {
            var userId = User.GetUserId();
            if(userId == null)
                return Unauthorized();

            var success = await _mcService.DeleteCollectionItemAsync(userId.Value, collectionId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}