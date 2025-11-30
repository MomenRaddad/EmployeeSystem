using EmployeeSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

namespace EmployeeSystem.Tests.EmployeeServiceTests
{
    public class DeleteTests:TestBase
    {
        // Test ID: EMP-DEL-001 | Priority: High
        [Fact]
        public async Task Delete_WhenEmployeeExists_ReturnsTrueAndRemovesFromDb()
        {
            // Arrange
            var db = CreateInMemoryDbContext();

            // Seed department (required for Employee)
            db.Departments.Add(new DepartmentModel
            {
                Id = 1,
                Name = "IT",
                DepartmentSupervisor = "Boss"
            });

            // Seed employee to delete
            db.Employees.Add(new EmployeeModel
            {
                Id = 1,
                FirstName = "Momen",
                LastName = "Test",
                Position = "Developer",
                DepartmentId = 1,
                DateOfEmployment = new DateTime(2020, 1, 1),
                IsActive = true
            });

            await db.SaveChangesAsync();

            var service = CreateService(db);

            // Act
            var result = await service.Delete(1);

            // service returned true
            Assert.True(result);

            // employee removed from DB
            var deleted = await db.Employees.FindAsync(1);
            Assert.Null(deleted);

            // DB contains 0 employees
            Assert.Empty(db.Employees);
        }
        
        // Test ID: EMP-DEL-002 | Priority: Medium
        [Fact]
        public async Task Delete_WhenEmployeeDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var db = CreateInMemoryDbContext();

            // Add a valid department 
            db.Departments.Add(new DepartmentModel
            {
                Id = 1,
                Name = "IT",
                DepartmentSupervisor = "Boss"
            });

            // Add one employee only with Id=1
            db.Employees.Add(new EmployeeModel
            {
                Id = 1,
                FirstName = "Existing",
                LastName = "Employee",
                Position = "Developer",
                DepartmentId = 1,
                DateOfEmployment = new DateTime(2020, 1, 1),
                IsActive = true
            });

            await db.SaveChangesAsync();

            var service = CreateService(db);

            // Act
            var result = await service.Delete(999);

            // return value
            Assert.False(result);

            // employee with Id=1 still exists
            var ExistsEmployee = await db.Employees.FindAsync(1);
            Assert.NotNull(ExistsEmployee);

            // count unchanged (still 1 employee)
            Assert.Equal(1, await db.Employees.CountAsync());
        }



    }
}
