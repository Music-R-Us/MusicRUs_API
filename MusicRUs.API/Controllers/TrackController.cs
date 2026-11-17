using MusicRUs.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MusicRUs.API.DTOs;

namespace MusicRUs.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrackController : ControllerBase
{
    private readonly ITrackService _trackService;

    public TrackController(ITrackService trackService)
    {
        _trackService = trackService;
    }

    // READ
    //#########################################
    // Get Track by ID
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TrackResponseDto?>> GetTrackById(int id)
    {
        try
        {
            var track = await _trackService.GetTrackByIdAsync(id);
            return Ok(track);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound($"{ex.Message}");
        }
    }

    //Search Track by Keyword
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<TrackResponseDto>>> SearchTracksByKeyword(string keyword)
    {
        var tracks = await _trackService.SearchTracksByKeywordAsync(keyword);
        return Ok(tracks);
    }
}