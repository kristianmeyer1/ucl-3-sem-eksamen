using Danplanner.Application.Interfaces.AddonInterfaces;
using Danplanner.Application.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Danplanner.Client.Pages
{
    public class ConfirmationModel : PageModel
    {
        private readonly IAddonGetAll _repo;
        public AccommodationDto SelectedAccommodation { get; set; }
        public List<AddonDto> Addons { get; set; }
        public string StartDisplay { get; set; }
        public string EndDisplay { get; set; }
        public int Days { get; set; }
        public string TotalPriceDisplay { get; set; }
        public ConfirmationModel(IAddonGetAll repo)
        {
            _repo = repo;
        }
        public void OnGet()
        {
            Addons = _repo.GetAllAddonsAsync().Result;
        }
    }
}
