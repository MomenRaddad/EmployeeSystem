using EmployeeSystem.Data;
using EmployeeSystem.Services;
using EmployeeSystem.Models;
using Xunit;

namespace EmployeeSystem.Tests.DepartmentServiceTests;

public class GetEmployeesTests : TestBase
{
    // Test ID: DEP-EMP-001 | Priority: High
    [Fact]
    public async Task GetEmployees_WhenDepartmentHasEmployees_ReturnsOnlyThatDepartmentsEmployees()
    {
        // Arrange
        var db = CreateInMemoryDbContext();

        db.Departments.Add(new DepartmentModel
        {
            Id = 1,
            Name = "IT",
            DepartmentSupervisor = "Boss1"
        });

        db.Employees.AddRange(
            new EmployeeModel
            {
                Id = 1,
                FirstName = "First",
                LastName = "One",
                DepartmentId = 1,
                Position = "Dev",
                DateOfEmployment = new System.DateTime(2020, 1, 1),
                IsActive = true
            },
            new EmployeeModel
            {
                Id = 2,
                FirstName = "Sec",
                LastName = "Two",
                DepartmentId = 1,
                Position = "Tester",
                DateOfEmployment = new System.DateTime(2021, 1, 1),
                IsActive = true
            },
            new EmployeeModel
            {
                Id = 3,
                FirstName = "Th",
                LastName = "Three",
                DepartmentId = 2,
                Position = "HR",
                DateOfEmployment = new System.DateTime(2019, 1, 1),
                IsActive = true
            }
        );

        await db.SaveChangesAsync();

        var service = CreateDepartmentService(db);

        // Act
        var result = await service.GetEmployees(1);
        var list = result.ToList();

        // Assert
        Assert.Equal(2, list.Count);
        Assert.All(list, e => Assert.Equal(1, e.DepartmentId));
    }
    
    // Test ID: DEP-EMP-002 | Priority: Medium
    [Fact]
    public async Task GetEmployees_WhenDepartmentHasNoEmployees_ReturnsEmpty()
    {
        // Arrange
        var db = CreateInMemoryDbContext();

        db.Departments.Add(new DepartmentModel
        {
            Id = 2,
            Name = "HR",
            DepartmentSupervisor = "Ali"
        });

        await db.SaveChangesAsync();

        var service = CreateDepartmentService(db);

        // Act
        var result = await service.GetEmployees(2);
        var list = result.ToList();

        // Assert
        Assert.Empty(list);
    }

    // Test ID: DEP-EMP-003 | Priority: Low
    [Fact]
    public async Task GetEmployees_WhenDepartmentDoesNotExist_ReturnsEmptyOrHandled()
    {
        // Arrange
        var db = CreateInMemoryDbContext();
        var service = CreateDepartmentService(db);


        // Act
        var result = await service.GetEmployees(999);
        var list = result.ToList();

        // Assert
        Assert.Empty(list); 
    }

}
