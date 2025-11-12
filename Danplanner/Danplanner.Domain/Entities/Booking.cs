using System.ComponentModel.DataAnnotations;

namespace Danplanner.Domain.Entities
{
    internal class Booking
    {
        [Key]
        public int BookingId { get; set; }
        [Required]
        public int BookingResidents { get; set; }
        [Required]
        public double BookingPrice { get; set; }
    }
}
