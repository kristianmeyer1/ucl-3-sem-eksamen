using Danplanner.Application.Interfaces.UserInterfaces;
using Danplanner.Application.Models;
using Danplanner.Persistence.DbMangagerDir;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Danplanner.Persistence.Repositories
{
    public class UserRepository : IUserRepository
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

        public async Task<UserDto?> GetByUserIdAsync(int userId)
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

        public async Task<UserDto?> GetByEmailAsync(string userEmail)
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

    }
}
