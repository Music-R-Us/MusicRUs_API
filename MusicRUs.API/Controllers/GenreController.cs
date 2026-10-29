using Microsoft.AspNetCore.Mvc;
using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicRUs.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenreController : ControllerBase
{
    private readonly IGenreService _genreService;

    public GenreController(IGenreService genreService)
    {
        _genreService = genreService;
    }

    // Implement the endpoints for Genre related operations

    // READ
    //##############################################################################
    // Get all genres
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GenreResponseDto>>> GetAllGenresAsync()
    {
        var genres =await _genreService.GetAllGenresAsync();
        return Ok(genres);
    }

    // Get genre by ID
    [HttpGet("{id:int}")]
    public async Task<ActionResult<GenreResponseDto?>> GetGenreByIdAsync(int id)
    {
        var genre = await _genreService.GetGenreByIdAsync(id);
        if(genre == null)
            return NotFound();
        
        return Ok(genre);
    }

    // Get genre by name
    [HttpGet("by-name")]
    public async Task<ActionResult<GenreResponseDto?>> GetGenreByNameAsync([FromQuery] string name)
    {
        var genre = await _genreService.GetGenreByNameAsync(name);
        if(genre == null)
            return NotFound();
        
        return Ok(genre);
    }

}
