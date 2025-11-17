using Danplanner.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Danplanner.Client.Pages.Admin
{
    public class LogModel : PageModel
    {
        public List<GridItemLog> GridData { get; set; } = new List<GridItemLog>();

        public void OnGet()
        {
            GridData.Add(new GridItemLog { Id = 1, LogDescription = "Bruger Adresse"});
        }
    }

    public class GridItemLog : AdminGridItem
    {
        public string LogDescription { get; set; } = "";
        public DateTime LogTimeStamp { get; set; } = new DateTime(2010, 10, 10, 10, 59, 0);
    }

}
