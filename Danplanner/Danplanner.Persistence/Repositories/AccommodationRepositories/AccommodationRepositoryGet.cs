using Danplanner.Application.Interfaces.AccommodationInterfaces;
using Danplanner.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Danplanner.Persistence.DbMangagerDir;
using Microsoft.EntityFrameworkCore;

namespace Danplanner.Persistence.Repositories.AccommodationRepositories
{
    public class AccommodationRepositoryGet : IAccommodationGetAll, IAccommodationGetById
    {
        private readonly string _dataFilePath;
        private readonly DbManager _db;

        public AccommodationRepositoryGet(IWebHostEnvironment env, DbManager dbManger)
        {
            _dataFilePath = Path.Combine(env.WebRootPath ?? string.Empty, "data", "accommodations.txt");
            _db = dbManger;
        }
        public async Task<IReadOnlyList<Accommodation>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            if (!File.Exists(_dataFilePath))
            {
                throw new FileNotFoundException("Accommodation data file not found", _dataFilePath);
            }

            var lines = await File.ReadAllLinesAsync(_dataFilePath, cancellationToken);
            var result = new List<Accommodation>();

            foreach (var raw in lines)
            {
                var line = raw?.Trim();
                if (string.IsNullOrEmpty(line)) continue;
                if (line.StartsWith("#")) continue;

                var parts = line.Split('|');
                if (parts.Length < 5) continue;

                var entity = new Accommodation
                {
                    AccommodationId = int.TryParse(parts[0].Trim(), out var id) ? id : 0,
                    AccommodationName = parts[1].Trim(),
                    AccommodationDescription = parts[2].Trim(),
                    PricePerNight = decimal.TryParse(parts[3].Trim(), out var price) ? price : null,
                    ImageUrl = parts[4].Trim(),
                };
                result.Add(entity);
            }
            return result;
        }

        public async Task<IReadOnlyCollection<int>> GetAvailableIdsAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Accommodation
                .AsNoTracking()
                .Where(a => a.Availability == 1)
                .Select(a => a.AccommodationId)
                .ToListAsync(cancellationToken);
        }
    }
}
