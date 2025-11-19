using Danplanner.Application.Models;

namespace Danplanner.Application.Interfaces.AccommodationInterfaces
{
    public interface IAccommodationService
    {
        Task<IReadOnlyList<AccommodationDto>> GetAccommodationsAsync(
            DateTime? start,
            DateTime? end,
            int? daysOverride,
            CancellationToken cancellationToken = default);
    }
}
