using System.ComponentModel.DataAnnotations;

namespace Danplanner.Domain.Entities
{
    public class Accommodation
    {
        [Key]
        public int AccommodationId { get; set; }
        [Required]
        public string AccommodationName { get; set; }
        public string AccommodationDescription { get; set; }
        [Required]
        public double PricePerNight { get; set; }
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public decimal? PricePerNight { get; set; }
        public string? ImageUrl { get; set; }
        public int Availability { get; set; }
        public string Key { get; set; } = string.Empty;
    }
}
