using System.Globalization;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace MusicRUs.API.Backend.Logic.Ticketmaster;

public class TicketmasterClient : ITicketmasterClient
{
    private const int PageSize = 100;
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _http;
    private readonly TicketmasterOptions _options;

    public TicketmasterClient(HttpClient http, IOptions<TicketmasterOptions> options)
    {
        _http = http;
        _options = options.Value;
    }

    public bool IsConfigured => !string.IsNullOrWhiteSpace(_options.ApiKey);

    public async Task<IReadOnlyList<TicketmasterAttraction>> SearchAttractionsAsync(string name)
    {
        var result = await GetAsync<TicketmasterAttractionSearch>(
            $"discovery/v2/attractions.json?keyword={Uri.EscapeDataString(name)}&classificationName=music&size=20");
        return result?.Embedded?.Attractions ?? [];
    }

    public async Task<IReadOnlyList<TicketmasterEvent>> GetUpcomingEventsAsync(string attractionId)
    {
        var now = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture);
        var result = await GetAsync<TicketmasterEventSearch>(
            $"discovery/v2/events.json?attractionId={Uri.EscapeDataString(attractionId)}" +
            $"&startDateTime={now}&sort=date,asc&size={PageSize}");
        return result?.Embedded?.Events ?? [];
    }

    private async Task<T?> GetAsync<T>(string url) where T : class
    {
        url += $"&apikey={Uri.EscapeDataString(_options.ApiKey!)}";

        for (var attempt = 1; ; attempt++)
        {
            using var response = await _http.GetAsync(url);

            if (response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.BadRequest)
                return null;

            // Max 5 calls per second. Wait and try once more
            if (attempt == 1 && response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                continue;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(Json);
        }
    }
}