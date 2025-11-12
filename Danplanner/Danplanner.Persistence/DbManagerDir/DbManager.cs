using Danplanner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Danplanner.Persistence.DbMangagerDir
{
   public class DbManager : DbContext
{
    public DbManager(DbContextOptions<DbManager> options) : base(options) { }

    public DbSet<Admin> Admin => Set<Admin>();
}

}
