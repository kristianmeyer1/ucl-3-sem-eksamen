using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Danplanner.Application.Models;
using Danplanner.Application.Interfaces.UserInterfaces;

namespace Danplanner.Client.Pages.Admin
{
    public class UsersModel : PageModel
    {
        public List<UserDto> GridData { get; set; } = new List<UserDto>();

        private readonly IUserService _userService;

        public UsersModel(IUserService userService)
        {
            _userService = userService;
        }

        public async Task OnGetAsync()
        {
            GridData = await _userService.GetAllUsersAsync();
        }
    }

    public class GridItemUser : AdminGridItem
    {      
        public string UserAdress { get; set; } = "";
        public string UserMobile { get; set; } = "";
        public string UserEmail { get; set; } = "";
    }
}
