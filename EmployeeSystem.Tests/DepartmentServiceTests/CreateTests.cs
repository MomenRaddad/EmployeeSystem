using EmployeeSystem.Data;
using EmployeeSystem.Services;
using EmployeeSystem.Models;
using Xunit;

namespace EmployeeSystem.Tests.DepartmentServiceTests;

public class CreateTests : TestBase
{
    // Test ID: DEP-CRT-001 | Priority: High
    [Fact]
    public async Task Create_WithValidDepartment_SavesToDbAndReturns()
    {
        // Arrange
        var db = CreateInMemoryDbContext();
        var service = CreateDepartmentService(db);

        var newDepartment = new DepartmentModel
        {
            Name = "IT",
            DepartmentSupervisor = "Ahmad Ali"
        };

        // Act
        var result = await service.Create(newDepartment);

        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("IT", result.Name);
        Assert.Equal("Ahmad Ali", result.DepartmentSupervisor);

        var fromDb = await db.Departments.FindAsync(result.Id);
        Assert.NotNull(fromDb);
        Assert.Equal(result.Id, fromDb.Id);
        Assert.Equal("IT", fromDb.Name);
        Assert.Equal("Ahmad Ali", fromDb.DepartmentSupervisor);
    }


}
