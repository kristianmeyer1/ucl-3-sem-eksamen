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
