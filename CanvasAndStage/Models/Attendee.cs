using System.ComponentModel.DataAnnotations;

namespace CanvasAndStage.Models
{
    public class Attendee
    {
        [Key]
        public int AttendeeId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string ContactNumber   { get; set; }

        // one attendee can attend many events
        public ICollection<Event> Events { get; set; }

        // one attendee can have multiple purchases
        public ICollection<Purchase> Purchases { get; set; }



    }

    public class AttendeeDto
    {
        [Required]
        public int AttendeeId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string ContactNumber { get; set; }

        // List of event IDs the attendee is associated with
        public List<int> EventIds { get; set; }

        // List of purchase IDs the attendee is associated with
        public List<int> PurchaseIds { get; set; }
    }
}
