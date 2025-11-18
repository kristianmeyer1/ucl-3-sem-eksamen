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
    [Route("api/user/[controller]")] // vores route bliver api/user
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repo;

        public UserController(IUserRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetUsers()
        {
            var users = await _repo.GetAllUsersAsync();
            return Ok(users);
            // vi returnerer 200 OK med listen af users
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDto?>> GetUserById(int id)
        {
            var user = await _repo.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }
    }

}
