using EmployeeSystem.Dtos.Auth;
using EmployeeSystem.Models;
using System.Threading.Tasks;

public interface IJwtTokenService
{
    Task<TokenResponseDto> GenerateTokenAsync(ApplicationUser user);
}
