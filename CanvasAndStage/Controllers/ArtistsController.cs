using CanvasAndStage.Interfaces;
using CanvasAndStage.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CanvasAndStage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArtistsController : ControllerBase
    {
        private readonly IArtistService _artistService;

        public ArtistsController(IArtistService artistService)
        {
            _artistService = artistService;
        }

        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<ArtistDto>>> ListArtists()
        {
            var artistDtos = await _artistService.ListArtists();
            return Ok(artistDtos);
        }

        [HttpGet("Find/{id}")]
        public async Task<ActionResult<ArtistDto>> FindArtist(int id)
        {
            var artist = await _artistService.FindArtist(id);

            if (artist == null)
            {
                return NotFound($"Artist with ID {id} doesn't exist.");
            }

            return Ok(artist);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddArtist(AddArtistDto dto)
        {
            ServiceResponse response = await _artistService.AddArtist(dto);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, new { error = "An error occurred while adding the artist." });
            }

            return CreatedAtAction("FindArtist", new { id = response.CreatedId }, new
            {
                message = $"Artist added successfully with ID {response.CreatedId}",
                artistId = response.CreatedId
            });
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateArtist(int id, UpdateArtistDto dto)
        {
            if (id != dto.ArtistId)
            {
                return BadRequest(new { message = "Artist ID mismatch." });
            }

            ServiceResponse response = await _artistService.UpdateArtist(id, dto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(new { error = "Artist not found." });
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, new { error = "An error occurred while updating the artist." });
            }

            return Ok(new { message = $"Artist with ID {id} updated successfully." });
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteArtist(int id)
        {
            ServiceResponse response = await _artistService.DeleteArtist(id);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(new { error = "Artist not found." });
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, new { error = "An error occurred while deleting the artist." });
            }

            return Ok(new { message = $"Artist with ID {id} deleted successfully." });
        }
    }
}
