using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Danplanner.Application.Models;
using Danplanner.Domain.Entities;

namespace Danplanner.Application.Interfaces
{
    public interface ITokenService
    {
        string CreateTokenForAdmin(Admin admin);
        string CreateTokenForUser(UserDto user);
    }
}
