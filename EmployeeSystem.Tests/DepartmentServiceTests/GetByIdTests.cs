using EmployeeSystem.Data;
using EmployeeSystem.Services;
using EmployeeSystem.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EmployeeSystem.Tests.DepartmentServiceTests;

public class GetByIdTests : TestBase
{
    // Test ID: DEP-GET-001 | Priority: High
    [Fact]
    public async Task GetById_WhenDepartmentExists_ReturnsDepartment()
    {
        // Arrange
        var db = CreateInMemoryDbContext();

        db.Departments.Add(new DepartmentModel
        {
            Id = 1,
            Name = "IT",
            DepartmentSupervisor = "Ahmad Ali"
        });

        await db.SaveChangesAsync();

        var service = CreateDepartmentService(db);

        // Act
        var result = await service.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("IT", result.Name);
        Assert.Equal("Ahmad Ali", result.DepartmentSupervisor);
    }

    // Test ID: DEP-GET-002 | Priority: High
    [Fact]
    public async Task GetById_WhenDepartmentDoesNotExist_ReturnsNull()
    {
        // Arrange
        var db = CreateInMemoryDbContext();

        db.Departments.Add(new DepartmentModel
        {
            Id = 1,
            Name = "HR",
            DepartmentSupervisor = "Sara Manager"
        });

        await db.SaveChangesAsync();

        var service = CreateDepartmentService(db);

        // Act
        var result = await service.GetById(999);

        // Assert
        Assert.Null(result);
    }


}
