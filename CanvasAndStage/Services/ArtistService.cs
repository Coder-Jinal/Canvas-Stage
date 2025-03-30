using CanvasAndStage.Interfaces;
using CanvasAndStage.Models;
using CanvasAndStage.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
            List<Artist> artists = await _context.Artists.ToListAsync();
            List<ArtistDto> artistDtos = new();

            foreach (Artist artist in artists)
            {
                artistDtos.Add(new ArtistDto()
                {
                    ArtistId = artist.ArtistId,
                    FName = artist.FName,
                    LName = artist.LName,
                    Bio = artist.Bio,
                    EmailId = artist.EmailId,
                    PhoneNumber = artist.PhoneNumber
                });
            }

            return artistDtos;
        }

        public async Task<ArtistDto?> FindArtist(int id)
        {
            var artist = await _context.Artists.FirstOrDefaultAsync(a => a.ArtistId == id);

            if (artist == null) return null;

            return new ArtistDto()
            {
                ArtistId = artist.ArtistId,
                FName = artist.FName,
                LName = artist.LName,
                Bio = artist.Bio,
                EmailId = artist.EmailId,
                PhoneNumber = artist.PhoneNumber
            };
        }

        public async Task<ServiceResponse> UpdateArtist(ArtistDto artistDto)
        {
            ServiceResponse response = new();

            var artist = await _context.Artists.FindAsync(artistDto.ArtistId);

            if (artist == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Artist not found.");
                return response;
            }

            artist.FName = artistDto.FName;
            artist.LName = artistDto.LName;
            artist.Bio = artistDto.Bio;
            artist.EmailId = artistDto.EmailId;
            artist.PhoneNumber = artistDto.PhoneNumber;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error updating the artist.");
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Updated;
            return response;
        }

        public async Task<ServiceResponse> AddArtist(ArtistDto artistDto)
        {
            ServiceResponse response = new();

            Artist artist = new()
            {
                FName = artistDto.FName,
                LName = artistDto.LName,
                Bio = artistDto.Bio,
                EmailId = artistDto.EmailId,
                PhoneNumber = artistDto.PhoneNumber
            };

            try
            {
                await _context.Artists.AddAsync(artist);
                await _context.SaveChangesAsync();

                response.Status = ServiceResponse.ServiceStatus.Created;
                response.CreatedId = artist.ArtistId;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error adding the artist.");
                response.Messages.Add(ex.Message);
                return response;
            }

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
                response.Messages.Add("Error deleting the artist.");
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Deleted;
            return response;
        }

        public async Task<ServiceResponse> LinkArtworkToArtist(int artistId, int artworkId)
        {
            ServiceResponse serviceResponse = new();

            // Find the artist and the artwork
            Artist? artist = await _context.Artists
                .Include(a => a.Artworks)
                .Where(a => a.ArtistId == artistId)
                .FirstOrDefaultAsync();
            Artwork? artwork = await _context.Artworks.FindAsync(artworkId);

            if (artist == null || artwork == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                if (artist == null) serviceResponse.Messages.Add("Artist was not found.");
                if (artwork == null) serviceResponse.Messages.Add("Artwork was not found.");
                return serviceResponse;
            }

            try
            {
                // Link the artwork to the artist
                artist.Artworks.Add(artwork);
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("There was an issue linking the artwork to the artist.");
                serviceResponse.Messages.Add(ex.Message);
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse> UnlinkArtworkFromArtist(int artistId, int artworkId)
        {
            ServiceResponse serviceResponse = new();

            // Find the artist and the artwork
            Artist? artist = await _context.Artists
                .Include(a => a.Artworks)
                .Where(a => a.ArtistId == artistId)
                .FirstOrDefaultAsync();
            Artwork? artwork = await _context.Artworks.FindAsync(artworkId);

            if (artist == null || artwork == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                if (artist == null) serviceResponse.Messages.Add("Artist was not found.");
                if (artwork == null) serviceResponse.Messages.Add("Artwork was not found.");
                return serviceResponse;
            }

            try
            {
                // Unlink the artwork from the artist
                artist.Artworks.Remove(artwork);
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("There was an issue unlinking the artwork from the artist.");
                serviceResponse.Messages.Add(ex.Message);
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse> ListArtworksForArtist(int artistId)
        {
            var response = new ServiceResponse();

            try
            {
                // Check if artist exists
                var artistExists = await _context.Artists.AnyAsync(a => a.ArtistId == artistId);
                if (!artistExists)
                {
                    response.Success = false;
                    response.Message = "Artist not found.";
                    return response;
                }

                // Fetch artworks for the given artist
                var artworks = await _context.Artworks
                    .Where(a => a.ArtistId == artistId) // Filter artworks by artistId
                    .Select(a => new ArtworkDto
                    {
                        ArtworkId = a.ArtworkId,
                        Title = a.Title,
                        Description = a.Description,
                        Date = a.Date,
                        Price = a.Price,
                        ArtistId = a.ArtistId
                    })
                    .ToListAsync(); // Execute at database level

                response.Success = true;
                response.Data = artworks;
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
