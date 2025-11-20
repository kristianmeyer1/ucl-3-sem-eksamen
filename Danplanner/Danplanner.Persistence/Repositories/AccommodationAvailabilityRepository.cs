using Danplanner.Application.Interfaces.AccommodationInterfaces;
using Danplanner.Persistence.DbMangagerDir;
using Microsoft.EntityFrameworkCore;

namespace Danplanner.Persistence.Repositories
{
    public class AccommodationAvailabilityRepository : IAccommodationAvailability
    {
        private readonly DbManager _db;

        public AccommodationAvailabilityRepository(DbManager db)
        {
            _db = db;
        }
        public async Task<IReadOnlyCollection<int>> GetAvailableIdsAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Accommodation
                .AsNoTracking()
                .Where(a => a.Availability == 1)
                .Select(a => a.AccommodationId)
                .ToListAsync(cancellationToken);
        }
        public async Task MarkUnavailableAsync(int accommodationId, CancellationToken ct = default)
        {
            var entity = await _db.Accommodation
                .FirstOrDefaultAsync(a => a.AccommodationId == accommodationId, ct);

            if (entity == null)
                return;

            entity.Availability = 0;
            await _db.SaveChangesAsync(ct);
        }
    }
}
