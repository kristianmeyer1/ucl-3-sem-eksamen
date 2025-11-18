using Danplanner.Application.Interfaces.AccommodationInterfaces;
using Danplanner.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using System.Text.Json;

namespace Danplanner.Client.Pages
{
    public class MapModel : PageModel
    {
        private readonly IWebHostEnvironment _env;
        private readonly IAccommodationAvailabilityRepository _availabilityRepo;

        public MapModel(IWebHostEnvironment env,
                        IAccommodationAvailabilityRepository availabilityRepo)
        {
            _env = env;
            _availabilityRepo = availabilityRepo;
        }

        [BindProperty(SupportsGet = true)]
        public string? Start { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? End { get; set; }

        public string FilteredMapJson { get; private set; } = "{}";
        public string? SelectedKey { get; private set; }

        public async Task OnGetAsync()
        {
            // 1) Indlæs map.json (samme som før)
            var dataDir = Path.Combine(_env.WebRootPath ?? string.Empty, "data");
            var mapFile = Path.Combine(dataDir, "map.json");
            MapDefinition mapDef;

            if (System.IO.File.Exists(mapFile))
            {
                var raw = System.IO.File.ReadAllText(mapFile);
                try
                {
                    mapDef = JsonSerializer.Deserialize<MapDefinition>(
                        raw,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    ) ?? new MapDefinition();
                }
                catch
                {
                    mapDef = new MapDefinition();
                }
            }
            else
            {
                throw new FileNotFoundException("Map definition file not found.", mapFile);
            }

            // 2) Forvalgt pin (din eksisterende logik)
            SelectedKey = Request.Cookies["selectedItem"];
            var qSelected = Request.Query["selected"].FirstOrDefault();
            if (!string.IsNullOrEmpty(qSelected))
                SelectedKey = qSelected;

            // 3) Hent ledige AccommodationId'er fra databasen via EF
            var availableIds = await _availabilityRepo.GetAvailableIdsAsync();
            MapPoint[] filteredPoints;

            if (availableIds == null || availableIds.Count == 0)
            {
                // sikkerhedsnet: vis alle pins hvis der ikke kommer noget
                filteredPoints = mapDef.Points;
            }
            else
            {
                var idSet = new HashSet<int>(availableIds);

                // 4) Filtrér punkterne i map.json på AccommodationId
                filteredPoints = mapDef.Points
                    .Where(p => p.AccommodationId.HasValue &&
                                idSet.Contains(p.AccommodationId.Value))
                    .ToArray();
            }

            var filteredMap = new MapDefinition
            {
                ImageUrl = mapDef.ImageUrl,
                Width = mapDef.Width,
                Height = mapDef.Height,
                Points = filteredPoints
            };

            // 5) Send til JavaScript
            FilteredMapJson = JsonSerializer.Serialize(filteredMap);
        }

        private static DateTime? ParseDate(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;

            if (DateTime.TryParseExact(
                    raw,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var dt))
            {
                return dt.Date;
            }

            return null;
        }
    }
}
