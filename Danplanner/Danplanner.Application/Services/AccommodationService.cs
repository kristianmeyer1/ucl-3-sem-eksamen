using Danplanner.Application.Interfaces;
using Danplanner.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                Key = a.Key,
                Title = a.Name,
                Description = a.Description,
                PriceText = a.PricePerNight.HasValue
                                ? $"{a.PricePerNight.Value:N0} kr. pr. døgn"
                                : "Kontakt os for pris",
                ImageUrl = a.ImageUrl ?? "/images/default.png"
            }).ToList();
        }
    }
}
