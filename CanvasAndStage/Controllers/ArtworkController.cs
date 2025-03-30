using Microsoft.AspNetCore.Mvc;
using CanvasAndStage.Interfaces;
using CanvasAndStage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace CanvasAndStage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtworkController : Controller
    {
        private readonly IArtworkService _artworkService;

        public ArtworkController(IArtworkService artworkService)
        {
            _artworkService = artworkService;
        }

        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<ArtworkDto>>> ListArtworks()
        {
            IEnumerable<ArtworkDto> artworkDtos = await _artworkService.ListArtworks();
            return Ok(artworkDtos);
        }

        [HttpGet("Find/{id}")]
        public async Task<ActionResult<ArtworkDto>> FindArtwork(int id)
        {
            var artwork = await _artworkService.FindArtwork(id);
            if (artwork == null) return NotFound();
            return Ok(artwork);
        }

        [HttpPut("Update/{id}")]
        public async Task<ActionResult> UpdateArtwork(int id, ArtworkDto artworkDto)
        {
            if (id != artworkDto.ArtworkId) return BadRequest("Artwork ID mismatch.");

            ServiceResponse response = await _artworkService.UpdateArtwork(artworkDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
                return NotFound(response.Messages);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, response.Messages);

            return NoContent();
        }

        [HttpPost("Add")]
        public async Task<ActionResult<ArtworkDto>> AddArtwork(ArtworkDto artworkDto)
        {
            ServiceResponse response = await _artworkService.AddArtwork(artworkDto);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, response.Messages);

            artworkDto.ArtworkId = (int)response.CreatedId;
            return CreatedAtAction(nameof(FindArtwork), new { id = artworkDto.ArtworkId }, artworkDto);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> DeleteArtwork(int id)
        {
            ServiceResponse response = await _artworkService.DeleteArtwork(id);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound) return NotFound();
            if (response.Status == ServiceResponse.ServiceStatus.Error) return StatusCode(500, response.Messages);

            return NoContent();
        }
    }
}
