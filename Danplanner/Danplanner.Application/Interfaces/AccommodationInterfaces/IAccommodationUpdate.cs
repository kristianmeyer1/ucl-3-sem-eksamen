namespace Danplanner.Application.Interfaces.AccommodationInterfaces
{
    public interface IAccommodationUpdate
    {
        Task MarkUnavailableAsync(int accommodationId, CancellationToken cancellationToken = default);
    }
}
