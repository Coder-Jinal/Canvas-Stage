using CanvasAndStage.Data;
using CanvasAndStage.Interfaces;
using CanvasAndStage.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var attendees = await _context.Attendees
                .Include(a => a.Events)
                .Include(a => a.Purchases)
                    .ThenInclude(p => p.Artwork)
                .ToListAsync();

            if (!attendees.Any()) return [];

            return attendees.Select(a => new AttendeeDto
            {
                AttendeeId = a.AttendeeId,
                FirstName = a.FirstName,
                LastName = a.LastName,
                Email = a.Email,
                ContactNumber = a.ContactNumber,
                EventNames = a.Events.Select(e => e.Name).ToList(),
                TotalEvents = a.Events.Count,
                TotalPurchase = a.Purchases.Sum(p => p.TotalPrice),
                TotalArtworksPurchased = a.Purchases.Count,
                ArtworkNames = a.Purchases.Select(p => p.Artwork.Title).Distinct().ToList()
            }).ToList();
        }

        public async Task<AttendeeDto?> FindAttendee(int id)
        {
            var attendee = await _context.Attendees
                .Include(a => a.Events)
                .Include(a => a.Purchases)
                    .ThenInclude(p => p.Artwork)
                .FirstOrDefaultAsync(a => a.AttendeeId == id);

            if (attendee == null) return null;

            return new AttendeeDto
            {
                AttendeeId = attendee.AttendeeId,
                FirstName = attendee.FirstName,
                LastName = attendee.LastName,
                Email = attendee.Email,
                ContactNumber = attendee.ContactNumber,
                EventNames = attendee.Events.Select(e => e.Name).ToList(),
                TotalEvents = attendee.Events.Count,
                TotalPurchase = attendee.Purchases.Sum(p => p.TotalPrice),
                TotalArtworksPurchased = attendee.Purchases.Count,
                ArtworkNames = attendee.Purchases.Select(p => p.Artwork.Title).Distinct().ToList()
            };
        }

        public async Task<ServiceResponse> AddAttendee(AddAttendeeDto dto)
        {
            ServiceResponse response = new();

            if (_context.Attendees.Any(a => a.Email == dto.Email))
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("An attendee with this email already exists.");
                return response;
            }

            Attendee newAttendee = new()
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                ContactNumber = dto.ContactNumber
            };

            try
            {
                _context.Attendees.Add(newAttendee);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error while adding the attendee.");
                response.Messages.Add(ex.Message);
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Created;
            response.CreatedId = newAttendee.AttendeeId;
            return response;
        }

        public async Task<ServiceResponse> UpdateAttendee(int id, UpdateAttendeeDto dto)
        {
            ServiceResponse response = new();

            if (id != dto.AttendeeId)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Attendee ID mismatch.");
                return response;
            }

            var attendee = await _context.Attendees.FindAsync(id);
            if (attendee == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Attendee not found.");
                return response;
            }

            attendee.FirstName = dto.FirstName;
            attendee.LastName = dto.LastName;
            attendee.Email = dto.Email;
            attendee.ContactNumber = dto.ContactNumber;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error while updating the attendee.");
                response.Messages.Add(ex.Message);
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Updated;
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
                response.Messages.Add("Error encountered while deleting the attendee.");
                response.Messages.Add(ex.Message);
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Deleted;
            return response;
        }
    }
}
