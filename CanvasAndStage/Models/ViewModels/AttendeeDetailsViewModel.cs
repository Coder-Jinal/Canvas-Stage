using System.Collections.Generic;

namespace CanvasAndStage.Models.ViewModels
{
    public class AttendeeDetailsViewModel
    {
        public AttendeeDto Attendee { get; set; }
        public List<EventDto> Events { get; set; }
        public List<PurchaseDto> Purchases { get; set; }
    }
}
