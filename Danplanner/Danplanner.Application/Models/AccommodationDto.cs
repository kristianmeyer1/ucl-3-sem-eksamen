using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danplanner.Application.Models
{
    public class AccommodationDto
    {
        public int AccommodationId { get; set; }
        public string AccommodationName { get; set; } = string.Empty;
        public string AccommodationDescription { get; set; } = string.Empty;
        public decimal? PricePerNight { get; set; }
        public int Availability { get; set; }
        public string? ImageUrl { get; set; }
    }
}
