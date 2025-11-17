using System.Text.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace Danplanner.Client.Pages
{
    public class MapModel : PageModel
    {
        private readonly IWebHostEnvironment _env;

        public MapModel(IWebHostEnvironment env)
        {
            _env = env;
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

            //var accomFile = Path.Combine(_env.WebRootPath ?? string.Empty, "data", "accommodations.txt");
            //HashSet<string> availableKeys = new(StringComparer.OrdinalIgnoreCase);
            //if (System.IO.File.Exists(accomFile))
            //{
            //    var lines = System.IO.File.ReadAllLines(accomFile);
            //    foreach (var raw in lines)
            //    {
            //        var line = raw?.Trim();
            //        if (string.IsNullOrEmpty(line)) continue;
            //        if (line.StartsWith("#")) continue;
            //        var parts = line.Split('|');
            //        if (parts.Length < 1) continue;
            //        var key = parts[0].Trim();
            //        if (!string.IsNullOrEmpty(key)) availableKeys.Add(key);
            //    }
            //}

            //if (mapDef.Points != null && availableKeys.Any())
            //{
            //    mapDef.Points = mapDef.Points.Where(p => !string.IsNullOrEmpty(p.Key) && availableKeys.Contains(p.Key)).ToArray();
            //}

            SelectedKey = Request.Cookies["selectedItem"];
            var qSelected = Request.Query["selected"].FirstOrDefault();
            if (!string.IsNullOrEmpty(qSelected)) SelectedKey = qSelected;

            FilteredMapJson = JsonSerializer.Serialize(mapDef);
        }

        public class MapDefinition
        {
            public string ImageUrl { get; set; } = string.Empty;
            public int Width { get; set; }
            public int Height { get; set; }
            public MapPoint[] Points { get; set; } = Array.Empty<MapPoint>();
        }

        public class MapPoint
        {
            public string Key { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string? Description { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public string? ImageUrl { get; set; }
        }
    }
}