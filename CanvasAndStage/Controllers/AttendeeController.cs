using Microsoft.AspNetCore.Mvc;
using CanvasAndStage.Interfaces;
using CanvasAndStage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CanvasAndStage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendeeController : Controller
    {
        private readonly IAttendeeService _attendeeService;

        public AttendeeController(IAttendeeService attendeeService)
        {
            _attendeeService = attendeeService;
        }

        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<AttendeeDto>>> ListAttendees()
        {
            var attendees = await _attendeeService.ListAttendees();
            return Ok(attendees);
        }

        [HttpGet("Find/{id}")]
        public async Task<ActionResult<AttendeeDto>> FindAttendee(int id)
        {
            var attendee = await _attendeeService.FindAttendee(id);
            if (attendee == null) return NotFound();
            return Ok(attendee);
        }

        [HttpPut("Update/{id}")]
        public async Task<ActionResult> UpdateAttendee(int id, AttendeeDto attendeeDto)
        {
            if (id != attendeeDto.AttendeeId) return BadRequest("Attendee ID mismatch.");

            ServiceResponse response = await _attendeeService.UpdateAttendee(attendeeDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
                return NotFound(response.Messages);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, response.Messages);

            return NoContent();
        }

        [HttpPost("Add")]
        public async Task<ActionResult<AttendeeDto>> AddAttendee(AttendeeDto attendeeDto)
        {
            ServiceResponse response = await _attendeeService.AddAttendee(attendeeDto);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, response.Messages);

            attendeeDto.AttendeeId = (int)response.CreatedId;
            return CreatedAtAction(nameof(FindAttendee), new { id = attendeeDto.AttendeeId }, attendeeDto);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> DeleteAttendee(int id)
        {
            ServiceResponse response = await _attendeeService.DeleteAttendee(id);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound) return NotFound();
            if (response.Status == ServiceResponse.ServiceStatus.Error) return StatusCode(500, response.Messages);

            return NoContent();
        }

        [HttpGet("ListEventsForAttendee/{attendeeId}")]
        public async Task<IActionResult> ListEventsForAttendee(int attendeeId)
        {
            var response = await _attendeeService.ListEventsForAttendee(attendeeId);

            if (!response.Success)
                return BadRequest(response.Message);

            return Ok(response.Data);
        }

    }
}
