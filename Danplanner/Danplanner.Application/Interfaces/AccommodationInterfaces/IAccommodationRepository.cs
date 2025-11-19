using Danplanner.Domain.Entities;

namespace Danplanner.Application.Interfaces.AccommodationInterfaces
{
    public interface IAccommodationRepository
    {
        Task<IReadOnlyList<Accommodation>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
