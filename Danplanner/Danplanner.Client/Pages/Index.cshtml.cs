using Danplanner.Application.Interfaces;
using Danplanner.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class IndexModel : PageModel
{
    private readonly IAdminRepository _adminRepository;

    public List<Admin> Admins { get; private set; } = new List<Admin>();

    public IndexModel(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public async Task OnGetAsync()
    {
        Admins = await _adminRepository.LoadAdminListAsync();
    }
}
