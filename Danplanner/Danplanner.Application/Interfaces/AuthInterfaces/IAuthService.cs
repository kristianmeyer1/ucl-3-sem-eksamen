using Danplanner.Application.Models.LoginDto;
using Danplanner.Application.Models.ModelsDto;

namespace Danplanner.Application.Interfaces.AuthInterfaces
{
    public interface IAuthService
    {
        Task<AdminDto> RegisterAsync(AdminDto request);
        Task<string?> LoginAsync(LoginDto request);

        Task<UserDto> RegisterUserAsync(UserDto request);

        Task<bool> RequestUserLoginCodeAsync(string email);
        Task<string?> VerifyUserLoginCodeAsync(string email, string code);

        Task<bool> RequestUserRegisterCodeAsync(string email);
        bool VerifyUserRegisterCode(string email, string code);
    }


}
