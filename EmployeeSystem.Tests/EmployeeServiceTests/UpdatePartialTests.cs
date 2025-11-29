using EmployeeSystem.Dtos;
using EmployeeSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace EmployeeSystem.Tests.EmployeeServiceTests
{
    public class UpdatePartialTests: TestBase
    {
        // Test ID: EMP-UPD-006 | Priority: High
        [Fact]
        public async Task UpdatePartial_WithInvalidDepartment_ReturnsErrorMessage()
        {
            // Arrange
            var db = CreateInMemoryDbContext();

            // valid existing department
            db.Departments.Add(new DepartmentModel
            {
                Id = 1,
                Name = "IT",
                DepartmentSupervisor = "Boss"
            });

            // existing employee linked to valid department 1
            db.Employees.Add(new EmployeeModel
            {
                Id = 1,
                FirstName = "Momen",
                LastName = "Original",
                DepartmentId = 1,
                Position = "Developer",
                DateOfEmployment = new DateTime(2020, 1, 1),
                IsActive = true
            });

            await db.SaveChangesAsync();

            var service = CreateService(db);

            var dto = new UpdateEmployeeDto
            {
                DepartmentId = 999,         
                FirstName = "NewName",      
                Position = "Senior Dev"
            };

            // Act
            var (success, error, notFound) = await service.UpdatePartial(1, dto);

            // Assert - flags
            Assert.False(success);
            Assert.False(notFound);
            Assert.Equal("Department not found.", error);

            // Assert - employee in DB not changed
            var fromDb = await db.Employees.FindAsync(1);
            Assert.NotNull(fromDb);         
            Assert.Equal(1, fromDb.DepartmentId);           
        }

        // Test ID: EMP-UPD-004 | Priority: High
         [Fact]
        public async Task UpdatePartial_WithPartialFields_UpdatesOnlyThoseFields()
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

            var dto = new UpdateEmployeeDto
            {
                FirstName = "NewFirst",
                Position = "Senior Dev"
              
            };

            // Act
            var (success, error, notFound) = await service.UpdatePartial(1, dto);

            // Assert - flags
            Assert.True(success);
            Assert.False(notFound);
            Assert.Null(error);

            // employee in DB updated correctly
            var fromDb = await db.Employees.FindAsync(1);
            Assert.NotNull(fromDb);

            Assert.Equal("NewFirst", fromDb!.FirstName);
            Assert.Equal("Senior Dev", fromDb.Position);

            // Old feilds should remain unchanged
            Assert.Equal("OldLast", fromDb.LastName);
            Assert.Equal(new DateTime(1999, 1, 1), fromDb.DateOfBirth);
            Assert.Equal(new DateTime(2020, 1, 1), fromDb.DateOfEmployment);
            Assert.Null(fromDb.EndOfServiceDate);
            Assert.Equal(1, fromDb.DepartmentId);
            Assert.True(fromDb.IsActive);
        }

        // Test ID: EMP-UPD-005 | Priority: High
        [Fact]
        public async Task UpdatePartial_WhenEmployeeDoesNotExist_ReturnsNotFound()
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
                FirstName = "Existing",
                LastName = "Employee",
                DepartmentId = 1,
                Position = "Developer",
                DateOfEmployment = new DateTime(2020, 1, 1),
                IsActive = true
            });

            await db.SaveChangesAsync();

            var service = CreateService(db);

            var dto = new UpdateEmployeeDto
            {
                FirstName = "NewName",
                Position = "Senior Dev"
            };

            // Act
            var (success, error, notFound) = await service.UpdatePartial(999, dto);

            Assert.False(success);     
            Assert.True(notFound);      
            Assert.Null(error);         

            var fromDb = await db.Employees.FindAsync(1);
            Assert.NotNull(fromDb);
            Assert.Equal("Existing", fromDb!.FirstName);
            Assert.Equal("Employee", fromDb.LastName);
            Assert.Equal(1, fromDb.DepartmentId);
            Assert.Equal("Developer", fromDb.Position);
            Assert.True(fromDb.IsActive);

         
            Assert.Equal(1, await db.Employees.CountAsync());
        }

    }
}
