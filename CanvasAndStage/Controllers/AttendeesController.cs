using CanvasAndStage.Interfaces;
using CanvasAndStage.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CanvasAndStage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendeesController : ControllerBase
    {
        private readonly IAttendeeService _attendeeService;

        public AttendeesController(IAttendeeService attendeeService)
        {
            _attendeeService = attendeeService;
        }

        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<AttendeeDto>>> ListAttendees()
        {
            var attendeeDtos = await _attendeeService.ListAttendees();
            return Ok(attendeeDtos);
        }

        [HttpGet("Find/{id}")]
        public async Task<ActionResult<AttendeeDto>> FindAttendee(int id)
        {
            var attendee = await _attendeeService.FindAttendee(id);

            if (attendee == null)
            {
                return NotFound($"Attendee with ID {id} doesn't exist.");
            }

            return Ok(attendee);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddAttendee(AddAttendeeDto dto)
        {
            ServiceResponse response = await _attendeeService.AddAttendee(dto);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, new { error = "An error occurred while adding the attendee." });
            }

            return CreatedAtAction("FindAttendee", new { id = response.CreatedId }, new
            {
                message = $"Attendee added successfully with ID {response.CreatedId}",
                attendeeId = response.CreatedId
            });
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateAttendee(int id, UpdateAttendeeDto dto)
        {
            if (id != dto.AttendeeId)
            {
                return BadRequest(new { message = "Attendee ID mismatch." });
            }

            ServiceResponse response = await _attendeeService.UpdateAttendee(id, dto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(new { error = "Attendee not found." });
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, new { error = "An error occurred while updating the attendee." });
            }

            return Ok(new { message = $"Attendee with ID {id} updated successfully." });
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteAttendee(int id)
        {
            ServiceResponse response = await _attendeeService.DeleteAttendee(id);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(new { error = "Attendee not found." });
            }

            return Ok(new { message = $"Attendee with ID {id} deleted successfully." });
        }
    }
}
