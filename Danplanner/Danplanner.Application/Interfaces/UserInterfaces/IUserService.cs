using Danplanner.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danplanner.Application.Interfaces.UserInterfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsersAsync();
    }
}
