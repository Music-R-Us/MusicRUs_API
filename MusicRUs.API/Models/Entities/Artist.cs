using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicRUs.API.Models.Entities;

//Represents an artist or band in the system from the Spotify API. 
public class Artist : IHasTimestamps
{
    public int Id {get; set;}
    public string SpotifyId {get; set;} = null!; //Spotify's 22-character ID. Unique, so re-importing never creates duplicates.
    public string Name {get; set;} = null!;
    public string? ImageUrl {get; set;}
    public string? SpotifyUrl {get; set;}
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<Album> Albums { get; } = [];
    public List<Track> Tracks { get; } = [];
    public List<Genre> Genres { get; set; } = []; 
    public string? TicketmasterId { get; set; } // Ticketmaster's ID for the artist
    public List<Concert> Concerts { get; set; } = [];
}