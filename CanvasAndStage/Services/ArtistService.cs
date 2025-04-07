using CanvasAndStage.Data;
using CanvasAndStage.Interfaces;
using CanvasAndStage.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanvasAndStage.Services
{
    public class ArtistService : IArtistService
    {
        private readonly ApplicationDbContext _context;

        public ArtistService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ArtistDto>> ListArtists()
        {
            var artists = await _context.Artists
                .Include(a => a.Artworks)
                .ToListAsync();

            if (!artists.Any()) return [];

            return artists.Select(a => new ArtistDto
            {
                ArtistId = a.ArtistId,
                FName = a.FName,
                LName = a.LName,
                Bio = a.Bio,
                EmailId = a.EmailId,
                PhoneNumber = a.PhoneNumber,
                TotalArtworks = a.Artworks.Count,

                Artworks = a.Artworks.Select(art => new ArtworkDto
                {
                    ArtworkId = art.ArtworkId,
                    Title = art.Title,
                    Description = art.Description,
                    Price = art.Price
                }).ToList()
            }).ToList();
        }


        public async Task<ArtistDto?> FindArtist(int id)
        {
            var artist = await _context.Artists
                .Include(a => a.Artworks)
                .FirstOrDefaultAsync(a => a.ArtistId == id);

            if (artist == null) return null;

            return new ArtistDto
            {
                ArtistId = artist.ArtistId,
                FName = artist.FName,
                LName = artist.LName,
                Bio = artist.Bio,
                EmailId = artist.EmailId,
                PhoneNumber = artist.PhoneNumber,
                TotalArtworks = artist.Artworks.Count,

              
                Artworks = artist.Artworks.Select(a => new ArtworkDto
                {
                    ArtworkId = a.ArtworkId,
                    Title = a.Title,
                    Description = a.Description,
                    Price = a.Price
                }).ToList()
            };
        }


        public async Task<ServiceResponse> AddArtist(AddArtistDto dto)
        {
            ServiceResponse response = new();

            Artist newArtist = new()
            {
                FName = dto.FName,
                LName = dto.LName,
                Bio = dto.Bio,
                EmailId = dto.EmailId,
                PhoneNumber = dto.PhoneNumber
            };

            try
            {
                _context.Artists.Add(newArtist);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error while adding the artist.");
                response.Messages.Add(ex.Message);
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Created;
            response.CreatedId = newArtist.ArtistId;
            return response;
        }

        public async Task<ServiceResponse> UpdateArtist(int id, UpdateArtistDto dto)
        {
            ServiceResponse response = new();

            if (id != dto.ArtistId)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Artist ID mismatch.");
                return response;
            }

            var artist = await _context.Artists.FindAsync(id);
            if (artist == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Artist not found.");
                return response;
            }

            artist.FName = dto.FName;
            artist.LName = dto.LName;
            artist.Bio = dto.Bio;
            artist.EmailId = dto.EmailId;
            artist.PhoneNumber = dto.PhoneNumber;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error while updating the artist.");
                response.Messages.Add(ex.Message);
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Updated;
            return response;
        }

        public async Task<ServiceResponse> DeleteArtist(int id)
        {
            ServiceResponse response = new();

            var artist = await _context.Artists.FindAsync(id);
            if (artist == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Artist not found.");
                return response;
            }

            try
            {
                _context.Artists.Remove(artist);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error encountered while deleting the artist.");
                response.Messages.Add(ex.Message);
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Deleted;
            return response;
        }
    }
}
