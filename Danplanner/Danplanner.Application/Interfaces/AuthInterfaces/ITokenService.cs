using Danplanner.Application.Models;
using Danplanner.Domain.Entities;

namespace Danplanner.Application.Interfaces.AuthInterfaces
{
    public interface ITokenService
    {
        string CreateTokenForAdmin(Admin admin);
        string CreateTokenForUser(UserDto user);
    }
}
