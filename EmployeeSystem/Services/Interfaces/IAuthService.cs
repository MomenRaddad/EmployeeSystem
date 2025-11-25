using System.Threading.Tasks;
using EmployeeSystem.Dtos.Auth;
using Microsoft.AspNetCore.Identity;

namespace EmployeeSystem.Services.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(RegisterDto input);
        Task<TokenResponseDto?> LoginAsync(LoginDto input);
        Task<IdentityResult> AdminCreateUserAsync(AdminCreateUserDto input);
    }
}
