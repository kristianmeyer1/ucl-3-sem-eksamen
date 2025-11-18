using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Danplanner.Application.Models;
using Danplanner.Application.Models.LoginDto;

namespace Danplanner.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AdminDto> RegisterAsync(AdminDto request);
        //Task<AdminDto> LoginAsync(AdminDto request);
        Task<string?> LoginAsync(LoginDto request);

        Task<bool> RequestUserLoginCodeAsync(string email);
        Task<string?> VerifyUserLoginCodeAsync(string email, string code);
    }


}
