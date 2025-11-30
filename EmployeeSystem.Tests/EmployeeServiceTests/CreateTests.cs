using EmployeeSystem.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Threading.Tasks;
using Xunit;

namespace EmployeeSystem.Tests.EmployeeServiceTests
{
    public class CreateTests : TestBase
    {
        // Test ID: EMP-CRT-001 | Priority: High

        [Fact]
        public async Task Create_WithValidEmployee_SavesToDbAndReturns()
        {
            var db = CreateInMemoryDbContext();
            db.Departments.Add(new EmployeeSystem.Models.DepartmentModel
            {
                Id = 1,
                Name = "IT",
                DepartmentSupervisor = "Ahmad Ali",
            });
            db.SaveChanges();
            var service = CreateService(db);

            var employee = new EmployeeModel
            {
                FirstName = "Momen",
                LastName = "Test",
                DepartmentId = 1,
                Position = "Developer",
                DateOfEmployment = new DateTime(2020, 1, 1),
                EndOfServiceDate = null // so IsActive should become true
            };

            // Act
            var result = await service.Create(employee);

            // Id > 0
            Assert.NotNull(result);
            Assert.True(result.Id > 0);

            // values match input 
            Assert.Equal(employee.FirstName, result.FirstName);
            Assert.Equal(employee.LastName, result.LastName);
            Assert.Equal(employee.DepartmentId, result.DepartmentId);
            Assert.Equal(employee.Position, result.Position);
            Assert.Equal(employee.DateOfEmployment, result.DateOfEmployment);

            // saved in DB with same values
            var fromDb = await db.Employees.FindAsync(result.Id);
            Assert.NotNull(fromDb);
            Assert.True(fromDb.Id > 0);
            Assert.Equal(employee.FirstName, fromDb.FirstName);
            Assert.Equal(employee.LastName, fromDb.LastName);
            Assert.Equal(employee.DepartmentId, fromDb.DepartmentId);
            Assert.Equal(employee.Position, fromDb.Position);
            Assert.Equal(employee.DateOfEmployment, fromDb.DateOfEmployment);

        }

        // Test ID: EMP-CRT-002 | Priority: High
        [Fact]
        public async Task Create_WithInvalidDepartment_ThrowsInvalidOperation()
        {
            // Arrange
            var db = CreateInMemoryDbContext();

            // No departments are added here 
            // so any DepartmentId we use will be invalid.
            var service = CreateService(db);

            var employee = new EmployeeModel
            {
                FirstName = "Momen",
                LastName = "Test",
                DepartmentId = 999,
                Position = "Developer",
                DateOfEmployment = new DateTime(2020, 1, 1),
                EndOfServiceDate = null
            };

            // Act + Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.Create(employee));

            // verify message matches service behavior
            Assert.Equal("Department not found.", ex.Message);
        }
       
     
    }
}
