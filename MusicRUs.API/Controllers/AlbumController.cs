using Microsoft.AspNetCore.Mvc;
using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.DTOs;

namespace MusicRUs.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlbumController : ControllerBase
{
    private readonly IAlbumService _albumService; 

    public AlbumController(IAlbumService albumService) 
    {
        _albumService = albumService;
    }

    // Implement the endpoints for album related operations

    // READ
    //##############################################################################
    // Get Album by ID
    [HttpGet("{id:int}")]
    public async Task<ActionResult<AlbumDetailsResponseDto?>> GetAlbumByIdAsync(int id)
    {
        try
        {
            var album = await _albumService.GetAlbumByIdAsync(id);
            if (album == null)
                return NotFound();
            return Ok(album);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound($"{ex.Message}");
        }
    }
    
    // Get Album by Spotify ID
    [HttpGet("spotify/{spotifyId}")]
    public async Task<ActionResult<AlbumDetailsResponseDto?>> GetAlbumBySpotifyIdAsync(string spotifyId)
    {
        var album = await _albumService.GetAlbumBySpotifyIdAsync(spotifyId);
        if (album == null)
            return NotFound();
        return Ok(album);
    }

    // Get Album by Title
    [HttpGet("by-title")]
    public async Task<ActionResult<AlbumDetailsResponseDto?>> GetAlbumByTitleAsync([FromQuery] string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return BadRequest("Title is required.");

        var album = await _albumService.GetAlbumByTitleAsync(title);
        if (album == null)
            return NotFound();
        return Ok(album);
    }

    // Get all Albums by Artist ID
    [HttpGet("artist/{artistId:int}")]
    public async Task<ActionResult<IEnumerable<AlbumResponseDto>>> GetAlbumsByArtistIdAsync(int artistId)
    {
        var albums = await _albumService.GetAlbumsByArtistIdAsync(artistId);
        if (albums == null || !albums.Any())
            return NotFound();
        
        return Ok(albums);
    }

    // Search albums by keyword
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<AlbumResponseDto>>> SearchAlbumsByKeywordAsync([FromQuery] string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return BadRequest("Keyword is required.");
    
        return Ok(await _albumService.SearchAlbumsByKeywordAsync(keyword));
    }
}