using Microsoft.AspNetCore.Mvc;
using CanvasAndStage.Interfaces;
using CanvasAndStage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CanvasAndStage.Controllers
{
    [Route("PurchasesPage")]
    public class PurchasesPageController : Controller
    {
        private readonly IPurchaseService _purchaseService;
        private readonly IAttendeeService _attendeeService;
        private readonly IArtworkService _artworkService;
        private readonly IEventService _eventService;

        public PurchasesPageController(
            IPurchaseService purchaseService,
            IAttendeeService attendeeService,
            IArtworkService artworkService,
            IEventService eventService)
        {
            _purchaseService = purchaseService;
            _attendeeService = attendeeService;
            _artworkService = artworkService;
            _eventService = eventService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        [HttpGet("List")]
        public async Task<IActionResult> List()
        {
            IEnumerable<PurchaseDto> purchases = await _purchaseService.ListPurchases();
            return View(purchases);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var purchase = await _purchaseService.FindPurchase(id);
            if (purchase == null)
            {
                return View("Error", new ErrorViewModel { Errors = ["Purchase not found."] });
            }
            return View(purchase);
        }

        [HttpGet("Add")]
        public async Task<IActionResult> Add()
        {
            ViewBag.Attendees = await _attendeeService.ListAttendees();
            ViewBag.Artworks = await _artworkService.ListArtworks();
            ViewBag.Events = await _eventService.ListEvents();
            return View();
        }

        [HttpPost("Add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddPurchaseDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _purchaseService.AddPurchase(dto);

            if (result.Status == ServiceResponse.ServiceStatus.Error)
            {
                return View("Error", new ErrorViewModel { Errors = result.Messages });
            }

            return RedirectToAction("List");
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var purchase = await _purchaseService.FindPurchase(id);

            if (purchase == null)
            {
                return View("Error", new ErrorViewModel { Errors = ["Purchase not found."] });
            }

            ViewBag.Attendees = await _attendeeService.ListAttendees();
            ViewBag.Artworks = await _artworkService.ListArtworks();
            ViewBag.Events = await _eventService.ListEvents();

            var dto = new UpdatePurchaseDto
            {
                PurchaseId = purchase.PurchaseId,
                TotalPrice = purchase.TotalPrice
            };

            return View(dto);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdatePurchaseDto dto)
        {
            if (id != dto.PurchaseId)
            {
                return View("Error", new ErrorViewModel { Errors = ["Purchase ID mismatch."] });
            }

            if (!ModelState.IsValid)
                return View(dto);

            var result = await _purchaseService.UpdatePurchase(id, dto);

            if (result.Status == ServiceResponse.ServiceStatus.Error)
            {
                return View("Error", new ErrorViewModel { Errors = result.Messages });
            }

            return RedirectToAction("Details", new { id });
        }

        [HttpGet("ConfirmDelete/{id}")]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var purchase = await _purchaseService.FindPurchase(id);

            if (purchase == null)
            {
                return View("Error", new ErrorViewModel { Errors = ["Purchase not found."] });
            }

            return View(purchase);
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _purchaseService.DeletePurchase(id);

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
