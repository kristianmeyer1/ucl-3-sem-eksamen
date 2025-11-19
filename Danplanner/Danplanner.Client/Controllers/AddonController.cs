using Danplanner.Application.Interfaces.AddonInterfaces;
using Danplanner.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Danplanner.Client.Controllers
{
    [ApiController]
    [Route("api/addon/[controller]")]
    public class AddonController : ControllerBase
    {
        private readonly IAddonRepository _repo;

        public AddonController(IAddonRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<List<AddonDto>>> GetAddons()
        {
            var addons = await _repo.GetAllAddonsAsync();
            return Ok(addons);
        }
    }

}
