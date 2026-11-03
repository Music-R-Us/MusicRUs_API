using MusicRUs.API.DTOs;

namespace MusicRUs.API.Services.Interfaces;

public interface ITrackService
{
    Task<TrackResponseDto> GetTrackByIdAsync(int id); // READ
}