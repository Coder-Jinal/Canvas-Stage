using CanvasAndStage.Models;
using CanvasAndStage.Services;

namespace CanvasAndStage.Interfaces
{
    public interface IEventService
    {
        Task<IEnumerable<EventDto>> ListEvents();
        Task<EventDto?> FindEvent(int id);
        Task<ServiceResponse> UpdateEvent(EventDto eventDto);
        Task<ServiceResponse> AddEvent(EventDto eventDto);
        Task<ServiceResponse> DeleteEvent(int id);
        Task<ServiceResponse> LinkAttendeeToEvent(int attendeeid, int eventid);
        Task<ServiceResponse> UnlinkAttendeeFromEvent(int attendeeid, int eventid);
        Task<ServiceResponse> LinkArtistToEvent(int artistid, int eventid);
        Task<ServiceResponse> UnlinkArtistFromEvent(int artistid, int eventid);
        Task<ServiceResponse> ListAttendeesForEvent(int eventId);
        Task<ServiceResponse> ListArtistsForEvent(int eventId);
    }
}
