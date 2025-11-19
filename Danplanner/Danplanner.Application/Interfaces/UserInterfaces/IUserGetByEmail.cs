using Danplanner.Application.Models;

namespace Danplanner.Application.Interfaces.UserInterfaces
{
    public interface IUserGetByEmail
    {
        Task<UserDto?> GetUserByEmailAsync(string userEmail);
    }
}
