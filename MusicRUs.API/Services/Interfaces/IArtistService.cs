using MusicRUs.API.DTOs;
using System.Linq.Expressions;
namespace MusicRUs.API.Services.Interfaces;

public interface IArtistService
{
    Task<ArtistWithAlbumsResponseDto?> GetArtistByIdAsync(int id); // READ
    Task<ArtistWithAlbumsResponseDto?> GetArtistByNameAsync(string name); // READ
    Task<IEnumerable<ArtistResponseDto>> SearchArtistsByKeywordAsync(string keyword);  // READ
}