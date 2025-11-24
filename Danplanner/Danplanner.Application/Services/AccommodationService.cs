using Danplanner.Application.Interfaces.AccommodationInterfaces;
using Danplanner.Application.Models.ModelsDto;

namespace Danplanner.Application.Services
{
    public class AccommodationService : IAccommodationTransfer
    {
        private readonly IAccommodationGetAll _repository;

        public AccommodationService(IAccommodationGetAll repository)
        {
            _repository = repository;
        }
        private static string? CategoryFromName(string name)
        {
            var n = (name ?? string.Empty).ToLowerInvariant();

            if (n.Contains("luksus")) return "luksushytte";
            if (n.Contains("hytte")) return "hytte";
            if (n.Contains("plads")) return "plads";
            return null;
        }
        public async Task<IReadOnlyList<AccommodationDto>> GetAccommodationsAsync(
            DateTime? start,
            DateTime? end,
            int? daysOverride,
            CancellationToken cancellationToken = default)
        {
            var entities = await _repository.GetAllAsync(cancellationToken);

            return entities.Select(a => new AccommodationDto
            {
                AccommodationId = a.AccommodationId,
                AccommodationName = a.AccommodationName,
                AccommodationDescription = a.AccommodationDescription,
                PricePerNight = a.PricePerNight,
                ImageUrl = a.ImageUrl ?? "/images/default.png",
                Availability = a.Availability,
                Category = !string.IsNullOrWhiteSpace(a.Category)
                    ? a.Category.ToLowerInvariant()
                    : CategoryFromName(a.AccommodationName)
            }).ToList();
        }
    }
}
