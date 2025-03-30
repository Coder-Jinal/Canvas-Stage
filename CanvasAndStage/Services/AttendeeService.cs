using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CanvasAndStage.Interfaces;
using CanvasAndStage.Models;
using CanvasAndStage.Data;
using Microsoft.EntityFrameworkCore;


namespace CanvasAndStage.Services
{
    public class AttendeeService : IAttendeeService
    {
        private readonly ApplicationDbContext _context;

        public AttendeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AttendeeDto>> ListAttendees()
        {
            var attendees = await _context.Attendees.Include(a => a.Events).Include(a => a.Purchases).ToListAsync();

            return attendees.Select(a => new AttendeeDto()
            {
                AttendeeId = a.AttendeeId,
                FirstName = a.FirstName,
                LastName = a.LastName,
                Email = a.Email,
                ContactNumber = a.ContactNumber,
                EventIds = a.Events.Select(e => e.EventId).ToList(),
                PurchaseIds = a.Purchases.Select(p => p.PurchaseId).ToList()
            }).ToList();
        }

        public async Task<AttendeeDto?> FindAttendee(int id)
        {
            var attendee = await _context.Attendees.Include(a => a.Events).Include(a => a.Purchases)
                .FirstOrDefaultAsync(a => a.AttendeeId == id);

            if (attendee == null) return null;

            return new AttendeeDto()
            {
                AttendeeId = attendee.AttendeeId,
                FirstName = attendee.FirstName,
                LastName = attendee.LastName,
                Email = attendee.Email,
                ContactNumber = attendee.ContactNumber,
                EventIds = attendee.Events.Select(e => e.EventId).ToList(),
                PurchaseIds = attendee.Purchases.Select(p => p.PurchaseId).ToList()
            };
        }

        public async Task<ServiceResponse> UpdateAttendee(AttendeeDto attendeeDto)
        {
            ServiceResponse response = new();

            var attendee = await _context.Attendees.FindAsync(attendeeDto.AttendeeId);
            if (attendee == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Attendee not found.");
                return response;
            }

            attendee.FirstName = attendeeDto.FirstName;
            attendee.LastName = attendeeDto.LastName;
            attendee.Email = attendeeDto.Email;
            attendee.ContactNumber = attendeeDto.ContactNumber;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error updating the attendee.");
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Updated;
            return response;
        }

        public async Task<ServiceResponse> AddAttendee(AttendeeDto attendeeDto)
        {
            ServiceResponse response = new();

            Attendee attendee = new()
            {
                FirstName = attendeeDto.FirstName,
                LastName = attendeeDto.LastName,
                Email = attendeeDto.Email,
                ContactNumber = attendeeDto.ContactNumber
            };

            try
            {
                await _context.Attendees.AddAsync(attendee);
                await _context.SaveChangesAsync();

                response.Status = ServiceResponse.ServiceStatus.Created;
                response.CreatedId = attendee.AttendeeId;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error adding the attendee.");
                response.Messages.Add(ex.Message);
                if (ex.InnerException != null)
                {
                    response.Messages.Add("Inner exception: " + ex.InnerException.Message);
                }
                return response;
            }

            return response;
        }

        public async Task<ServiceResponse> DeleteAttendee(int id)
        {
            ServiceResponse response = new();

            var attendee = await _context.Attendees.FindAsync(id);
            if (attendee == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Attendee not found.");
                return response;
            }

            try
            {
                _context.Attendees.Remove(attendee);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error deleting the attendee.");
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Deleted;
            return response;
        }

        public async Task<ServiceResponse> ListEventsForAttendee(int attendeeId)
        {
            var response = new ServiceResponse();

            try
            {
                // Check if attendee exists
                var attendeeExists = await _context.Attendees.AnyAsync(a => a.AttendeeId == attendeeId);
                if (!attendeeExists)
                {
                    response.Success = false;
                    response.Message = "Attendee not found.";
                    return response;
                }

                // Fetch events for the given attendee
                var events = await _context.Events
                    .Where(e => e.Attendees.Any(a => a.AttendeeId == attendeeId)) // Ensure EF can translate this
                    .Select(e => new EventDto
                    {
                        EventId = e.EventId,
                        Name = e.Name,
                        Description = e.Description,
                        Location = e.Location,
                        Date = e.Date
                    })
                    .ToListAsync(); // Forces execution at database level

                response.Success = true;
                response.Data = events;
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
