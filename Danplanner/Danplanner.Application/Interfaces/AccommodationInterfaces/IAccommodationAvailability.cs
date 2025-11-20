namespace Danplanner.Application.Interfaces.AccommodationInterfaces
{
    public interface IAccommodationAvailability
    {
        Task<IReadOnlyCollection<int>> GetAvailableIdsAsync(
            CancellationToken cancellationToken = default);
    }
}
