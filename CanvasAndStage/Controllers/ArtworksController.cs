using CanvasAndStage.Interfaces;
using CanvasAndStage.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CanvasAndStage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArtworksController : ControllerBase
    {
        private readonly IArtworkService _artworkService;

        public ArtworksController(IArtworkService artworkService)
        {
            _artworkService = artworkService;
        }

        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<ArtworkDto>>> ListArtworks()
        {
            var artworkDtos = await _artworkService.ListArtworks();
            return Ok(artworkDtos);
        }

        [HttpGet("Find/{id}")]
        public async Task<ActionResult<ArtworkDto>> FindArtwork(int id)
        {
            var artwork = await _artworkService.FindArtwork(id);

            if (artwork == null)
            {
                return NotFound(new { error = $"Artwork with ID {id} doesn't exist." });
            }

            return Ok(artwork);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddArtwork([FromBody] AddArtworkDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "Invalid input data." });
            }

            ServiceResponse response = await _artworkService.AddArtwork(dto);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, new { error = "An error occurred while adding the artwork." });
            }

            return CreatedAtAction(nameof(FindArtwork), new { id = response.CreatedId }, new
            {
                message = $"Artwork added successfully with ID {response.CreatedId}",
                artworkId = response.CreatedId
            });
        }


        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateArtwork(int id, [FromBody] UpdateArtworkDto dto)
        {
            if (id != dto.ArtworkId)
            {
                return BadRequest(new { error = "Artwork ID mismatch." });
            }

            ServiceResponse response = await _artworkService.UpdateArtwork(id, dto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(new { error = "Artwork not found." });
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, new { error = "An error occurred while updating the artwork." });
            }

            return Ok(new { message = $"Artwork with ID {id} updated successfully." });
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteArtwork(int id)
        {
            ServiceResponse response = await _artworkService.DeleteArtwork(id);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(new { error = "Artwork not found." });
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, new { error = "An error occurred while deleting the artwork." });
            }

            return Ok(new { message = $"Artwork with ID {id} deleted successfully." });
        }
    }
}
