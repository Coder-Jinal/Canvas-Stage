using System.Collections.Generic;
using System.Threading.Tasks;
using CanvasAndStage.Models;

namespace CanvasAndStage.Interfaces
{
    public interface IAttendeeService
    {
        Task<IEnumerable<AttendeeDto>> ListAttendees();
        Task<AttendeeDto?> FindAttendee(int id);
        Task<ServiceResponse> UpdateAttendee(AttendeeDto attendeeDto);
        Task<ServiceResponse> AddAttendee(AttendeeDto attendeeDto);
        Task<ServiceResponse> DeleteAttendee(int id);
        Task<ServiceResponse> ListEventsForAttendee(int attendeeId);
    }
}
