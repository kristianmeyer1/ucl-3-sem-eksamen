using Danplanner.Domain.Entities;

namespace Danplanner.Application.Interfaces.UserInterfaces
{
    public interface IUserAdd
    {
        Task AddUserAsync(User user);
    }
}
