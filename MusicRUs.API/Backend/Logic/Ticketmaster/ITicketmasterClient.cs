namespace MusicRUs.API.Backend.Logic.Ticketmaster;

public interface ITicketmasterClient
{
    bool IsConfigured { get; }
    Task<IReadOnlyList<TicketmasterAttraction>> SearchAttractionsAsync(string name);
    Task<IReadOnlyList<TicketmasterEvent>> GetUpcomingEventsAsync(string attractionId);
}