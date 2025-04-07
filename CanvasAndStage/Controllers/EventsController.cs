using CanvasAndStage.Interfaces;
using CanvasAndStage.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CanvasAndStage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<EventDto>>> ListEvents()
        {
            IEnumerable<EventDto> eventDtos = await _eventService.ListEvents();
            return Ok(eventDtos);
        }

        [HttpGet("Find/{id}")]
        public async Task<ActionResult<EventDto>> FindEvent(int id)
        {
            var eventDto = await _eventService.FindEvent(id);

            if (eventDto == null)
            {
                return NotFound($"Event with ID {id} doesn't exist");
            }
            else
            {
                return Ok(eventDto);
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddEvent(AddEventDto dto)
        {
            ServiceResponse response = await _eventService.AddEvent(dto);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, new { error = "An unexpected error occurred while adding the event." });
            }

            return CreatedAtAction("FindEvent", new { id = response.CreatedId }, new
            {
                message = $"Event added successfully with ID {response.CreatedId}",
                eventId = response.CreatedId
            });
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateEvent(int id, UpdateEventDto dto)
        {
            if (id != dto.EventId)
            {
                return BadRequest(new { message = "Event ID mismatch." });
            }

            ServiceResponse response = await _eventService.UpdateEvent(id, dto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(new { error = "Event not found." });
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, new { error = "An unexpected error occurred while updating the event." });
            }

            return Ok(new { message = $"Event with ID {id} updated successfully." });
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            ServiceResponse response = await _eventService.DeleteEvent(id);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(new { error = "Event not found." });
            }

            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, new { error = "An unexpected error occurred while deleting the event." });
            }

            return Ok(new { message = $"Event with ID {id} deleted successfully." });
        }



        [HttpPost("LinkArtist")]
        public async Task<IActionResult> LinkArtist([FromBody] LinkArtistDto dto)
        {
            var response = await _eventService.LinkArtist(dto.EventId, dto.ArtistId);
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(new { error = "Event or Artist not found." });
            }
            return Ok(new { message = "Artist linked to event successfully." });
        }

        [HttpPost("UnlinkArtist")]
        public async Task<IActionResult> UnlinkArtist([FromBody] LinkArtistDto dto)
        {
            var response = await _eventService.UnlinkArtist(dto.EventId, dto.ArtistId);
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(new { error = "Event or Artist not found." });
            }
            return Ok(new { message = "Artist unlinked from event successfully." });
        }

        [HttpPost("LinkAttendee")]
        public async Task<IActionResult> LinkAttendee([FromBody] LinkAttendeeDto dto)
        {
            var response = await _eventService.LinkAttendee(dto.EventId, dto.AttendeeId);
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(new { error = "Event or Attendee not found." });
            }
            return Ok(new { message = "Attendee linked to event successfully." });
        }

        [HttpPost("UnlinkAttendee")]
        public async Task<IActionResult> UnlinkAttendee([FromBody] LinkAttendeeDto dto)
        {
            var response = await _eventService.UnlinkAttendee(dto.EventId, dto.AttendeeId);
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(new { error = "Event or Attendee not found." });
            }
            return Ok(new { message = "Attendee unlinked from event successfully." });
        }
    }

    public class LinkArtistDto
    {
        public int EventId { get; set; }
        public int ArtistId { get; set; }
    }

    public class LinkAttendeeDto
    {
        public int EventId { get; set; }
        public int AttendeeId { get; set; }
    }
}

