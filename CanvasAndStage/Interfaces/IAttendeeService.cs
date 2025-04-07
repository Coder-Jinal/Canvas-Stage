using CanvasAndStage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CanvasAndStage.Interfaces
{
    public interface IAttendeeService
    {
        Task<IEnumerable<AttendeeDto>> ListAttendees();
        Task<AttendeeDto?> FindAttendee(int id);
        Task<ServiceResponse> AddAttendee(AddAttendeeDto dto);
        Task<ServiceResponse> UpdateAttendee(int id, UpdateAttendeeDto dto);
        Task<ServiceResponse> DeleteAttendee(int id);
    }
}
