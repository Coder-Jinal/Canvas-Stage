﻿using System.ComponentModel.DataAnnotations;

namespace CanvasAndStage.Models
{
    public class Artist
    {
        [Key]
        public int ArtistId { get; set; }

        [Required]
        public string FName { get; set; }

        [Required]
        public string LName { get; set; }

        public string Bio   { get; set; }

        [Required]
        [EmailAddress]
        public string EmailId { get; set; }

        public int PhoneNumber { get; set; }

        // one artist can particiapate in many events
        public ICollection<Event> Events { get; set; }

        // one artist can have many artworks
        public ICollection<Artwork> Artworks { get; set; }
    }



    public class ArtistDto
    {
        public int ArtistId { get; set; }

        public string FName { get; set; }

        public string LName { get; set; }

        public string Bio { get; set; }

        public string EmailId { get; set; }

        public int PhoneNumber { get; set; }

        public int TotalArtworks { get; set; }
        public List<ArtworkDto> Artworks { get; set; } = new List<ArtworkDto>();
    }

    public class AddArtistDto
    {
        public string FName { get; set; }

        public string LName { get; set; }

        public string Bio { get; set; }

        public string EmailId { get; set; }

        public int PhoneNumber { get; set; }
    }

    public class UpdateArtistDto
    {
        public int ArtistId { get; set; }

        public string FName { get; set; }

        public string LName { get; set; }

        public string Bio { get; set; }

        public string EmailId { get; set; }

        public int PhoneNumber { get; set; }
    }

}
