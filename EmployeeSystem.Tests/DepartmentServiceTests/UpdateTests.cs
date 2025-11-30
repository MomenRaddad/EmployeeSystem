using EmployeeSystem.Data;
using EmployeeSystem.Services;
using EmployeeSystem.Models;
using Xunit;

namespace EmployeeSystem.Tests.DepartmentServiceTests;

public class UpdateTests : TestBase
{
    // Test ID: DEP-UPD-001 | Priority: High
    [Fact]
    public async Task Update_WhenDepartmentExists_ReturnsTrueAndUpdates()
    {
        // Arrange
        var db = CreateInMemoryDbContext();

        db.Departments.Add(new DepartmentModel
        {
            Id = 1,
            Name = "Old Name",
            DepartmentSupervisor = "Old Supervisor"
        });

        await db.SaveChangesAsync();

        var service = CreateDepartmentService(db);

        var updatedDepartment = new DepartmentModel
        {
            Name = "New Name",
            DepartmentSupervisor = "New Supervisor"
        };

        // Act
        var result = await service.Update(1, updatedDepartment);

       
        Assert.True(result);

        
        var fromDb = await db.Departments.FindAsync(1);
        Assert.NotNull(fromDb);
        Assert.Equal("New Name", fromDb.Name);
        Assert.Equal("New Supervisor", fromDb.DepartmentSupervisor);
    }
    // Test ID: DEP-UPD-002 | Priority: High
    [Fact]
    public async Task Update_WhenDepartmentDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var db = CreateInMemoryDbContext();

        
        db.Departments.Add(new DepartmentModel
        {
            Id = 1,
            Name = "HR",
            DepartmentSupervisor = "Ali"
        });

        await db.SaveChangesAsync();

        var service = CreateDepartmentService(db);

        var updatedDepartment = new DepartmentModel
        {
            Name = "New",
            DepartmentSupervisor = "New"
        };

        // Act
        var result = await service.Update(999, updatedDepartment);

        // Assert
        Assert.False(result);

       
        var fromDb = await db.Departments.FindAsync(1);
        Assert.NotNull(fromDb);
        Assert.Equal("HR", fromDb.Name);
        Assert.Equal("Ali", fromDb.DepartmentSupervisor);
    }


}
