using CanvasAndStage.Data;
using CanvasAndStage.Interfaces;
using CanvasAndStage.Models;
using Microsoft.EntityFrameworkCore;

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
            List<Artwork> artworks = await _context.Artworks.Include(a => a.Artist).ToListAsync();
            List<ArtworkDto> artworkDtos = new();

            foreach (Artwork artwork in artworks)
            {
                artworkDtos.Add(new ArtworkDto()
                {
                    ArtworkId = artwork.ArtworkId,
                    Title = artwork.Title,
                    Description = artwork.Description,
                    Date = artwork.Date,
                    Price = artwork.Price,
                    ArtistId = artwork.ArtistId
                    //FName = artwork.Artist.FName
                });
            }

            return artworkDtos;
        }

        public async Task<ArtworkDto?> FindArtwork(int id)
        {
            var artwork = await _context.Artworks.Include(a => a.Artist)
                                                 .FirstOrDefaultAsync(a => a.ArtworkId == id);

            if (artwork == null) return null;

            return new ArtworkDto()
            {
                ArtworkId = artwork.ArtworkId,
                Title = artwork.Title,
                Description = artwork.Description,
                Date = artwork.Date,
                Price = artwork.Price,
                ArtistId = artwork.ArtistId
                //FName = artwork.Artist.FName
            };
        }

        public async Task<ServiceResponse> UpdateArtwork(ArtworkDto artworkDto)
        {
            ServiceResponse response = new();

            var artwork = await _context.Artworks.FindAsync(artworkDto.ArtworkId);

            if (artwork == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Artwork not found.");
                return response;
            }

            artwork.Title = artworkDto.Title;
            artwork.Description = artworkDto.Description;
            artwork.Date = artworkDto.Date;
            artwork.Price = artworkDto.Price;
            artwork.ArtistId = artworkDto.ArtistId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error updating the artwork.");
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Updated;
            return response;
        }

        public async Task<ServiceResponse> AddArtwork(ArtworkDto artworkDto)
        {
            ServiceResponse response = new();

            Artwork artwork = new()
            {
                Title = artworkDto.Title,
                Description = artworkDto.Description,
                Date = artworkDto.Date,
                Price = artworkDto.Price,
                ArtistId = artworkDto.ArtistId
            };

            try
            {
                await _context.Artworks.AddAsync(artwork);
                await _context.SaveChangesAsync();

                response.Status = ServiceResponse.ServiceStatus.Created;
                response.CreatedId = artwork.ArtworkId;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error adding the artwork.");
                response.Messages.Add(ex.Message);
                if (ex.InnerException != null)
                {
                    response.Messages.Add("Inner exception: " + ex.InnerException.Message);
                }
                return response;
            }

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
                response.Messages.Add("Error deleting the artwork.");
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Deleted;
            return response;
        }
    }
}
