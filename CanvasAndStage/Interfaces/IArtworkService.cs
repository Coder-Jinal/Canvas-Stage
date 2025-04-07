using CanvasAndStage.Models;

public interface IArtworkService
{
    Task<IEnumerable<ArtworkDto>> ListArtworks();
    Task<ArtworkDto?> FindArtwork(int id);
    Task<ServiceResponse> AddArtwork(AddArtworkDto dto);  
    Task<ServiceResponse> UpdateArtwork(int id, UpdateArtworkDto dto);
    Task<ServiceResponse> DeleteArtwork(int id);
}
