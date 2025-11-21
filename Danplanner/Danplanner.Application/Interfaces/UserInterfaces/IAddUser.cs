using Danplanner.Domain.Entities;

namespace Danplanner.Application.Interfaces.UserInterfaces
{
    public interface IAddUser
    {
        Task AddUserAsync(User user);
    }
}
