using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Danplanner.Domain.Entities;

namespace Danplanner.Application.Interfaces
{
    public interface IAdminRepository
    {
        Task<List<Admin>> LoadAdminListAsync();
    }
}
