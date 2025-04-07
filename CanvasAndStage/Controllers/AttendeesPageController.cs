using Microsoft.AspNetCore.Mvc;
using CanvasAndStage.Interfaces;
using CanvasAndStage.Models;
using CanvasAndStage.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanvasAndStage.Controllers
{
    [Route("AttendeesPage")]
    public class AttendeesPageController : Controller
    {
        private readonly IAttendeeService _attendeeService;
        private readonly IEventService _eventService;
        private readonly IPurchaseService _purchaseService;

        public AttendeesPageController(
            IAttendeeService attendeeService,
            IEventService eventService,
            IPurchaseService purchaseService)
        {
            _attendeeService = attendeeService;
            _eventService = eventService;
            _purchaseService = purchaseService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: AttendeesPage/List
        [HttpGet("List")]
        public async Task<IActionResult> List()
        {
            IEnumerable<AttendeeDto> attendees = await _attendeeService.ListAttendees();
            return View(attendees);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var attendee = await _attendeeService.FindAttendee(id);
            if (attendee == null)
            {
                return View("Error", new ErrorViewModel { Errors = ["Attendee not found."] });
            }

            var purchases = await _purchaseService.ListPurchases();
            var attendeePurchases = purchases.Where(p => p.AttendeeName == $"{attendee.FirstName} {attendee.LastName}").ToList();

            var viewModel = new AttendeeDetailsViewModel
            {
                Attendee = attendee,
                Purchases = attendeePurchases
            };

            return View(viewModel);
        }


        // GET: AttendeesPage/Add
        [HttpGet("Add")]
        public IActionResult Add()
        {
            return View();
        }

        // POST: AttendeesPage/Add
        [HttpPost("Add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddAttendeeDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _attendeeService.AddAttendee(dto);

            if (result.Status == ServiceResponse.ServiceStatus.Error)
            {
                return View("Error", new ErrorViewModel { Errors = result.Messages });
            }

            return RedirectToAction("List");
        }

        // GET: AttendeesPage/Edit/{id}
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var attendee = await _attendeeService.FindAttendee(id);

            if (attendee == null)
            {
                return View("Error", new ErrorViewModel { Errors = ["Attendee not found."] });
            }

            var dto = new UpdateAttendeeDto
            {
                AttendeeId = attendee.AttendeeId,
                FirstName = attendee.FirstName,
                LastName = attendee.LastName,
                Email = attendee.Email,
                ContactNumber = attendee.ContactNumber
            };

            return View(dto);
        }

        // POST: AttendeesPage/Edit/{id}
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateAttendeeDto dto)
        {
            if (id != dto.AttendeeId)
            {
                return View("Error", new ErrorViewModel { Errors = ["Attendee ID mismatch."] });
            }

            if (!ModelState.IsValid)
                return View(dto);

            var result = await _attendeeService.UpdateAttendee(id, dto);

            if (result.Status == ServiceResponse.ServiceStatus.Error)
            {
                return View("Error", new ErrorViewModel { Errors = result.Messages });
            }

            return RedirectToAction("Details", new { id });
        }

        // GET: AttendeesPage/ConfirmDelete/{id}
        [HttpGet("ConfirmDelete/{id}")]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var attendee = await _attendeeService.FindAttendee(id);

            if (attendee == null)
            {
                return View("Error", new ErrorViewModel { Errors = ["Attendee not found."] });
            }

            return View(attendee);
        }

        // POST: AttendeesPage/Delete/{id}
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _attendeeService.DeleteAttendee(id);

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
