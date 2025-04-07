using CanvasAndStage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CanvasAndStage.Interfaces
{
    public interface IArtistService
    {
        Task<IEnumerable<ArtistDto>> ListArtists();
        Task<ArtistDto?> FindArtist(int id);
        Task<ServiceResponse> AddArtist(AddArtistDto dto);
        Task<ServiceResponse> UpdateArtist(int id, UpdateArtistDto dto);
        Task<ServiceResponse> DeleteArtist(int id);
    }
}
