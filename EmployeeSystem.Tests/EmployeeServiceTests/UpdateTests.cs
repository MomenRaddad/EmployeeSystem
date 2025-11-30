using EmployeeSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

namespace EmployeeSystem.Tests.EmployeeServiceTests
{
    public class UpdateTests: TestBase
    {
        // Test ID: EMP-UPD-001 |  Priority: High
        [Fact]
        public async Task Update_WithValidData_ReturnsTrueAndUpdatesEmployee()
        {
            // Arrange
            var db = CreateInMemoryDbContext();

            // existing department (valid DepartmentId)
            db.Departments.Add(new DepartmentModel
            {
                Id = 1,
                Name = "IT",
                DepartmentSupervisor = "Ali"
            });

            // existing employee to update
            db.Employees.Add(new EmployeeModel
            {
                Id = 1,
                FirstName = "OldFirst",
                LastName = "OldLast",
                DateOfBirth = new DateTime(1999, 1, 1),
                DateOfEmployment = new DateTime(2020, 1, 1),
                EndOfServiceDate = null,
                Position = "Junior Dev",
                DepartmentId = 1,
                IsActive = true
            });

            await db.SaveChangesAsync();

            var service = CreateService(db);

            // updated data (valid update model + same valid DepartmentId)
            var updatedEmployee = new EmployeeModel
            {
                FirstName = "NewFirst",
                LastName = "NewLast",
                DateOfBirth = new DateTime(1998, 5, 10),
                DateOfEmployment = new DateTime(2021, 2, 1),
                EndOfServiceDate = new DateTime(2025, 1, 1),
                Position = "Senior Dev",
                DepartmentId = 1,     
                IsActive = false     
            };

            // Act
            var result = await service.Update(1, updatedEmployee);

            // returns true
            Assert.True(result);

            // DB fields updated
            var fromDb = await db.Employees.FindAsync(1);
            Assert.NotNull(fromDb);
            Assert.Equal(updatedEmployee.FirstName, fromDb!.FirstName);
            Assert.Equal(updatedEmployee.LastName, fromDb.LastName);
            Assert.Equal(updatedEmployee.DateOfBirth, fromDb.DateOfBirth);
            Assert.Equal(updatedEmployee.DateOfEmployment, fromDb.DateOfEmployment);
            Assert.Equal(updatedEmployee.EndOfServiceDate, fromDb.EndOfServiceDate);
            Assert.Equal(updatedEmployee.Position, fromDb.Position);
            Assert.Equal(updatedEmployee.DepartmentId, fromDb.DepartmentId);
            Assert.Equal(updatedEmployee.IsActive, fromDb.IsActive);
        }

        // Test ID: EMP-UPD-002 |  Priority: High

        [Fact]
        public async Task Update_WithNonExistingEmployee_ReturnsFalse()
        {
            // Arrange
            var db = CreateInMemoryDbContext();

            // Existing department + one existing employee (not the one we will try to update)
            db.Departments.Add(new DepartmentModel
            {
                Id = 1,
                Name = "IT",
                DepartmentSupervisor = "Ahmad"
            });

            db.Employees.Add(new EmployeeModel
            {
                Id = 1,
                FirstName = "Original",
                LastName = "Employee",
                DepartmentId = 1,
                Position = "Developer",
                DateOfEmployment = new DateTime(2020, 1, 1),
                IsActive = true
            });

            await db.SaveChangesAsync();

            var service = CreateService(db);

            // This model is fine, but the Id we will  pass (999) does not exist in DB
            var updatedEmployee = new EmployeeModel
            {
                FirstName = "Updated",
                LastName = "Name",
                DepartmentId = 1,
                Position = "Senior Dev",
                DateOfEmployment = new DateTime(2021, 1, 1),
                IsActive = false
            };

            // Act
            var result = await service.Update(999, updatedEmployee);

            // method returns false
            Assert.False(result);

            // no DB changes 
            var fromDb = await db.Employees.FindAsync(1);
            Assert.NotNull(fromDb);
            Assert.Equal("Original", fromDb!.FirstName);
            Assert.Equal("Employee", fromDb.LastName);
            Assert.Equal(1, fromDb.DepartmentId);
            Assert.Equal("Developer", fromDb.Position);
            Assert.True(fromDb.IsActive);

            // still only 1 employee in DB
            Assert.Equal(1, await db.Employees.CountAsync());
        }
       
        // Test ID: EMP-UPD-003 |  Priority: High

        [Fact]
        public async Task Update_WithInvalidDepartment_ThrowsInvalidOperation()
        {
            // Arrange
            var db = CreateInMemoryDbContext();

            // valid department with Id=1
            db.Departments.Add(new DepartmentModel
            {
                Id = 1,
                Name = "IT",
                DepartmentSupervisor = "Boss"
            });

            // existing employee
            db.Employees.Add(new EmployeeModel
            {
                Id = 1,
                FirstName = "Old",
                LastName = "Employee",
                DepartmentId = 1,
                Position = "Developer",
                DateOfEmployment = new DateTime(2020, 1, 1),
                IsActive = true
            });

            await db.SaveChangesAsync();

            var service = CreateService(db);

            // update model with INVALID DepartmentId 
            var updatedEmployee = new EmployeeModel
            {
                FirstName = "Updated",
                LastName = "Changed",
                DepartmentId = 999, 
                Position = "Senior Dev",
                DateOfEmployment = new DateTime(2022, 2, 2),
                IsActive = true
            };

            // Act + Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.Update(1, updatedEmployee));

            Assert.Equal("Department not found.", ex.Message);
        }

    }


}
