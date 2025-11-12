using System.ComponentModel.DataAnnotations;

namespace Danplanner.Domain.Entities
{
    internal class Accommodation
    {
        [Key]
        public int AccommodationId { get; set; }
        [Required]
        public string AccommodationName { get; set; }
        [Required]
        public string AccommodationDescription { get; set; }
    }
}
