using Danplanner.Application.Interfaces.UserInterfaces;
using Danplanner.Application.Models;
using Danplanner.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Danplanner.Client.Pages.Admin
{
    public class UsersModel : PageModel
    {

        private readonly IUserRepository _userRepo;

        public UsersModel(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        [BindProperty]
        public UserDto SelectedUser { get; set; } = new UserDto();

        public List<UserDto> GridData { get; set; } = new List<UserDto>();

        // Henter alle brugere til visning i grid, den bliver kørt i html koden så har derfor ingen klassiske "references"
        public async Task OnGetAsync(int? id)
        {
            GridData = await _userRepo.GetAllUsersAsync();

            if (id.HasValue)
            {
                SelectedUser = await _userRepo.GetUserByIdAsync(id.Value);
            }
        }
        public IActionResult OnPost()
        {
            // Gem opdaterede værdier fra SelectedUserDto
            // Redirect eller return Page()
            return RedirectToPage("/Admin/Users");
        }

        public IActionResult OnPostDelete()
        {
            // Slet brugeren
            return RedirectToPage("/Admin/Users");
        }

    }
}
