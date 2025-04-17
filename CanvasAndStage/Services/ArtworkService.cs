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
    public class ArtworkService : IArtworkService
    {
        private readonly ApplicationDbContext _context;

        public ArtworkService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ArtworkDto>> ListArtworks()
        {
            var artworks = await _context.Artworks
                .Include(a => a.Artist)
                .Include(a => a.Purchases)
                .ThenInclude(p => p.Attendee)
                .ToListAsync();

            if (!artworks.Any()) return [];

            return artworks.Select(a => new ArtworkDto
            {
                ArtworkId = a.ArtworkId,
                Title = a.Title,
                Description = a.Description,
                Date = a.Date,
                Price = a.Price,
                TotalPriceWithTax = a.Price * 1.10f, // Example: Adding 10% tax
                ArtistName = $"{a.Artist.FName} {a.Artist.LName}",
                TimesPurchased = a.Purchases.Count,
                AttendeePurchased = a.Purchases.Select(p => $"{p.Attendee.FirstName} {p.Attendee.LastName}").ToList()
            }).ToList();
        }

        public async Task<ArtworkDto?> FindArtwork(int id)
        {
            var artwork = await _context.Artworks
                .Include(a => a.Artist)
                .Include(a => a.Purchases)
                .ThenInclude(p => p.Attendee)
                .FirstOrDefaultAsync(a => a.ArtworkId == id);

            if (artwork == null) return null;

            return new ArtworkDto
            {
                ArtworkId = artwork.ArtworkId,
                Title = artwork.Title,
                Description = artwork.Description,
                Date = artwork.Date,
                Price = artwork.Price,
                TotalPriceWithTax = artwork.Price * 1.10f, // Example: Adding 10% tax
                ArtistName = $"{artwork.Artist.FName} {artwork.Artist.LName}",
                TimesPurchased = artwork.Purchases.Count,
                AttendeePurchased = artwork.Purchases.Select(p => $"{p.Attendee.FirstName} {p.Attendee.LastName}").ToList()
            };
        }

        public async Task<ServiceResponse> AddArtwork(AddArtworkDto dto)
        {
            ServiceResponse response = new();

            var artist = await _context.Artists.FindAsync(dto.ArtistId);
            if (artist == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Artist not found.");
                return response;
            }

            Artwork newArtwork = new()
            {
                Title = dto.Title,
                Description = dto.Description,
                Date = dto.Date,
                Price = dto.Price,
                ArtistId = dto.ArtistId
            };

            try
            {
                _context.Artworks.Add(newArtwork);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error while adding the artwork.");
                response.Messages.Add(ex.Message);
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Created;
            response.CreatedId = newArtwork.ArtworkId;
            return response;
        }


        public async Task<ServiceResponse> UpdateArtwork(int id, UpdateArtworkDto dto)
        {
            ServiceResponse response = new();

            if (id != dto.ArtworkId)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Artwork ID mismatch.");
                return response;
            }

            var artwork = await _context.Artworks.FindAsync(id);
            if (artwork == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Artwork not found.");
                return response;
            }

            var artist = await _context.Artists.FindAsync(dto.ArtistId);
            if (artist == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Selected artist not found.");
                return response;
            }

            artwork.Title = dto.Title;
            artwork.Description = dto.Description;
            artwork.Date = dto.Date;
            artwork.Price = dto.Price;
            artwork.ArtistId = dto.ArtistId; 
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error while updating the artwork.");
                response.Messages.Add(ex.Message);
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Updated;
            return response;
        }


        public async Task<ServiceResponse> DeleteArtwork(int id)
        {
            ServiceResponse response = new();

            var artwork = await _context.Artworks.FindAsync(id);
            if (artwork == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Artwork not found.");
                return response;
            }

            try
            {
                _context.Artworks.Remove(artwork);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error encountered while deleting the artwork.");
                response.Messages.Add(ex.Message);
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Deleted;
            return response;
        }

        public async Task<PaginatedResult<ArtworkDto>> GetPaginatedArtworks(int page, int pageSize)
        {
            var query = _context.Artworks
                .Include(a => a.Artist)
                .Include(a => a.Purchases)
                .ThenInclude(p => p.Attendee)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var pagedArtworks = await query
                .OrderBy(a => a.Title) 
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = pagedArtworks.Select(a => new ArtworkDto
            {
                ArtworkId = a.ArtworkId,
                Title = a.Title,
                Description = a.Description,
                Date = a.Date,
                Price = a.Price,
                TotalPriceWithTax = a.Price * 1.10f,
                ArtistName = $"{a.Artist.FName} {a.Artist.LName}",
                TimesPurchased = a.Purchases.Count,
                AttendeePurchased = a.Purchases.Select(p => $"{p.Attendee.FirstName} {p.Attendee.LastName}").ToList()
            }).ToList();

            return new PaginatedResult<ArtworkDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

    }
}
