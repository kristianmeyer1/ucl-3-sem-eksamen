using Danplanner.Application.Interfaces;
using Danplanner.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danplanner.Persistence.Repositories
{
    public class AccommodationRepository : IAccommodationRepository
    {
        private readonly string _dataFilePath;

        public AccommodationRepository(IWebHostEnvironment env)
        {
            _dataFilePath = Path.Combine(env.WebRootPath ?? string.Empty, "data","accommodations.txt");
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
                    Key = parts[0].Trim(),
                    Name = parts[1].Trim(),
                    Description = parts[2].Trim(),
                    PricePerNight = decimal.TryParse(parts[3].Trim(), out var price) ? price : null,
                    ImageUrl = parts[4].Trim(),
                };
                result.Add(entity);
            }
            return result;
        }
    }
}
