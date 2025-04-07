using CanvasAndStage.Interfaces;
using CanvasAndStage.Models;
using CanvasAndStage.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CanvasAndStage.Controllers
{
    [Route("ArtistsPage")]
    public class ArtistsPageController : Controller
    {
        private readonly IArtistService _artistService;

        public ArtistsPageController(IArtistService artistService)
        {
            _artistService = artistService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        [HttpGet("List")]
        public async Task<IActionResult> List()
        {
            IEnumerable<ArtistDto> artists = await _artistService.ListArtists();
            return View(artists);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var artist = await _artistService.FindArtist(id);
            if (artist == null)
            {
                return View("Error", new ErrorViewModel { Errors = ["Artist not found."] });
            }

            return View(artist);
        }

        [HttpGet("Add")]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost("Add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddArtistDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _artistService.AddArtist(dto);

            if (result.Status == ServiceResponse.ServiceStatus.Error)
            {
                return View("Error", new ErrorViewModel { Errors = result.Messages });
            }

            return RedirectToAction("List");
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var artist = await _artistService.FindArtist(id);

            if (artist == null)
            {
                return View("Error", new ErrorViewModel { Errors = ["Artist not found."] });
            }

            var dto = new UpdateArtistDto
            {
                ArtistId = artist.ArtistId,
                FName = artist.FName,
                LName = artist.LName,
                Bio = artist.Bio,
                EmailId = artist.EmailId,
                PhoneNumber = artist.PhoneNumber
            };

            return View(dto);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateArtistDto dto)
        {
            if (id != dto.ArtistId)
            {
                return View("Error", new ErrorViewModel { Errors = ["Artist ID mismatch."] });
            }

            if (!ModelState.IsValid)
                return View(dto);

            var result = await _artistService.UpdateArtist(id, dto);

            if (result.Status == ServiceResponse.ServiceStatus.Error)
            {
                return View("Error", new ErrorViewModel { Errors = result.Messages });
            }

            return RedirectToAction("Details", new { id });
        }

        [HttpGet("ConfirmDelete/{id}")]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var artist = await _artistService.FindArtist(id);

            if (artist == null)
            {
                return View("Error", new ErrorViewModel { Errors = ["Artist not found."] });
            }

            return View(artist);
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _artistService.DeleteArtist(id);

            if (result.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List");
            }
            else
            {
                return View("Error", new ErrorViewModel { Errors = result.Messages });
            }
        }
    }
}
