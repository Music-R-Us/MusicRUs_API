using MusicRUs.API.DTOs;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Services.Interfaces;

public interface IGenreService
{
    Task<IEnumerable<GenreResponseDto>> GetAllGenresAsync(); // READ
    Task<GenreResponseDto?> GetGenreByIdAsync(int id); // READ
    Task<GenreResponseDto?> GetGenreByNameAsync(string name); // READ
}