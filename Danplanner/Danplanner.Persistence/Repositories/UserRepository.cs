using Danplanner.Application.Interfaces.UserInterfaces;
using Danplanner.Application.Models;
using Danplanner.Domain.Entities;
using Danplanner.Persistence.DbMangagerDir;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    UserAdress = u.UserAdress,
                    UserMobile = u.UserMobile,
                    UserEmail = u.UserEmail
                })
                .ToListAsync();
        }

    }
}
