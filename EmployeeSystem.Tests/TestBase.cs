using EmployeeSystem.Data;
using EmployeeSystem.Models;
using EmployeeSystem.Services;
using EmployeeSystem.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmployeeSystem.Tests;

public abstract class TestBase
{
    internal static AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    internal static EmployeeService CreateService(AppDbContext db)
    {
        var loggerMock = new Mock<ILogger<EmployeeService>>();
        var cacheMock = new Mock<IAppCache>();
        return new EmployeeService(db, loggerMock.Object, cacheMock.Object);
    }
    internal static DepartmentService CreateDepartmentService(AppDbContext db)
    {
        var loggerMock = new Mock<ILogger<DepartmentService>>();
        var cacheMock = new Mock<IAppCache>();

        return new DepartmentService(db, loggerMock.Object,cacheMock.Object);
    }


    internal static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        return new Mock<UserManager<ApplicationUser>>(
            store.Object,
            null, null, null, null, null, null, null, null
        );
    }

    internal static Mock<RoleManager<IdentityRole>> CreateRoleManagerMock()
    {
        var store = new Mock<IRoleStore<IdentityRole>>();
        return new Mock<RoleManager<IdentityRole>>(
            store.Object,
            null, null, null, null
        );
    }

    internal static AuthService CreateAuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IJwtTokenService jwtToken,
        ILogger<AuthService> logger)
    {
        return new AuthService(userManager, roleManager, jwtToken, logger);
    }
}
 