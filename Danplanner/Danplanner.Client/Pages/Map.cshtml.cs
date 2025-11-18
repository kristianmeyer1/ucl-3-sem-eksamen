using Danplanner.Application.Interfaces.AccommodationInterfaces;
using Danplanner.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Danplanner.Client.Pages
{
    public class MapModel : PageModel
    {
        private readonly IWebHostEnvironment _env;
        private readonly IAccommodationService _accommodationService;

        public MapModel(IWebHostEnvironment env, IAccommodationService accommodationService)
        {
            _env = env;
            _accommodationService = accommodationService;
        }

        [BindProperty(SupportsGet = true)]
        public string? Start { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? End { get; set; }

        public string FilteredMapJson { get; private set; } = "{}";
        public string? SelectedKey { get; private set; }

        public void OnGet()
        {
            var dataDir = Path.Combine(_env.WebRootPath ?? string.Empty, "data");
            var mapFile = Path.Combine(dataDir, "map.json");
            MapDefinition mapDef = null;

            if (System.IO.File.Exists(mapFile))
            {
                var raw = System.IO.File.ReadAllText(mapFile);
                try
                {
                    mapDef = JsonSerializer.Deserialize<MapDefinition>(raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new MapDefinition();
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

            SelectedKey = Request.Cookies["selectedItem"];
            var qSelected = Request.Query["selected"].FirstOrDefault();
            if (!string.IsNullOrEmpty(qSelected)) SelectedKey = qSelected;

            FilteredMapJson = JsonSerializer.Serialize(mapDef);
        }
    }
}