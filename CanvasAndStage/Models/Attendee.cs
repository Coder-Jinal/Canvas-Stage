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

        public int ContactNumber   { get; set; }

        // one attendee can attend many events
        public ICollection<Event> Events { get; set; }

        // one attendee can have multiple purchases
        public ICollection<Purchase> Purchases { get; set; }

    }


    public class AttendeeDto
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

        public int ContactNumber { get; set; }

        public List<string> EventNames { get; set; }

        public int TotalEvents { get; set; }

        public float TotalPurchase { get; set; }

        public int TotalArtworksPurchased { get; set; }

        public List<string> ArtworkNames { get; set; }
    }

    public class AddAttendeeDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public int ContactNumber { get; set; }
    }

    public class UpdateAttendeeDto
    {
        public int AttendeeId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public int ContactNumber { get; set; }
    }
}
