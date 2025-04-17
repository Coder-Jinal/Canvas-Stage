using Microsoft.AspNetCore.Mvc;
using CanvasAndStage.Interfaces;
using CanvasAndStage.Models;
using CanvasAndStage.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CanvasAndStage.Services;
using Microsoft.AspNetCore.Authorization;

namespace CanvasAndStage.Controllers
{
    [Route("EventsPage")]
    public class EventsPageController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IAttendeeService _attendeeService;
        private readonly IPurchaseService _purchaseService;
        private readonly IArtistService _artistService;

        public EventsPageController(
            IEventService eventService,
            IAttendeeService attendeeService,
            IPurchaseService purchaseService,
            IArtistService artistService)
        {
            _eventService = eventService;
            _attendeeService = attendeeService;
            _purchaseService = purchaseService;
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
            var result = await _eventService.GetPaginatedEvents(page, pageSize);
            return View(result);
        }


        // GET: EventsPage/Add
        [HttpGet("Add")]
        public IActionResult Add()
        {
            return View();
        }

        // POST: EventsPage/Add
        [HttpPost("Add")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddEventDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _eventService.AddEvent(dto);

            if (result.Status == ServiceResponse.ServiceStatus.Error)
            {
                return View("Error", new ErrorViewModel { Errors = result.Messages });
            }

            return RedirectToAction("List");
        }

        // GET: EventsPage/Edit/{id}
        [HttpGet("Edit/{id}")]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var eventDto = await _eventService.FindEvent(id);

            if (eventDto == null)
            {
                return View("Error", new ErrorViewModel { Errors = ["Event not found."] });
            }

            var dto = new UpdateEventDto
            {
                EventId = eventDto.EventId,
                Name = eventDto.Name,
                Location = eventDto.Location,
                Description = eventDto.Description,
                Date = eventDto.Date
            };

            return View(dto);
        }

        // POST: EventsPage/Edit/{id}
        [HttpPost("Edit/{id}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateEventDto dto)
        {
            if (id != dto.EventId)
            {
                return View("Error", new ErrorViewModel { Errors = ["Event ID mismatch."] });
            }

            if (!ModelState.IsValid)
                return View(dto);

            var result = await _eventService.UpdateEvent(id, dto);

            if (result.Status == ServiceResponse.ServiceStatus.Error)
            {
                return View("Error", new ErrorViewModel { Errors = result.Messages });
            }

            return RedirectToAction("Details", new { id });
        }

        // GET: EventsPage/ConfirmDelete/{id}
        [HttpGet("ConfirmDelete/{id}")]
        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var eventDto = await _eventService.FindEvent(id);

            if (eventDto == null)
            {
                return View("Error", new ErrorViewModel { Errors = ["Event not found."] });
            }

            return View(eventDto);
        }

        // POST: EventsPage/Delete/{id}
        [HttpPost("Delete/{id}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _eventService.DeleteEvent(id);

            if (result.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List");
            }
            else
            {
                return View("Error", new ErrorViewModel { Errors = result.Messages });
            }
        }



        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var eventDto = await _eventService.FindEvent(id);
            if (eventDto == null)
            {
                return View("Error", new ErrorViewModel { Errors = ["Event not found."] });
            }

            var attendees = await _attendeeService.ListAttendees();
            var purchases = await _purchaseService.ListPurchases();
            var artists = await _artistService.ListArtists();

            var eventAttendees = attendees.Where(a => eventDto.AttendeeNames.Contains($"{a.FirstName} {a.LastName}")).ToList();
            var eventPurchases = purchases.Where(p => p.EventName == eventDto.Name).ToList();
            var eventArtists = artists.Where(a => eventDto.ArtistNames.Contains($"{a.FName} {a.LName}")).ToList();

            var unlinkedArtists = artists.Where(a => !eventDto.ArtistNames.Contains($"{a.FName} {a.LName}")).ToList();
            var unlinkedAttendees = attendees.Where(a => !eventDto.AttendeeNames.Contains($"{a.FirstName} {a.LastName}")).ToList();

            var viewModel = new EventDetailsViewModel
            {
                Event = eventDto,
                Attendees = eventAttendees,
                Artists = eventArtists,
                Purchases = eventPurchases ?? new List<PurchaseDto>(),
                UnlinkedArtists = unlinkedArtists,
                UnlinkedAttendees = unlinkedAttendees
            };

            return View(viewModel);
        }


        [HttpPost("LinkArtist")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkArtist(LinkArtistDto dto)
        {
            var response = await _eventService.LinkArtist(dto.EventId, dto.ArtistId);
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return View("Error", new ErrorViewModel { Errors = ["Event or Artist not found."] });
            }

            return RedirectToAction("Details", new { id = dto.EventId });
        }

        [HttpPost("UnlinkArtist")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlinkArtist(LinkArtistDto dto)
        {
            var response = await _eventService.UnlinkArtist(dto.EventId, dto.ArtistId);
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return View("Error", new ErrorViewModel { Errors = ["Event or Artist not found."] });
            }

            return RedirectToAction("Details", new { id = dto.EventId });
        }

        [HttpPost("LinkAttendee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkAttendee(LinkAttendeeDto dto)
        {
            var response = await _eventService.LinkAttendee(dto.EventId, dto.AttendeeId);
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return View("Error", new ErrorViewModel { Errors = ["Event or Attendee not found."] });
            }

            return RedirectToAction("Details", new { id = dto.EventId });
        }


        [HttpPost("UnlinkAttendee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlinkAttendee(LinkAttendeeDto dto)
        {
            var response = await _eventService.UnlinkAttendee(dto.EventId, dto.AttendeeId);
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return View("Error", new ErrorViewModel { Errors = ["Event or Attendee not found."] });
            }

            return RedirectToAction("Details", new { id = dto.EventId });
        }

    }
}
