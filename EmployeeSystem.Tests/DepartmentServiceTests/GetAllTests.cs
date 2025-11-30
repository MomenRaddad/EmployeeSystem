using EmployeeSystem.Data;
using EmployeeSystem.Services;
using EmployeeSystem.Models;
using Xunit;

namespace EmployeeSystem.Tests.DepartmentServiceTests;

public class GetAllTests : TestBase
{
    // Test ID: DEP-GAL-001 | Priority: Medium
    [Fact]
    public async Task GetAll_WhenDepartmentsExist_ReturnsAllDepartments()
    {
        // Arrange
        var db = CreateInMemoryDbContext();

        db.Departments.AddRange(new List<DepartmentModel>
            {
                new DepartmentModel { Id = 1, Name = "IT",  DepartmentSupervisor = "Ahmad" },
                new DepartmentModel { Id = 2, Name = "HR",  DepartmentSupervisor = "Ali" },
                new DepartmentModel { Id = 3, Name = "FIN", DepartmentSupervisor = "Samer" }
            });

        await db.SaveChangesAsync();

        var service = CreateDepartmentService(db);

        // Act
        var result = await service.GetAll();

        // Assert
        Assert.NotNull(result);
        var list = Assert.IsType<List<DepartmentModel>>(result);
        Assert.Equal(3, list.Count);
    }
   
    // Test ID: DEP-GAL-002 | Priority: Low
    [Fact]
    public async Task GetAll_WhenNoDepartmentsExist_ReturnsEmpty()
    {
        // Arrange
        var db = CreateInMemoryDbContext();
        var service = CreateDepartmentService(db);

   

        // Act
        var result = await service.GetAll();

        // Assert
        Assert.NotNull(result);    
        Assert.Empty(result);      
    }


}
