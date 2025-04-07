using CanvasAndStage.Data;
using CanvasAndStage.Interfaces;
using CanvasAndStage.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanvasAndStage.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly ApplicationDbContext _context;

        public PurchaseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PurchaseDto>> ListPurchases()
        {
            var purchases = await _context.Purchases
                .Include(p => p.Artwork)
                    .ThenInclude(a => a.Artist)
                .Include(p => p.Event)
                .Include(p => p.Attendee)
                .ToListAsync();

            if (!purchases.Any()) return [];

            return purchases.Select(p => new PurchaseDto
            {
                PurchaseId = p.PurchaseId,
                TotalPrice = p.TotalPrice,
                ArtworkName = p.Artwork.Title,
                ArtistName = $"{p.Artwork.Artist.FName} {p.Artwork.Artist.LName}",
                AttendeeName = $"{p.Attendee.FirstName} {p.Attendee.LastName}",
                EventName = p.Event.Name
            }).ToList();
        }

        public async Task<PurchaseDto?> FindPurchase(int id)
        {
            var purchase = await _context.Purchases
                .Include(p => p.Artwork)
                    .ThenInclude(a => a.Artist)
                .Include(p => p.Event)
                .Include(p => p.Attendee)
                .FirstOrDefaultAsync(p => p.PurchaseId == id);

            if (purchase == null) return null;

            return new PurchaseDto
            {
                PurchaseId = purchase.PurchaseId,
                TotalPrice = purchase.TotalPrice,
                ArtworkName = purchase.Artwork.Title,
                ArtistName = $"{purchase.Artwork.Artist.FName} {purchase.Artwork.Artist.LName}",
                AttendeeName = $"{purchase.Attendee.FirstName} {purchase.Attendee.LastName}",
                EventName = purchase.Event.Name
            };
        }

        public async Task<ServiceResponse> AddPurchase(AddPurchaseDto dto)
        {
            ServiceResponse response = new();

            var attendee = await _context.Attendees.FindAsync(dto.AttendeeId);
            var artwork = await _context.Artworks.Include(a => a.Artist).FirstOrDefaultAsync(a => a.ArtworkId == dto.ArtworkId);
            var eventDetails = await _context.Events.FindAsync(dto.EventId);

            if (attendee == null || artwork == null || eventDetails == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Attendee, Artwork, or Event not found.");
                return response;
            }

            if (dto.TotalPrice <= 0)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Total price must be greater than zero.");
                return response;
            }

            Purchase newPurchase = new()
            {
                AttendeeId = dto.AttendeeId,
                ArtworkId = dto.ArtworkId,
                EventId = dto.EventId,
                TotalPrice = dto.TotalPrice
            };

            try
            {
                _context.Purchases.Add(newPurchase);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error while adding the purchase.");
                response.Messages.Add(ex.Message);
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Created;
            response.CreatedId = newPurchase.PurchaseId;
            return response;
        }

        public async Task<ServiceResponse> UpdatePurchase(int id, UpdatePurchaseDto dto)
        {
            ServiceResponse response = new();

            if (id != dto.PurchaseId)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Purchase ID mismatch.");
                return response;
            }

            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Purchase not found.");
                return response;
            }

            purchase.AttendeeId = dto.AttendeeId;
            purchase.ArtworkId = dto.ArtworkId;
            purchase.EventId = dto.EventId;
            purchase.TotalPrice = dto.TotalPrice;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error while updating the purchase.");
                response.Messages.Add(ex.Message);
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Updated;
            return response;
        }

        public async Task<ServiceResponse> DeletePurchase(int id)
        {
            ServiceResponse response = new();

            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Purchase not found.");
                return response;
            }

            try
            {
                _context.Purchases.Remove(purchase);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error encountered while deleting the purchase.");
                response.Messages.Add(ex.Message);
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Deleted;
            return response;
        }
    }
}
