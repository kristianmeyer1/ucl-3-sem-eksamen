using Danplanner.Application.Interfaces.AccommodationInterfaces;
using Danplanner.Application.Interfaces.ReservationInterfaces;
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
        private readonly IAccommodationGetById _accommodationGetById;
        private readonly IReservationLockService _reservationLockService;

        public MapModel(IWebHostEnvironment env, IAccommodationGetById accommodationGetById, IReservationLockService reservationLockService)
        {
            _env = env;
            _accommodationGetById = accommodationGetById;
            _reservationLockService = reservationLockService;
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
            var startDt = ParseDate(Start);
            var endDt = ParseDate(End);
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
            // filtrér mapDef.Points på availability (+ kategori hvis du har det)
            var categoryFilter = SelectedCategory?.Trim().ToLowerInvariant();
            var hasCategoryFilter = !string.IsNullOrEmpty(categoryFilter);

            // 4) Hent ledige AccommodationId'er fra databasen via EF
            var availableIds = await _accommodationGetById.GetAvailableIdsAsync(startDt, endDt);
            var idSet = new HashSet<int>(availableIds);

            // 5) Filtrér punkterne på availability
            var filteredPoints = mapDef.Points
                .Where(p =>
                    p.AccommodationId.HasValue &&
                    idSet.Contains(p.AccommodationId.Value) &&
                    (!hasCategoryFilter ||
                     string.Equals(p.Category?.Trim(), categoryFilter, StringComparison.OrdinalIgnoreCase)))
                .ToArray();

            var filteredMap = new MapDefinition
            {
                ImageUrl = mapDef.ImageUrl,
                Width = mapDef.Width,
                Height = mapDef.Height,
                Points = filteredPoints
            };

            FilteredMapJson = JsonSerializer.Serialize(filteredMap);
        }
        public async Task<IActionResult> OnPostLockAsync([FromBody] LockRequest req)
        {
            if (req == null || req.AccommodationId <= 0)
                return new JsonResult(new LockResponse { Success = false, Message = "Invalid request" });

            // Build a deterministic lock key — include dates/placeKey to allow same accommodation for different dates
            var key = $"{req.AccommodationId}|{req.Start}|{req.End}|{req.PlaceKey}";

            // owner can be connection id or user id if available
            var owner = User?.Identity?.Name ?? HttpContext.Connection.Id;

            var result = await _reservationLockService.TryLockAsync(key, TimeSpan.FromMinutes(1), owner);

            var resp = new LockResponse
            {
                Success = result.Success,
                Token = result.Token,
                ExpiresAt = result.ExpiresAt,
                Message = result.Message
            };

            return new JsonResult(resp);
        }

        private static DateTime? ParseDate(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;
            return DateTime.TryParseExact(raw, "yyyy-MM-dd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)
                ? dt.Date
                : (DateTime?)null;
        }
    }
}
