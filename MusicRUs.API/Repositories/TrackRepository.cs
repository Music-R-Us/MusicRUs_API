using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MusicRUs.API.Repositories;

public class TrackRepository : Repository<Track>, ITrackRepository
{
    public TrackRepository(DbContext context) : base(context) {}
}