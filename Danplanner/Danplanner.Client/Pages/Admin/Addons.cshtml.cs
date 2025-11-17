using Danplanner.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Danplanner.Client.Pages.Admin
{
    public class AddonsModel : PageModel
    {
        public List<GridItemAddon> GridData { get; set; } = new List<GridItemAddon>();

        public void OnGet()
        {
            GridData.Add(new GridItemAddon { Id = 1, AddonName = "Bruger Adresse", AddonDescription = "Bruger Tlf"});
        }
    }

    public class GridItemAddon : AdminGridItem
    {
        public string AddonName { get; set; } = "";
        public double AddonPrice { get; set; } = 0;
        public string AddonDescription { get; set; } = "";
    }
}
