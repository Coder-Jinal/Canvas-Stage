using CanvasAndStage.Models;
using CanvasAndStage.Services;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace CanvasAndStage.Interfaces
{
    public interface IArtistService
    {
        Task<IEnumerable<ArtistDto>> ListArtists();
        Task<ArtistDto?> FindArtist(int id);
        Task<ServiceResponse> UpdateArtist(ArtistDto artistDto);
        Task<ServiceResponse> AddArtist(ArtistDto artistDto);
        Task<ServiceResponse> DeleteArtist(int id);
        Task<ServiceResponse> LinkArtworkToArtist(int artworkid, int artistid);
        Task<ServiceResponse> UnlinkArtworkFromArtist(int artworkid, int artistid);
        Task<ServiceResponse> ListArtworksForArtist(int artistId);
    }
}
