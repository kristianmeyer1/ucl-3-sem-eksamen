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
