using EmployeeSystem.Dtos.Auth;
using EmployeeSystem.Models;

public interface IJwtTokenService
{
    Task<TokenResponseDto> GenerateTokenAsync(ApplicationUser user);
}
