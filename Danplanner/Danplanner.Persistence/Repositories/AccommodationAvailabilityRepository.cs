using Danplanner.Application.Interfaces.AccommodationInterfaces;
using Danplanner.Persistence.DbMangagerDir;
using Microsoft.EntityFrameworkCore;

namespace Danplanner.Persistence.Repositories
{
    public class AccommodationAvailabilityRepository : IAccommodationAvailabilityRepository
    {
        private readonly DbManager _db;

        public AccommodationAvailabilityRepository(DbManager db)
        {
            _db = db;
        }
        public async Task<IReadOnlyCollection<int>> GetAvailableIdsAsync(
            CancellationToken cancellationToken = default)
        {
            return await _db.Accommodation
                .AsNoTracking()
                .Where(a => a.Availability == 1)
                .Select(a => a.AccommodationId)
                .ToListAsync(cancellationToken);
        }
    }
}
