using Danplanner.Application.Interfaces.UserInterfaces;
using Danplanner.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Danplanner.Client.Controllers
{
    [ApiController]
    [Route("api/user/[controller]")] // vores route bliver api/user
    public class UserController : ControllerBase
    {
        private readonly IUserGetAll _getAll;
        private readonly IUserGetById _getById;

        public UserController(IUserGetAll getAll, IUserGetById getById)
        {
            _getAll = getAll;
            _getById = getById;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetUsers()
        {
            var users = await _getAll.GetAllUsersAsync();
            return Ok(users);
            // vi returnerer 200 OK med listen af users
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDto?>> GetUserById(int id)
        {
            var user = await _getById.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }
    }

}
