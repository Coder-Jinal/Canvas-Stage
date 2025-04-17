using CanvasAndStage.Models;
using CanvasAndStage.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CanvasAndStage.Interfaces
{
    public interface IEventService
    {
        Task<IEnumerable<EventDto>> ListEvents();
        Task<EventDto?> FindEvent(int id);
        Task<ServiceResponse> AddEvent(AddEventDto dto);
        Task<ServiceResponse> UpdateEvent(int id, UpdateEventDto dto);
        Task<ServiceResponse> DeleteEvent(int id);

        Task<ServiceResponse> LinkAttendee(int eventId, int attendeeId);
        Task<ServiceResponse> UnlinkAttendee(int eventId, int attendeeId);

        Task<ServiceResponse> LinkArtist(int eventId, int artistId);
        Task<ServiceResponse> UnlinkArtist(int eventId, int artistId);

        Task<PaginatedResult<EventDto>> GetPaginatedEvents(int page, int pageSize);
    }
}
