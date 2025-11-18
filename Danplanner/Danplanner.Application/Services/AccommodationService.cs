using Danplanner.Application.Interfaces.AccommodationInterfaces;
using Danplanner.Application.Models;

namespace Danplanner.Application.Services
{
    public class AccommodationService : IAccommodationService
    {
        private readonly IAccommodationRepository _repository;

        public AccommodationService(IAccommodationRepository repository)
        {
            _repository = repository;
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
                Availability = a.Availability
            }).ToList();
        }
    }
}
