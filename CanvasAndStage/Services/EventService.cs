using CanvasAndStage.Interfaces;
using CanvasAndStage.Models;
using CanvasAndStage.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace CanvasAndStage.Services
{
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _context;

        public EventService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventDto>> ListEvents()
        {
            List<Event> events = await _context.Events.ToListAsync();
            List<EventDto> eventDtos = new();

            foreach (Event ev in events)
            {
                eventDtos.Add(new EventDto()
                {
                    EventId = ev.EventId,
                    Name = ev.Name,
                    Description = ev.Description,
                    Location = ev.Location,
                    Date = ev.Date
                });
            }

            return eventDtos;
        }

        public async Task<EventDto?> FindEvent(int id)
        {
            var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventId == id);

            if (ev == null) return null;

            return new EventDto()
            {
                EventId = ev.EventId,
                Name = ev.Name,
                Description = ev.Description,
                Location = ev.Location,
                Date = ev.Date
            };
        }

        public async Task<ServiceResponse> UpdateEvent(EventDto eventDto)
        {
            ServiceResponse response = new();

            var ev = await _context.Events.FindAsync(eventDto.EventId);

            if (ev == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Event not found.");
                return response;
            }

            ev.Name = eventDto.Name;
            ev.Description = eventDto.Description;
            ev.Location = eventDto.Location;
            ev.Date = eventDto.Date;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error updating the event.");
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Updated;
            return response;
        }


        public async Task<ServiceResponse> AddEvent(EventDto eventDto)
        {
            ServiceResponse response = new();

            Event ev = new()
            {
                Name = eventDto.Name,
                Description = eventDto.Description,
                Location = eventDto.Location,
                Date = eventDto.Date
            };

            try
            {
                await _context.Events.AddAsync(ev);
                await _context.SaveChangesAsync();

                response.Status = ServiceResponse.ServiceStatus.Created;
                response.CreatedId = ev.EventId;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error adding the event.");
                response.Messages.Add(ex.Message);
                return response;
            }

            return response;
        }


        public async Task<ServiceResponse> DeleteEvent(int id)
        {
            ServiceResponse response = new();

            var ev = await _context.Events.FindAsync(id);
            if (ev == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Event not found.");
                return response;
            }

            try
            {
                _context.Events.Remove(ev);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error deleting the event.");
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Deleted;
            return response;
        }

        public async Task<ServiceResponse> LinkAttendeeToEvent(int eventId, int attendeeId)
        {
            ServiceResponse serviceResponse = new();

            Event? eventEntity = await _context.Events
                .Include(e => e.Attendees)
                .Where(e => e.EventId == eventId)
                .FirstOrDefaultAsync();
            Attendee? attendee = await _context.Attendees.FindAsync(attendeeId);

            if (eventEntity == null || attendee == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                if (eventEntity == null) serviceResponse.Messages.Add("Event was not found.");
                if (attendee == null) serviceResponse.Messages.Add("Attendee was not found.");
                return serviceResponse;
            }

            try
            {
                eventEntity.Attendees.Add(attendee);
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("There was an issue linking the attendee to the event.");
                serviceResponse.Messages.Add(ex.Message);
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse> UnlinkAttendeeFromEvent(int eventId, int attendeeId)
        {
            ServiceResponse serviceResponse = new();

            Event? eventEntity = await _context.Events
                .Include(e => e.Attendees)
                .Where(e => e.EventId == eventId)
                .FirstOrDefaultAsync();
            Attendee? attendee = await _context.Attendees.FindAsync(attendeeId);

            if (eventEntity == null || attendee == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                if (eventEntity == null) serviceResponse.Messages.Add("Event was not found.");
                if (attendee == null) serviceResponse.Messages.Add("Attendee was not found.");
                return serviceResponse;
            }

            try
            {
                eventEntity.Attendees.Remove(attendee);
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("There was an issue unlinking the attendee from the event.");
                serviceResponse.Messages.Add(ex.Message);
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse> LinkArtistToEvent(int eventId, int artistId)
        {
            ServiceResponse serviceResponse = new();

            Event? eventEntity = await _context.Events
                .Include(e => e.Artists)
                .Where(e => e.EventId == eventId)
                .FirstOrDefaultAsync();
            Artist? artist = await _context.Artists.FindAsync(artistId);

            if (eventEntity == null || artist == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                if (eventEntity == null)
                {
                    serviceResponse.Messages.Add("Event was not found.");
                }
                if (artist == null)
                {
                    serviceResponse.Messages.Add("Artist was not found.");
                }
                return serviceResponse;
            }

            try
            {
                eventEntity.Artists.Add(artist);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Messages.Add("There was an issue linking the artist to the event.");
                serviceResponse.Messages.Add(ex.Message);
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                return serviceResponse;
            }

            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            return serviceResponse;
        }

        public async Task<ServiceResponse> UnlinkArtistFromEvent(int eventId, int artistId)
        {
            ServiceResponse serviceResponse = new();

            Event? eventEntity = await _context.Events
                .Include(e => e.Artists)
                .Where(e => e.EventId == eventId)
                .FirstOrDefaultAsync();
            Artist? artist = await _context.Artists.FindAsync(artistId);

            if (eventEntity == null || artist == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                if (eventEntity == null)
                {
                    serviceResponse.Messages.Add("Event was not found.");
                }
                if (artist == null)
                {
                    serviceResponse.Messages.Add("Artist was not found.");
                }
                return serviceResponse;
            }

            try
            {
                eventEntity.Artists.Remove(artist);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Messages.Add("There was an issue unlinking the artist from the event.");
                serviceResponse.Messages.Add(ex.Message);
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                return serviceResponse;
            }

            serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
            return serviceResponse;
        }

        public async Task<ServiceResponse> ListAttendeesForEvent(int eventId)
        {
            var response = new ServiceResponse();

            try
            {
                // Check if event exists
                var eventExists = await _context.Events.AnyAsync(e => e.EventId == eventId);
                if (!eventExists)
                {
                    response.Success = false;
                    response.Message = "Event not found.";
                    return response;
                }

                // Fetch attendees for the given event
                var attendees = await _context.Attendees
                    .Where(a => a.Events.Any(e => e.EventId == eventId)) // Ensure EF can translate this
                    .Select(a => new AttendeeDto
                    {
                        AttendeeId = a.AttendeeId,
                        FirstName = a.FirstName,
                        LastName = a.LastName,
                        Email = a.Email,
                        ContactNumber = a.ContactNumber,
                        EventIds = a.Events.Select(e => e.EventId).ToList(),
                        PurchaseIds = a.Purchases.Select(p => p.PurchaseId).ToList()
                    })
                    .ToListAsync(); // Forces execution at database level

                response.Success = true;
                response.Data = attendees;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }




        public async Task<ServiceResponse> ListArtistsForEvent(int eventId)
        {
            var response = new ServiceResponse();

            try
            {
                List<Artist> artists = await _context.Artists
                    .Where(a => a.Events.Any(e => e.EventId == eventId))
                    .ToListAsync();

                List<ArtistDto> artistDtos = artists.Select(a => new ArtistDto
                {
                    ArtistId = a.ArtistId,
                    FName = a.FName,
                    LName = a.LName,
                    Bio = a.Bio,
                    EmailId = a.EmailId,
                    PhoneNumber = a.PhoneNumber
                }).ToList();

                response.Success = true;
                response.Data = artistDtos;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }




    }
}
