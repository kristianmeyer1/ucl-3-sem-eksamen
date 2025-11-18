using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Danplanner.Application.Models;
using Danplanner.Application.Interfaces.UserInterfaces;

namespace Danplanner.Client.Pages.Admin
{
    public class UsersModel : PageModel
    {
        public List<UserDto> GridData { get; set; } = new List<UserDto>();

        private readonly IUserRepository _repo;

        public UsersModel(IUserRepository userService)
        {
            _repo = userService;
        }

        // Henter alle brugere til visning i grid, den bliver kørt i html koden så har derfor ingen klassiske "references"
        public async Task OnGetAsync()
        {
            GridData = await _repo.GetAllUsersAsync();
        }
    }
}
