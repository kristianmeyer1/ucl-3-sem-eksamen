using Danplanner.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danplanner.Application.Interfaces.AddonInterfaces
{
    public interface IAddonGetAll
    {
        Task<List<AddonDto>> GetAllAddonsAsync();
    }
}
