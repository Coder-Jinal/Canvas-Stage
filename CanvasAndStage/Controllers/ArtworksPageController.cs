using CanvasAndStage.Interfaces;
using CanvasAndStage.Models;
using CanvasAndStage.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CanvasAndStage.Controllers
{
    [Route("ArtworksPage")]
    public class ArtworksPageController : Controller
    {
        private readonly IArtworkService _artworkService;
        private readonly IArtistService _artistService;

        public ArtworksPageController(IArtworkService artworkService, IArtistService artistService)
        {
            _artworkService = artworkService;
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
            var result = await _artworkService.GetPaginatedArtworks(page, pageSize);
            return View(result);
        }


        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var artwork = await _artworkService.FindArtwork(id);
            if (artwork == null)
            {
                return View("Error", new ErrorViewModel { Errors = ["Artwork not found."] });
            }

            return View(artwork);
        }

        [HttpGet("Add")]
        public async Task<IActionResult> Add()
        {
            var artists = await _artistService.ListArtists();
            ViewBag.Artists = artists;
            return View();
        }

        [HttpPost("Add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddArtworkDto dto)
        {
            if (!ModelState.IsValid)
            {
                var artists = await _artistService.ListArtists();
                ViewBag.Artists = artists;
                return View(dto);
            }

            var result = await _artworkService.AddArtwork(dto); 

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
            var artwork = await _artworkService.FindArtwork(id);

            if (artwork == null)
            {
                return View("Error", new ErrorViewModel { Errors = ["Artwork not found."] });
            }

            var artists = await _artistService.ListArtists(); 
            ViewBag.Artists = artists;

            var dto = new UpdateArtworkDto
            {
                ArtworkId = artwork.ArtworkId,
                Title = artwork.Title,
                Description = artwork.Description,
                Date = artwork.Date,
                Price = artwork.Price,
                ArtistId = artwork.ArtistId  
            };

            return View(dto);
        }

        [HttpPost("Edit/{id}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateArtworkDto dto)
        {
            if (id != dto.ArtworkId)
            {
                return View("Error", new ErrorViewModel { Errors = ["Artwork ID mismatch."] });
            }

            if (!ModelState.IsValid)
                return View(dto);

            var result = await _artworkService.UpdateArtwork(id, dto);

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
            var artwork = await _artworkService.FindArtwork(id);

            if (artwork == null)
            {
                return View("Error", new ErrorViewModel { Errors = ["Artwork not found."] });
            }

            return View(artwork);
        }

        [HttpPost("Delete/{id}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _artworkService.DeleteArtwork(id);

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
