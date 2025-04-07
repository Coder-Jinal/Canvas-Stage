using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CanvasAndStage.Models
{
    public class Purchase
    {
        [Key]
        public int PurchaseId { get; set; }
        public float TotalPrice { get; set; }

        // one purchase belongs to one attendee
        [ForeignKey("Attendees")]
        public int AttendeeId { get; set; }
        public virtual Attendee Attendee { get; set; }


        // one purchase belongs to one artwork
        [ForeignKey("Artworks")]
        public int ArtworkId { get; set; }
        public virtual Artwork Artwork { get; set; }


        // one purchase belongs to one event
        [ForeignKey("Events")]
        public int EventId { get; set; }
        public virtual Event Event { get; set; }
    }


    public class PurchaseDto
    {
        public int PurchaseId { get; set; }

        public float TotalPrice { get; set; }

        public string ArtworkName { get; set; }

        public string ArtistName { get; set; }
        public string AttendeeName { get; set; }

        public string EventName { get; set; }


    }

    public class AddPurchaseDto
    {
        public int AttendeeId { get; set; }  

        public int ArtworkId { get; set; }
        
        public int EventId { get; set; }

        public float TotalPrice { get; set; }


    }

    public class UpdatePurchaseDto
    {
        public int PurchaseId { get; set; }

        public int AttendeeId { get; set; }

        public int ArtworkId { get; set; }

        public int EventId { get; set; }
        public float TotalPrice { get; set; }


    }
}
