using Microsoft.AspNetCore.Mvc;
using MusicRUs.API.DTOs;
using MusicRUs.API.Services.Interfaces;

namespace MusicRUs.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConcertController : ControllerBase
{
    private readonly IConcertService _concertService;

    public ConcertController(IConcertService concertService)
    {
        _concertService = concertService;
    }

    // READ
    //##############################################################################
    // Upcoming concerts for an artist. countryCode (fx DK) is optional
    [HttpGet("artist/{artistId:int}")]
    public async Task<ActionResult<IEnumerable<ConcertResponseDto>>> GetUpcomingByArtist(int artistId, [FromQuery] string? countryCode)
    {
        var concerts = await _concertService.GetUpcomingConcertsAsync(artistId, countryCode);
        if (concerts == null)
            return NotFound();

        return Ok(concerts);
    }
}