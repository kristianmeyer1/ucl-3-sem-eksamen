using System.ComponentModel.DataAnnotations;

namespace Danplanner.Domain.Entities
{
    public class Accommodation
    {
        [Key]
        public int AccommodationId { get; set; }
        [Required]
        public string AccommodationName { get; set; } = string.Empty;
        [Required]
        public string AccommodationDescription { get; set; } = string.Empty;
        [Required]
        public decimal? PricePerNight { get; set; }
        [Required]
        public int Availability { get; set; }
        public string? ImageUrl { get; set; }
    }
}
