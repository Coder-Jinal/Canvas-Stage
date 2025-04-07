using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CanvasAndStage.Models
{
    public class Artwork
    {
        [Key]
        public int ArtworkId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public DateOnly Date { get; set; }

        public float Price { get; set; }

        // one artwork belong to one artist
        [ForeignKey("Artists")]
        public int ArtistId { get; set; } // the primary key ArtistId of Artists Table is foreign key of Artworks Table
        public virtual Artist Artist { get; set; } // path to Artists table

        // one artwork can be purchased multiple times
        public ICollection<Purchase> Purchases { get; set; }

    }


    public class ArtworkDto
    {
        public int ArtworkId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateOnly Date { get; set; }

        public float Price { get; set; }

        public float TotalPriceWithTax { get; set; }

        public string ArtistName { get; set; }

        public int TimesPurchased { get; set; }

        public List<string> AttendeePurchased { get; set; }

        public int ArtistId { get; set; } 
    }



    public class AddArtworkDto
    { 
        public string Title { get; set; }

        public string Description { get; set; }

        public DateOnly Date { get; set; }

        public float Price { get; set; }

        public int ArtistId { get; set; }
    }


    public class UpdateArtworkDto
    {
        public int ArtworkId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateOnly Date { get; set; }

        public float Price { get; set; }
        public int ArtistId { get; set; } 
    }
}
