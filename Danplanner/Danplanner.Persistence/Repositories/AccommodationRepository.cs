using Danplanner.Application.Interfaces.AccommodationInterfaces;
using Danplanner.Domain.Entities;
using Microsoft.AspNetCore.Hosting;

namespace Danplanner.Persistence.Repositories
{
    public class AccommodationRepository : IAccommodationGetAll
    {
        private readonly string _dataFilePath;

        public AccommodationRepository(IWebHostEnvironment env)
        {
            _dataFilePath = Path.Combine(env.WebRootPath ?? string.Empty, "data", "accommodations.txt");
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
    }
}
