using System.ComponentModel.DataAnnotations;

namespace Danplanner.Domain.Entities
{
    public class Accommodation
    {
        [Key]
        public int AccommodationId { get; set; }
        [Required]
        public string AccommodationName { get; set; }
        [Required]
        public string AccommodationDescription { get; set; }
    }
}
