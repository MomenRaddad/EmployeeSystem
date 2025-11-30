using EmployeeSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

namespace EmployeeSystem.Tests.EmployeeServiceTests
{
    public class DeactivateTests:TestBase
    {
        // Test ID: EMP-DEA-001 | Priority: High
        
        [Fact]
        public async Task Deactivate_WhenEmployeeExists_SetsInactiveAndEndDate()
        {
            // Arrange
            var db = CreateInMemoryDbContext();

            db.Departments.Add(new DepartmentModel
            {
                Id = 1,
                Name = "IT",
                DepartmentSupervisor = "Boss"
            });

            db.Employees.Add(new EmployeeModel
            {
                Id = 1,
                FirstName = "Momen",
                LastName = "ActiveUser",
                DepartmentId = 1,
                Position = "Developer",
                DateOfEmployment = new DateTime(2020, 1, 1),
                IsActive = true,
                EndOfServiceDate = null     
            });

            await db.SaveChangesAsync();

            var service = CreateService(db);

            var endDate = new DateTime(2025, 1, 1);

            // Act
            var result = await service.Deactivate(1, endDate);

            // return value
            Assert.True(result);

            // values updated correctly
            var fromDb = await db.Employees.FindAsync(1);
            Assert.NotNull(fromDb);

            Assert.False(fromDb.IsActive);             
            Assert.Equal(endDate, fromDb.EndOfServiceDate); 
        }

        // Test ID: EMP-DEA-002 | Priority: Medium
        [Fact]
        public async Task Deactivate_WhenEmployeeDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var db = CreateInMemoryDbContext();

           
            db.Departments.Add(new DepartmentModel
            {
                Id = 1,
                Name = "IT",
                DepartmentSupervisor = "Boss"
            });

        
            db.Employees.Add(new EmployeeModel
            {
                Id = 1,
                FirstName = "Exists",
                LastName = "User",
                DepartmentId = 1,
                Position = "Developer",
                DateOfEmployment = new DateTime(2020, 1, 1),
                IsActive = true
            });

            await db.SaveChangesAsync();

            var service = CreateService(db);

            // Act
            var result = await service.Deactivate(999, null);

            // return false
            Assert.False(result);

           
            var Employee = await db.Employees.FindAsync(1);
            Assert.NotNull(Employee);
            Assert.True(Employee.IsActive);
            Assert.Null(Employee.EndOfServiceDate);

          
            Assert.Equal(1, await db.Employees.CountAsync());
        }
    }
}
