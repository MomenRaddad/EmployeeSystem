using EmployeeSystem.Data;
using EmployeeSystem.Services;
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
        return new EmployeeService(db, loggerMock.Object);
    }
    internal static DepartmentService CreateDepartmentService(AppDbContext db)
    {
        var loggerMock = new Mock<ILogger<DepartmentService>>();
        return new DepartmentService(db, loggerMock.Object);
    }
}
 