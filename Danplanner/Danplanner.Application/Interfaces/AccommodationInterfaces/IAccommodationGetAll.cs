using Danplanner.Domain.Entities;

namespace Danplanner.Application.Interfaces.AccommodationInterfaces
{
    public interface IAccommodationGetAll
    {
        Task<IReadOnlyList<Accommodation>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
