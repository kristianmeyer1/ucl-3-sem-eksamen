using Danplanner.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danplanner.Application.Interfaces.UserInterfaces
{
    public interface IUserGetById
    {
        Task<UserDto> GetUserByIdAsync(int userId);

    }
}
