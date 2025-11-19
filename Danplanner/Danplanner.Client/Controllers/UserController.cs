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
        private readonly IUserGetAll _userGetAll;
        private readonly IUserGetById _userGetById;

        public UserController(IUserGetAll userGetAll, IUserGetById userGetById)
        {
            _userGetAll = userGetAll;
            _userGetById = userGetById;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetUsers()
        {
            var users = await _userGetAll.GetAllUsersAsync();
            return Ok(users);
            // vi returnerer 200 OK med listen af users
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDto?>> GetUserById(int id)
        {
            var user = await _userGetById.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }
    }

}
