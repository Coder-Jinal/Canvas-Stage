using CanvasAndStage.Data;
using CanvasAndStage.Interfaces;
using CanvasAndStage.Models;
using CanvasAndStage.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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
            var events = await _context.Events
                .Include(e => e.Attendees)
                .Include(e => e.Artists)
                .Include(e => e.Purchases)
                .ToListAsync();

            if (events.Count == 0) return [];

            return events.Select(e => new EventDto
            {
                EventId = e.EventId,
                Name = e.Name,
                Location = e.Location,
                Description = e.Description,
                Date = e.Date,
                TotalAttendees = e.Attendees.Count,
                AttendeeNames = e.Attendees.Select(a => a.FirstName + " " + a.LastName).ToList(),
                TotalArtists = e.Artists.Count,
                ArtistNames = e.Artists.Select(a => a.FName + " " + a.LName).ToList(),
                TotalPurchase = e.Purchases.Sum(p => p.TotalPrice)
            }).ToList();
        }

        public async Task<EventDto?> FindEvent(int id)
        {
            var targetEvent = await _context.Events
                .Include(e => e.Attendees)
                .Include(e => e.Artists)
                    .ThenInclude(a => a.Artworks)
                .Include(e => e.Purchases)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (targetEvent == null) return null;

            return new EventDto
            {
                EventId = targetEvent.EventId,
                Name = targetEvent.Name,
                Location = targetEvent.Location,
                Description = targetEvent.Description,
                Date = targetEvent.Date,
                TotalAttendees = targetEvent.Attendees.Count,
                AttendeeNames = targetEvent.Attendees.Select(a => a.FirstName + " " + a.LastName).ToList(),
                TotalArtists = targetEvent.Artists.Count,
                ArtistNames = targetEvent.Artists.Select(a => a.FName + " " + a.LName).ToList(),
                TotalPurchase = targetEvent.Purchases.Sum(p => p.TotalPrice)
            };
        }



        public async Task<ServiceResponse> AddEvent(AddEventDto dto)
        {
            ServiceResponse response = new();

            Event newEvent = new()
            {
                Name = dto.Name,
                Location = dto.Location,
                Description = dto.Description,
                Date = dto.Date
            };

            try
            {
                _context.Events.Add(newEvent);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("An error occurred while adding the event.");
                response.Messages.Add(ex.Message);
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Created;
            response.CreatedId = newEvent.EventId;
            return response;
        }

        public async Task<ServiceResponse> UpdateEvent(int id, UpdateEventDto dto)
        {
            ServiceResponse response = new();

            if (id != dto.EventId)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Event ID mismatch.");
                return response;
            }

            var eventToUpdate = await _context.Events.FindAsync(id);
            if (eventToUpdate == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Event not found.");
                return response;
            }

            eventToUpdate.Name = dto.Name;
            eventToUpdate.Location = dto.Location;
            eventToUpdate.Description = dto.Description;
            eventToUpdate.Date = dto.Date;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("An error occurred while updating the event.");
                response.Messages.Add(ex.Message);
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Updated;
            return response;
        }

        public async Task<ServiceResponse> DeleteEvent(int id)
        {
            ServiceResponse response = new();

            var eventToDelete = await _context.Events.FindAsync(id);
            if (eventToDelete == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Event not found.");
                return response;
            }

            try
            {
                _context.Events.Remove(eventToDelete);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error encountered while deleting the event.");
                response.Messages.Add(ex.Message);
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Deleted;
            return response;
        }



        public async Task<ServiceResponse> LinkAttendee(int eventId, int attendeeId)
        {
            ServiceResponse response = new();

            var eventEntity = await _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            var attendee = await _context.Attendees.FindAsync(attendeeId);

            if (eventEntity == null || attendee == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Event or Attendee not found.");
                return response;
            }

            if (eventEntity.Attendees.Any(a => a.AttendeeId == attendeeId))
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Attendee is already linked to this event.");
                return response;
            }

            eventEntity.Attendees.Add(attendee);

            try
            {
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Success;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("An error occurred while linking the attendee.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        public async Task<ServiceResponse> UnlinkAttendee(int eventId, int attendeeId)
        {
            ServiceResponse response = new();

            var eventEntity = await _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (eventEntity == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Event not found.");
                return response;
            }

            var attendee = eventEntity.Attendees.FirstOrDefault(a => a.AttendeeId == attendeeId);
            if (attendee == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Attendee not linked to this event.");
                return response;
            }

            eventEntity.Attendees.Remove(attendee);

            try
            {
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Success;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("An error occurred while unlinking the attendee.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }


        public async Task<ServiceResponse> LinkArtist(int eventId, int artistId)
        {
            ServiceResponse response = new();

            var eventEntity = await _context.Events
                .Include(e => e.Artists)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            var artist = await _context.Artists.FindAsync(artistId);

            if (eventEntity == null || artist == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Event or Artist not found.");
                return response;
            }

            if (eventEntity.Artists.Any(a => a.ArtistId == artistId))
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Artist is already linked to this event.");
                return response;
            }

            eventEntity.Artists.Add(artist);

            try
            {
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Success;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("An error occurred while linking the artist.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        public async Task<ServiceResponse> UnlinkArtist(int eventId, int artistId)
        {
            ServiceResponse response = new();

            var eventEntity = await _context.Events
                .Include(e => e.Artists)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (eventEntity == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Event not found.");
                return response;
            }

            var artist = eventEntity.Artists.FirstOrDefault(a => a.ArtistId == artistId);
            if (artist == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Artist not linked to this event.");
                return response;
            }

            eventEntity.Artists.Remove(artist);

            try
            {
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Success;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("An error occurred while unlinking the artist.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }


    }
}
