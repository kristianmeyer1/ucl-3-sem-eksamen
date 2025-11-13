using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Danplanner.Client.Pages
{
    public class AccommodationModel : PageModel
    {
        private readonly IWebHostEnvironment _env;

        public AccommodationModel(IWebHostEnvironment env)
        {
            _env = env;
        }

        // query string dates (ISO yyyy-MM-dd)
        [BindProperty(SupportsGet = true)]
        public string? Start { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? End { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? DaysQuery { get; set; }

        public string StartDisplay { get; private set; } = "—";
        public string EndDisplay { get; private set; } = "—";
        public int Days { get; private set; } = 0;

        public List<AccommodationItem> Items { get; private set; } = new();

        public void OnGet()
        {
            // parse dates
            DateTime? startDt = null;
            DateTime? endDt = null;
            if (!string.IsNullOrEmpty(Start)
                && DateTime.TryParseExact(Start, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var s))
            {
                startDt = s.Date;
                StartDisplay = s.ToString("d. MMMM", CultureInfo.GetCultureInfo("da-DK"));
            }
            else if (!string.IsNullOrEmpty(Start))
            {
                StartDisplay = Start;
            }

            if (!string.IsNullOrEmpty(End)
                && DateTime.TryParseExact(End, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var e))
            {
                endDt = e.Date;
                EndDisplay = e.ToString("d. MMMM", CultureInfo.GetCultureInfo("da-DK"));
            }
            else if (!string.IsNullOrEmpty(End))
            {
                EndDisplay = End;
            }

            if (startDt.HasValue && endDt.HasValue)
            {
                var computed = Math.Max(0, (endDt.Value - startDt.Value).Days);
                Days = DaysQuery ?? computed;
            }
            else if (DaysQuery.HasValue)
            {
                Days = DaysQuery.Value;
            }

            // load items from text file
            var dataFile = Path.Combine(_env.WebRootPath ?? string.Empty, "data", "accommodations.txt");
            if (System.IO.File.Exists(dataFile))
            {
                var lines = System.IO.File.ReadAllLines(dataFile);
                foreach (var raw in lines)
                {
                    var line = raw?.Trim();
                    if (string.IsNullOrEmpty(line)) continue;
                    if (line.StartsWith("#")) continue; // comment
                    // expected format: key|title|description|price|imagePath
                    var parts = line.Split('|');
                    if (parts.Length < 5)
                    {
                        // skip malformed lines
                        continue;
                    }

                    var item = new AccommodationItem
                    {
                        Key = parts[0].Trim(),
                        Title = parts[1].Trim(),
                        Description = parts[2].Trim(),
                        PriceText = parts[3].Trim(),
                        ImageUrl = parts[4].Trim() // should be relative to webroot, e.g. /images/hytte-a.jpg
                    };
                    Items.Add(item);
                }
            }
            else
            {
                // fallback: add defaults so page still shows something
                Items.Add(new AccommodationItem { Key = "hytteA", Title = "Hytte A", Description = "Hyggelig hytte.", PriceText = "Fra 450 kr / nat", ImageUrl = "/images/hytte-a.jpg" });
                Items.Add(new AccommodationItem { Key = "hytteB", Title = "Hytte B", Description = "Mindre hytte.", PriceText = "Fra 350 kr / nat", ImageUrl = "/images/hytte-b.jpg" });
                Items.Add(new AccommodationItem { Key = "plads", Title = "Plads", Description = "Stor plads.", PriceText = "Fra 200 kr / nat", ImageUrl = "/images/plads.jpg" });
            }
        }

        public class AccommodationItem
        {
            public string Key { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string PriceText { get; set; } = string.Empty;
            public string ImageUrl { get; set; } = string.Empty;
        }
    }
}