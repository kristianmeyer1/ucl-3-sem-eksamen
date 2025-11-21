using Danplanner.Application.Models;

namespace Danplanner.Application.Interfaces.UserInterfaces
{
    public interface IUserUpdate
    {
        Task<UserDto> UpdateUserAsync(UserDto userDto);
    }
}
