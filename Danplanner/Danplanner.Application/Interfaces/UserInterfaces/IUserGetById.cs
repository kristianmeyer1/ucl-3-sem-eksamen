using Danplanner.Application.Models;

namespace Danplanner.Application.Interfaces.UserInterfaces
{
    public interface IUserGetById
    {
        Task<UserDto> GetUserByIdAsync(int userId);
    }
}
