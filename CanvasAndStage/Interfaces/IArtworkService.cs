using CanvasAndStage.Models;
using CanvasAndStage.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CanvasAndStage.Interfaces
{
    public interface IArtworkService
    {
        Task<IEnumerable<ArtworkDto>> ListArtworks();
        Task<ArtworkDto?> FindArtwork(int id);
        Task<ServiceResponse> UpdateArtwork(ArtworkDto artworkDto);
        Task<ServiceResponse> AddArtwork(ArtworkDto artworkDto);
        Task<ServiceResponse> DeleteArtwork(int id);
    }
}
