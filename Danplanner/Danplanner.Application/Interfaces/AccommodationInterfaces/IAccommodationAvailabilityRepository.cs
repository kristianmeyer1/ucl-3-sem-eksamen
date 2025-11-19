namespace Danplanner.Application.Interfaces.AccommodationInterfaces
{
    public interface IAccommodationAvailabilityRepository
    {
        Task<IReadOnlyCollection<int>> GetAvailableIdsAsync(
            CancellationToken cancellationToken = default);
    }
}
