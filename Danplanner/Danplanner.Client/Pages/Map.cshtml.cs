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
        private readonly IAccommodationAvailability _availabilityRepo;

        public MapModel(IWebHostEnvironment env,
                        IAccommodationAvailability availabilityRepo)
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
        public string? SelectedCategory { get; set; }

        public async Task OnGetAsync()
        {
            // 1) Indlæs map.json
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

            // 2) Forvalgt pin
            SelectedKey = Request.Cookies["selectedItem"];
            var qSelected = Request.Query["selected"].FirstOrDefault();
            if (!string.IsNullOrEmpty(qSelected))
                SelectedKey = qSelected;

            // 3) Kategori filter
            SelectedCategory = Request.Cookies["selectedCategory"];
            var qCategory = Request.Query["category"].FirstOrDefault();
            if (!string.IsNullOrEmpty(qCategory))
                SelectedCategory = qCategory;
            var categoryFilter = SelectedCategory?.Trim();
            var hasCategoryFilter = !string.IsNullOrEmpty(categoryFilter);

            // 4) Hent ledige AccommodationId'er fra databasen via EF
            var availableIds = await _availabilityRepo.GetAvailableIdsAsync();
            MapPoint[] filteredPoints;

            if (availableIds == null || availableIds.Count == 0)
            {
                // sikkerhedsnet: vis alle pins, hvis der ikke kommer noget
                filteredPoints = mapDef.Points
                    .Where(p => !hasCategoryFilter ||
                                string.Equals(p.Category?.Trim(), categoryFilter, StringComparison.OrdinalIgnoreCase))
                    .ToArray();

            }
            else
            {
                var idSet = new HashSet<int>(availableIds);

                // 5) Filtrér punkterne i map.json på AccommodationId
                filteredPoints = mapDef.Points
                    .Where(p => p.AccommodationId.HasValue && 
                                idSet.Contains(p.AccommodationId.Value) &&
                                (!hasCategoryFilter || string.Equals(p.Category?.Trim(), categoryFilter, StringComparison.OrdinalIgnoreCase)))
                    .ToArray();
            }

            var filteredMap = new MapDefinition
            {
                ImageUrl = mapDef.ImageUrl,
                Width = mapDef.Width,
                Height = mapDef.Height,
                Points = filteredPoints
            };

            // 6) Send til JavaScript
            FilteredMapJson = JsonSerializer.Serialize(filteredMap);
        }
    }
}
