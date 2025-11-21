using Danplanner.Application.Interfaces.UserInterfaces;
using Danplanner.Application.Models;
using Danplanner.Domain.Entities;
using Danplanner.Persistence.DbMangagerDir;
using Microsoft.EntityFrameworkCore;

namespace Danplanner.Persistence.Repositories
{
    public class UserRepository : IUserGetAll, IUserGetById, IUserGetByEmail, IAddUser
    {
        private readonly DbManager _dbManager;

        public UserRepository(DbManager dbManager)
        {
            _dbManager = dbManager;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return await _dbManager.User
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    UserEmail = u.UserEmail,
                    UserMobile = u.UserMobile,
                    UserAdress = u.UserAdress
                })
                .ToListAsync();
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _dbManager.User.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return null;

            return new UserDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                UserEmail = user.UserEmail,
                UserMobile = user.UserMobile,
                UserAdress = user.UserAdress
            };
        }

        public async Task<UserDto?> GetUserByEmailAsync(string userEmail)
        {
            var user = await _dbManager.User.FirstOrDefaultAsync(u => u.UserEmail == userEmail);
            if (user == null) return null;

            return new UserDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                UserEmail = user.UserEmail,
                UserMobile = user.UserMobile,
                UserAdress = user.UserAdress
            };
        }

        public async Task AddUserAsync(User user)
        {
            await _dbManager.User.AddAsync(user);
            await _dbManager.SaveChangesAsync();
        }

    }

}
