using Danplanner.Application.Interfaces.UserInterfaces;
using Danplanner.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Danplanner.Client.Pages.Admin
{
    public class UsersModel : PageModel
    {

        private readonly IUserGetAll _userGetAll;
        private readonly IUserGetById _userGetById;

        public UsersModel(IUserGetAll userGetAll, IUserGetById userGetById)
        {
            _userGetAll = userGetAll;
            _userGetById = userGetById;
        }

        [BindProperty]
        public UserDto SelectedUser { get; set; } = new UserDto();

        public List<UserDto> GridData { get; set; } = new List<UserDto>();

        // Henter alle brugere til visning i grid, den bliver kørt i html koden så har derfor ingen klassiske "references"
        public async Task OnGetAsync(int? id)
        {
            GridData = await _userGetAll.GetAllUsersAsync();

            if (id.HasValue)
            {
                SelectedUser = await _userGetById.GetUserByIdAsync(id.Value);
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
