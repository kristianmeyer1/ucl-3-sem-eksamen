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
    }
}
