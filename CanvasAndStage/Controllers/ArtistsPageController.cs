using CanvasAndStage.Interfaces;
using CanvasAndStage.Models;
using CanvasAndStage.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> List(int page = 1, int pageSize = 5)
        {
            var result = await _artistService.GetPaginatedArtists(page, pageSize);
            return View(result);
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
