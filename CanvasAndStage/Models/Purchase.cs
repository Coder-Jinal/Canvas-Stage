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
}
