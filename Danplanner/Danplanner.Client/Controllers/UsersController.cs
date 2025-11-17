using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Danplanner.Persistence.Repositories;
using Danplanner.Application.Models;
using Danplanner.Application.Interfaces.UserInterfaces;

namespace Danplanner.Infrastructure.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repo;

        public UsersController(IUserRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetUsers()
        {
            var users = await _repo.GetAllUsersAsync();
            return Ok(users);
        }
    }

}
