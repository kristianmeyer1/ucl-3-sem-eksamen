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
        public string AccommodationName { get; set; }
        public string AccommodationDescription { get; set; }
        public double PricePerNight { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PriceText { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}
