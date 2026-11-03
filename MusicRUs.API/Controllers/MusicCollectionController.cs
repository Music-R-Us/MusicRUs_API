using Microsoft.AspNetCore.Mvc;
using MusicRUs.API.DTOs;
using MusicRUs.API.Services.Interfaces;

namespace MusicRUs.API.Controllers;

[ApiController]
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
        var item = await _mcService.AddToOwnedCollectionAsync(dto);
        if(item == null)
        {
            var album = await _albumService.GetAlbumByIdAsync(dto.AlbumId);
            return Conflict($"{album?.Title} ({dto.Format}) already exists in your collection");
        }
        return Ok(item);
    }

    // Add Wanted Collection Item
    [HttpPost("wanted")]
    public async Task<ActionResult<MusicCollectionResponseDto>> AddToWantedCollection([FromBody] CreateWantedMusicCollectionDto dto)
    {
        var item = await _mcService.AddToWantedCollectionAsync(dto);
        if(item == null)
        {
            var album = await _albumService.GetAlbumByIdAsync(dto.AlbumId);
            return Conflict($"{album?.Title} ({dto.Format}) already exists in your wanted-list");
        }
        return Ok(item);
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
    [HttpGet("{userId:int}/{collectionId:int}")]
    public async Task<ActionResult<MusicCollectionResponseDto>> GetCollectionItem(int userId, int collectionId)
    {
        var item = await _mcService.GetCollectionItemAsync(userId, collectionId);
        if(item == null)
            return NotFound();
        return Ok(item);
    }

    // DELETE
    //##############################################################################
    // Delete Collection Item by Id
    [HttpDelete("{userId:int}/{collectionId:int}")]
    public async Task<ActionResult> DeleteCollectionItem(int userId, int collectionId)
    {
        var success = await _mcService.DeleteCollectionItemAsync(userId, collectionId);
        if(!success)
            return NotFound();
        return NoContent();
    }
}