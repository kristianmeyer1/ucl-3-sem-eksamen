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
        public DbSet<Admin> Admins { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "Server=mysql80.unoeuro.com;Port=3306;Database=danlinedanser_dk_db;Uid=danlinedanser_dk;Pwd=DhxrtHRg345wnyfdeFb9;";
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
