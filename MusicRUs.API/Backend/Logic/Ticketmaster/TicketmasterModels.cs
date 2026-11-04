using System.Text.Json.Serialization;

namespace MusicRUs.API.Backend.Logic.Ticketmaster;

// Only the fields we use from the Discovery API v2

public class TicketmasterAttractionSearch
{
    [JsonPropertyName("_embedded")]
    public TicketmasterAttractionList? Embedded { get; init; }
}

public class TicketmasterAttractionList
{
    public List<TicketmasterAttraction> Attractions { get; init; } = [];
}

public class TicketmasterAttraction
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
    public Dictionary<string, List<TicketmasterLink>>? ExternalLinks { get; init; } // fx "spotify"
}

public class TicketmasterLink
{
    public string? Url { get; init; }
}

public class TicketmasterEventSearch
{
    [JsonPropertyName("_embedded")]
    public TicketmasterEventList? Embedded { get; init; }
}

public class TicketmasterEventList
{
    public List<TicketmasterEvent> Events { get; init; } = [];
}

public class TicketmasterEvent
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
    public string? Url { get; init; }
    public TicketmasterDates? Dates { get; init; }

    [JsonPropertyName("_embedded")]
    public TicketmasterEventEmbedded? Embedded { get; init; }
}

public class TicketmasterDates
{
    public TicketmasterStart? Start { get; init; }
    public string? Timezone { get; init; }
    public TicketmasterStatus? Status { get; init; }
}

public class TicketmasterStart
{
    public string? LocalDate { get; init; }
    public string? LocalTime { get; init; }

    [JsonPropertyName("dateTime")]
    public DateTime? DateTimeUtc { get; init; } // Missing if the time is not announced yet
}

public class TicketmasterStatus
{
    public string? Code { get; init; } // onsale, offsale, canceled, postponed, rescheduled
}

public class TicketmasterEventEmbedded
{
    public List<TicketmasterVenue> Venues { get; init; } = [];
}

public class TicketmasterVenue
{
    public string? Name { get; init; }
    public string? Timezone { get; init; }
    public TicketmasterNamed? City { get; init; }
    public TicketmasterNamed? State { get; init; }
    public TicketmasterCountry? Country { get; init; }
}

public class TicketmasterNamed
{
    public string? Name { get; init; }
}

public class TicketmasterCountry
{
    public string? CountryCode { get; init; }
}