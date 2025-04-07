using System.Collections.Generic;

namespace CanvasAndStage.Models.ViewModels
{
    public class EventDetailsViewModel
    {
        public EventDto Event { get; set; }
        public List<ArtistDto> Artists { get; set; } = new List<ArtistDto>();
        public List<AttendeeDto> Attendees { get; set; } = new List<AttendeeDto>();
        public List<PurchaseDto> Purchases { get; set; } = new List<PurchaseDto>();

        public List<ArtistDto> UnlinkedArtists { get; set; } = new List<ArtistDto>();
        public List<AttendeeDto> UnlinkedAttendees { get; set; } = new List<AttendeeDto>();
    }
}
