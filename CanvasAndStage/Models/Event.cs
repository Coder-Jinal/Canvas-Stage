using System.ComponentModel.DataAnnotations;

namespace CanvasAndStage.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public DateOnly Date { get; set; }

        // one event can be attended by many attendees
        public ICollection<Attendee> Attendees { get; set; }

        // one event can host many artists
        public ICollection<Artist> Artists { get; set; }

        // one event can have multiple purchases of artworks by attendees

        public ICollection<Purchase> Purchases { get; set; }

    }

    public class EventDto
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public DateOnly Date { get; set; }
        public int TotalAttendees { get; set; }
        public List<string> AttendeeNames { get; set; }
        public int TotalArtists { get; set; }
        public List<string> ArtistNames { get; set; }
        public float TotalPurchase { get; set; }
    }

    public class AddEventDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public DateOnly Date { get; set; }

    }


    public class UpdateEventDto
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public DateOnly Date { get; set; }

    }
}
