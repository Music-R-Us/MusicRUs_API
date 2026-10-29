using MusicRUs.API.DTOs;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;
using MusicRUs.API.Services.Interfaces;

namespace MusicRUs.API.Services;

public class GenreService : IGenreService
{
    private readonly IGenreRepository _genreRepo;

    public GenreService(IGenreRepository genreRepo)
    {
        _genreRepo = genreRepo;
    }

    // Implement the methods defined in IGenreService here

    // Get all genres
    public async Task<IEnumerable<GenreResponseDto>> GetAllGenresAsync() // READ
    {
        var genres = await _genreRepo.GetAllAsync();
        return genres.Select(MapToDto);
    }

    // Get genre by ID
    public async Task<GenreResponseDto?> GetGenreByIdAsync(int id) // READ
    {
        var genre = await _genreRepo.GetByIdAsync(id);
        return genre is null ? null : MapToDto(genre); 
    }
    // Get genre by name
    public async Task<GenreResponseDto?> GetGenreByNameAsync(string name) // READ
    {
        var genre = await _genreRepo.GetByStringAsync(name);
        return genre is null ? null : MapToDto(genre);
    }

    //#########################################

    // Helper method to map Genre entity to GenreResponseDto (MapToDto)
    private static GenreResponseDto MapToDto(Genre genre) => new()
    {
        Id = genre.Id,
        Name = genre.Name
    };
}