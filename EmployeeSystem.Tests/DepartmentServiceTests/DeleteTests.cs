using EmployeeSystem.Data;
using EmployeeSystem.Models;
using EmployeeSystem.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EmployeeSystem.Tests.DepartmentServiceTests;

public class DeleteTests : TestBase
{
    // Test ID: DEP-DEL-001 | Priority: High
    [Fact]
    public async Task Delete_WhenDepartmentExists_ReturnsTrueAndRemovesFromDb()
    {
        // Arrange
        var db = CreateInMemoryDbContext();

        db.Departments.Add(new DepartmentModel
        {
            Id = 1,
            Name = "IT",
            DepartmentSupervisor = "Supervisor"
        });

        await db.SaveChangesAsync();

        var service = CreateDepartmentService(db);

        // Act
        var result = await service.Delete(1);

        // Assert
        Assert.True(result);

        var deleted = await db.Departments.FindAsync(1);
        Assert.Null(deleted);                 
        Assert.Equal(0, await db.Departments.CountAsync());
    }
    
    // Test ID: DEP-DEL-002 | Priority: Medium
    [Fact]
    public async Task Delete_WhenDepartmentDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var db = CreateInMemoryDbContext();

        db.Departments.Add(new DepartmentModel
        {
            Id = 1,
            Name = "IT",
            DepartmentSupervisor = "Supervisor"
        });

        await db.SaveChangesAsync();

        var service = CreateDepartmentService(db);

        // Act
        var result = await service.Delete(999);

        // Assert
        Assert.False(result);

        var Department = await db.Departments.FindAsync(1);
        Assert.NotNull(Department);          
        Assert.Equal(1, await db.Departments.CountAsync());
    }


}
