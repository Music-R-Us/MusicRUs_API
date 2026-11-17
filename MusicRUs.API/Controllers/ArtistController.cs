using Microsoft.AspNetCore.Mvc;
using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.DTOs;

namespace MusicRUs.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArtistController : ControllerBase
{
    private readonly IArtistService _artistService;

    public ArtistController(IArtistService artistService)
    {
        _artistService = artistService;
    }
    
    // Implement the endpoints for artist related operations

    // READ
    //##############################################################################
    // Get Artist by ID
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ArtistWithAlbumsResponseDto?>> GetArtistByIdAsync(int id)
    {
        try
        {
            var artist = await _artistService.GetArtistByIdAsync(id);
            return Ok(artist);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound($"{ex.Message}");
        }
    }
 
    // Get Artist by Name
    [HttpGet("by-name")]
    public async Task<ActionResult<ArtistWithAlbumsResponseDto?>> GetArtistByNameAsync(string name) 
    {
        try
        {
            var artist = await _artistService.GetArtistByNameAsync(name);
            return Ok(artist);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound($"{ex.Message}");
        }
    }

    // Search Artists by Keyword
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ArtistResponseDto>>> SearchArtistsByKeywordAsync(string keyword)
    {
        var artists = await _artistService.SearchArtistsByKeywordAsync(keyword);
        return Ok(artists);
    }
}