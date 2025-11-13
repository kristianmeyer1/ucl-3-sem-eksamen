using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Danplanner.Application.Interfaces;

namespace Danplanner.Client.Pages.Admin
{
    public class UsersModel : PageModel
    {
        public List<GridItemUser> GridData { get; set; } = new List<GridItemUser>();

        public void OnGet()
        {
            GridData.Add(new GridItemUser { Id = 1, UserAdress = "Bruger Adresse", UserMobile = "Bruger Tlf", UserEmail = "Bruger Email" });
            GridData.Add(new GridItemUser { Id = 2, UserAdress = "Bruger Adresse", UserMobile = "Bruger Tlf", UserEmail = "Bruger Email" });

        }
    }

    public class GridItemUser : AdminGridItem
    {      
        public string UserAdress { get; set; } = "";
        public string UserMobile { get; set; } = "";
        public string UserEmail { get; set; } = "";
    }
}
