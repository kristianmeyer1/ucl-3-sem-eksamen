using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Danplanner.Application.Models;

namespace Danplanner.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AdminDto> RegisterAsync(AdminDto request);
        Task<AdminDto> LoginAsync(AdminDto request);
    }
}
