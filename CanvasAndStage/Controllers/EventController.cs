using Microsoft.AspNetCore.Mvc;
using CanvasAndStage.Interfaces;
using CanvasAndStage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CanvasAndStage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : Controller
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
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
            var ev = await _eventService.FindEvent(id);
            if (ev == null) return NotFound();
            return Ok(ev);
        }

        [HttpPut("Update/{id}")]
        //[Authorize(Roles = "admin")]
        public async Task<ActionResult> UpdateEvent(int id, EventDto eventDto)
        {
            if (id != eventDto.EventId) return BadRequest("Event ID mismatch.");

            ServiceResponse response = await _eventService.UpdateEvent(eventDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
                return NotFound(response.Messages);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, response.Messages);

            return NoContent();
        }

        [HttpPost("Add")]
        //[Authorize(Roles = "admin")]
        public async Task<ActionResult<EventDto>> AddEvent(EventDto eventDto)
        {
            ServiceResponse response = await _eventService.AddEvent(eventDto);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, response.Messages);

            eventDto.EventId = (int)response.CreatedId;
            return CreatedAtAction(nameof(FindEvent), new { id = eventDto.EventId }, eventDto);
        }

        [HttpDelete("Delete/{id}")]
        //[Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteEvent(int id)
        {
            ServiceResponse response = await _eventService.DeleteEvent(id);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound) return NotFound();
            if (response.Status == ServiceResponse.ServiceStatus.Error) return StatusCode(500, response.Messages);

            return NoContent();
        }

        [HttpPost("LinkAttendee")]
        //[Authorize(Roles = "admin")]
        public async Task<ActionResult> LinkAttendee(int eventId, int attendeeId)
        {
            ServiceResponse response = await _eventService.LinkAttendeeToEvent(eventId, attendeeId);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return NoContent();
        }

        [HttpDelete("UnlinkAttendee")]
        //[Authorize(Roles = "admin")]
        public async Task<ActionResult> UnlinkAttendee(int eventId, int attendeeId)
        {
            ServiceResponse response = await _eventService.UnlinkAttendeeFromEvent(eventId, attendeeId);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return NoContent();
        }

        [HttpPost("LinkArtist")]
        //[Authorize(Roles = "admin")]
        public async Task<ActionResult> LinkArtist(int eventId, int artistId)
        {
            ServiceResponse response = await _eventService.LinkArtistToEvent(eventId, artistId);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return NoContent();
        }

        [HttpDelete("UnlinkArtist")]
        //[Authorize(Roles = "admin")]
        public async Task<ActionResult> UnlinkArtist(int eventId, int artistId)
        {
            ServiceResponse response = await _eventService.UnlinkArtistFromEvent(eventId, artistId);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return NoContent();
        }

        [HttpGet("ListAttendeesForEvent/{eventId}")]
        public async Task<IActionResult> ListAttendeesForEvent(int eventId)
        {
            var response = await _eventService.ListAttendeesForEvent(eventId);

            if (!response.Success)
                return BadRequest(response.Message);

            return Ok(response.Data);
        }

        [HttpGet("ListArtistsForEvent/{eventId}")]
        public async Task<IActionResult> ListArtistsForEvent(int eventId)
        {
            var response = await _eventService.ListArtistsForEvent(eventId);

            if (!response.Success)
                return BadRequest(response.Message);

            return Ok(response.Data);
        }




    }
}
