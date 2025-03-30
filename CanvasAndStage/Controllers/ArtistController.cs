using Microsoft.AspNetCore.Mvc;
using CanvasAndStage.Interfaces;
using CanvasAndStage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CanvasAndStage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistController : Controller
    {
        private readonly IArtistService _artistService;

        public ArtistController(IArtistService artistService)
        {
            _artistService = artistService;
        }

        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<ArtistDto>>> ListArtists()
        {
            IEnumerable<ArtistDto> artistDtos = await _artistService.ListArtists();
            return Ok(artistDtos);
        }

        [HttpGet("Find/{id}")]
        public async Task<ActionResult<ArtistDto>> FindArtist(int id)
        {
            var artist = await _artistService.FindArtist(id);
            if (artist == null) return NotFound();
            return Ok(artist);
        }

        [HttpPut("Update/{id}")]
        //[Authorize(Roles = "admin")]
        public async Task<ActionResult> UpdateArtist(int id, ArtistDto artistDto)
        {
            if (id != artistDto.ArtistId) return BadRequest("Artist ID mismatch.");

            ServiceResponse response = await _artistService.UpdateArtist(artistDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
                return NotFound(response.Messages);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, response.Messages);

            return NoContent();
        }

        [HttpPost("Add")]
        //[Authorize(Roles = "admin")]
        public async Task<ActionResult<ArtistDto>> AddArtist(ArtistDto artistDto)
        {
            ServiceResponse response = await _artistService.AddArtist(artistDto);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, response.Messages);

            artistDto.ArtistId = (int)response.CreatedId;
            return CreatedAtAction(nameof(FindArtist), new { id = artistDto.ArtistId }, artistDto);
        }

        [HttpDelete("Delete/{id}")]
        //[Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteArtist(int id)
        {
            ServiceResponse response = await _artistService.DeleteArtist(id);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound) return NotFound();
            if (response.Status == ServiceResponse.ServiceStatus.Error) return StatusCode(500, response.Messages);

            return NoContent();
        }

        [HttpPost("LinkArtwork")]
        public async Task<ActionResult> LinkArtwork(int artistId, int artworkId)
        {
            ServiceResponse response = await _artistService.LinkArtworkToArtist(artistId, artworkId);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return NoContent();
        }

        //need improvement here when i try to unlink, it deletes artwork.

        [HttpDelete("UnlinkArtwork")]
        public async Task<ActionResult> UnlinkArtwork(int artistId, int artworkId)
        {
            ServiceResponse response = await _artistService.UnlinkArtworkFromArtist(artistId, artworkId);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return NoContent();
        }

        [HttpGet("ListArtworksForArtist/{artistId}")]
        public async Task<IActionResult> ListArtworksForArtist(int artistId)
        {
            var response = await _artistService.ListArtworksForArtist(artistId);

            if (!response.Success)
                return BadRequest(response.Message);

            return Ok(response.Data);
        }

    }
}
