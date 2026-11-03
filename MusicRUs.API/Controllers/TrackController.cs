using MusicRUs.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MusicRUs.API.DTOs;
using MusicRUs.API.Models.Entities;

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

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TrackResponseDto?>> GetTrackById(int id)
    {
        var track = await _trackService.GetTrackByIdAsync(id);
        if (track == null)
            return NotFound();

        return Ok(track);
    }
}