using Danplanner.Application.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Danplanner.Application.Interfaces.UserInterfaces
{
    public interface IUserRepository
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetByUserIdAsync(int userId);
        Task<UserDto?> GetByEmailAsync(string userEmail);
    }
}
