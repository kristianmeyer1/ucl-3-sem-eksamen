using Danplanner.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Danplanner.Persistence.DbMangagerDir
{
    public class DbManager : DbContext
    {
        public DbManager(DbContextOptions<DbManager> options) : base(options) { }

        public DbSet<Admin> Admin => Set<Admin>();
        public DbSet<User> User => Set<User>();
        public DbSet<Addon> Addon => Set<Addon>();
        public DbSet<Accommodation> Accommodation => Set<Accommodation>();
    }

}
