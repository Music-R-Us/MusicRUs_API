using System.Globalization;
using Microsoft.Extensions.Caching.Memory;
using MusicRUs.API.Backend.Logic.Ticketmaster;
using MusicRUs.API.DTOs;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;
using MusicRUs.API.Services.Interfaces;

namespace MusicRUs.API.Services;

public class ConcertService : IConcertService
{
    private static readonly TimeSpan RefreshEvery = TimeSpan.FromHours(12);

    private readonly IConcertRepository _concerts;
    private readonly ITicketmasterClient _ticketmaster;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ConcertService> _logger;

    public ConcertService(IConcertRepository concerts, ITicketmasterClient ticketmaster,
        IMemoryCache cache, ILogger<ConcertService> logger)
    {
        _concerts = concerts;
        _ticketmaster = ticketmaster;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ConcertResponseDto>?> GetUpcomingConcertsAsync(int artistId, string? countryCode)
    {
        var artist = await _concerts.GetArtistAsync(artistId);
        if (artist is null)
            return null;

        await TryRefreshAsync(artist);

        var code = string.IsNullOrWhiteSpace(countryCode) ? null : countryCode.Trim().ToUpperInvariant();
        var concerts = await _concerts.GetUpcomingAsync(artistId, DateTime.UtcNow, code);
        return concerts.Select(MapToDto).ToList();
    }

    // Fetch from Ticketmaster at most every 12 hours per artist. On errors the database is used
    private async Task TryRefreshAsync(Artist artist)
    {
        var cacheKey = $"ticketmaster:concerts:{artist.Id}";
        if (!_ticketmaster.IsConfigured || _cache.TryGetValue(cacheKey, out _))
            return;

        try
        {
            await RefreshAsync(artist);
            _cache.Set(cacheKey, true, RefreshEvery);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            _logger.LogWarning(ex, "Could not fetch concerts from Ticketmaster for artist {ArtistId}.", artist.Id);
        }
    }

    private async Task RefreshAsync(Artist artist)
    {
        artist.TicketmasterId ??= await FindAttractionIdAsync(artist);
        if (artist.TicketmasterId is null)
            return; // Not on Ticketmaster (we try again in 12 hours)

        var events = await _ticketmaster.GetUpcomingEventsAsync(artist.TicketmasterId);
        var existing = await _concerts.GetUpcomingFromTicketmasterAsync(artist.Id, DateTime.UtcNow);
        var byExternalId = existing.ToDictionary(c => c.ExternalId!);
        var seen = new HashSet<string>();
        DateTime? lastSeen = null;

        foreach (var ev in events)
        {
            if (IsCancelled(ev) || !seen.Add(ev.Id))
                continue;

            var startsAt = GetStartsAtUtc(ev);
            if (startsAt is null)
                continue;
            if (lastSeen is null || startsAt > lastSeen)
                lastSeen = startsAt;

            if (!byExternalId.TryGetValue(ev.Id, out var concert))
            {
                concert = new Concert { ArtistId = artist.Id, ExternalId = ev.Id };
                _concerts.Add(concert);
            }

            var venue = ev.Embedded?.Venues.FirstOrDefault();
            concert.Name = Cut(ev.Name, 255) ?? artist.Name;
            concert.VenueName = Cut(venue?.Name, 255) ?? "Unknown venue";
            concert.City = Cut(venue?.City?.Name, 100);
            concert.Region = Cut(venue?.State?.Name, 100);
            concert.CountryCode = venue?.Country?.CountryCode is { Length: 2 } cc ? cc.ToUpperInvariant() : null;
            concert.StartsAt = startsAt.Value;
            concert.VenueTimeZone = Cut(ev.Dates?.Timezone ?? venue?.Timezone, 64);
            concert.Url = ev.Url;
        }

        // Remove concerts Ticketmaster no longer lists (cancelled or moved).
        // Only up to the last date we got, in case the list was cut off at 100
        foreach (var old in existing.Where(c => !seen.Contains(c.ExternalId!) && (lastSeen is null || c.StartsAt <= lastSeen)))
            _concerts.Remove(old);

        await _concerts.SaveChangesAsync();
    }

    private async Task<string?> FindAttractionIdAsync(Artist artist)
    {
        var attractions = await _ticketmaster.SearchAttractionsAsync(artist.Name);

        // Best match: Ticketmaster links to the same Spotify artist
        var match = attractions.FirstOrDefault(a => SpotifyLinks(a).Any(url => url.Contains(artist.SpotifyId, StringComparison.Ordinal)));

        // Otherwise only if exactly one has the same name and no Spotify link to someone else
        if (match is null)
        {
            var sameName = attractions
                .Where(a => string.Equals(a.Name, artist.Name, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (sameName.Count == 1 && !SpotifyLinks(sameName[0]).Any())
                match = sameName[0];
        }

        if (match is null)
            return null;

        // ticketmaster_id is unique, so it can only belong to one artist
        return await _concerts.TicketmasterIdInUseAsync(match.Id, artist.Id) ? null : match.Id;
    }

    private static IEnumerable<string> SpotifyLinks(TicketmasterAttraction attraction)
        => attraction.ExternalLinks is not null && attraction.ExternalLinks.TryGetValue("spotify", out var links)
            ? links.Select(l => l.Url).OfType<string>()
            : [];

    private static bool IsCancelled(TicketmasterEvent ev)
        => ev.Dates?.Status?.Code is { } code
           && (code.Equals("canceled", StringComparison.OrdinalIgnoreCase)
               || code.Equals("cancelled", StringComparison.OrdinalIgnoreCase));

    private static DateTime? GetStartsAtUtc(TicketmasterEvent ev)
    {
        var start = ev.Dates?.Start;
        if (start?.DateTimeUtc is { } utc)
            return DateTime.SpecifyKind(utc, DateTimeKind.Utc);

        // No exact time yet: use the local date (and time if known) in the venue's time zone
        if (!DateOnly.TryParse(start?.LocalDate, CultureInfo.InvariantCulture, out var date))
            return null;
        var time = TimeOnly.TryParse(start?.LocalTime, CultureInfo.InvariantCulture, out var t) ? t : TimeOnly.MinValue;
        var local = date.ToDateTime(time);

        try
        {
            var zone = TimeZoneInfo.FindSystemTimeZoneById(ev.Dates?.Timezone ?? "UTC");
            return TimeZoneInfo.ConvertTimeToUtc(local, zone);
        }
        catch (Exception ex) when (ex is TimeZoneNotFoundException or InvalidTimeZoneException)
        {
            return DateTime.SpecifyKind(local, DateTimeKind.Utc);
        }
    }

    private static string? Cut(string? value, int max)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        var trimmed = value.Trim();
        return trimmed.Length <= max ? trimmed : trimmed[..max];
    }

    private static ConcertResponseDto MapToDto(Concert c) => new()
    {
        Id = c.Id,
        ArtistId = c.ArtistId,
        Name = c.Name,
        VenueName = c.VenueName,
        City = c.City,
        Region = c.Region,
        CountryCode = c.CountryCode,
        StartsAt = c.StartsAt,
        VenueTimeZone = c.VenueTimeZone,
        Url = c.Url
    };
}