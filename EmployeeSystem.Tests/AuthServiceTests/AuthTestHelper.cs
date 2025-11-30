using EmployeeSystem.Dtos.Auth;
using EmployeeSystem.Models;

namespace EmployeeSystem.Tests.AuthServiceTests;

public static class AuthTestHelper
{
    public static RegisterDto CreateValidRegisterDto(string email = "test@example.com")
        => new RegisterDto { Email = email, Password = "StrongP@ss1" };

    public static LoginDto CreateValidLoginDto(string email = "test@example.com")
        => new LoginDto { Email = email, Password = "StrongP@ss1" };

    public static ApplicationUser CreateUser(string email = "test@example.com")
        => new ApplicationUser { Email = email, UserName = email };
}
