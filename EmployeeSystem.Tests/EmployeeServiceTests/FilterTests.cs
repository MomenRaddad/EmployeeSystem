using EmployeeSystem.Dtos;
using EmployeeSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

namespace EmployeeSystem.Tests.EmployeeServiceTests
{
    public class FilterTests:TestBase
    {
        // Test ID: EMP-FLT-001 | Priority: High
        [Fact]
        public async Task FilterEmployees_ByDepartmentId_ReturnsOnlyThatDepartment()
        {
            // Arrange
            var db = CreateInMemoryDbContext();

            
            db.Departments.AddRange(
                new DepartmentModel { Id = 1, Name = "IT", DepartmentSupervisor = "Boss" },
                new DepartmentModel { Id = 2, Name = "HR", DepartmentSupervisor = "Manager" }
            );

            
            db.Employees.AddRange(
                new EmployeeModel
                {
                    Id = 1,
                    FirstName = "Momen",
                    LastName = "IT1",
                    DepartmentId = 1,
                    Position = "Dev",
                    DateOfEmployment = new DateTime(2020, 1, 1),
                    IsActive = true
                },
                new EmployeeModel
                {
                    Id = 2,
                    FirstName = "Sara",
                    LastName = "IT2",
                    DepartmentId = 1,
                    Position = "Tester",
                    DateOfEmployment = new DateTime(2021, 1, 1),
                    IsActive = true
                },
                new EmployeeModel
                {
                    Id = 3,
                    FirstName = "Ali",
                    LastName = "HR1",
                    DepartmentId = 2,
                    Position = "HR",
                    DateOfEmployment = new DateTime(2019, 1, 1),
                    IsActive = true
                }
            );

            await db.SaveChangesAsync();

            var service = CreateService(db);

            var filter = new EmployeeFilter
            {
                DepartmentId = 1
            };

            // Act
            var result = await service.FilterEmployees(filter);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            foreach (var emp in result)
            {
                Assert.Equal(1, emp.DepartmentId); 
            }
        }
       
        // Test ID: EMP-FLT-002 | Priority: High
        [Fact]
        public async Task FilterEmployees_ByIsActiveFalse_ReturnsOnlyInactive()
        {
            // Arrange
            var db = CreateInMemoryDbContext();

            db.Departments.AddRange(
                new DepartmentModel { Id = 1, Name = "IT", DepartmentSupervisor = "Boss" },
                new DepartmentModel { Id = 2, Name = "HR", DepartmentSupervisor = "Manager" }
            );

            db.Employees.AddRange(
                new EmployeeModel
                {
                    Id = 1,
                    FirstName = "Active1",
                    LastName = "IT",
                    DepartmentId = 1,
                    Position = "Dev",
                    DateOfEmployment = new DateTime(2020, 1, 1),
                    IsActive = true
                },
                new EmployeeModel
                {
                    Id = 2,
                    FirstName = "Inactive1",
                    LastName = "IT",
                    DepartmentId = 1,
                    Position = "Tester",
                    DateOfEmployment = new DateTime(2021, 1, 1),
                    IsActive = false
                },
                new EmployeeModel
                {
                    Id = 3,
                    FirstName = "Active2",
                    LastName = "HR",
                    DepartmentId = 2,
                    Position = "HR",
                    DateOfEmployment = new DateTime(2019, 1, 1),
                    IsActive = true
                },
                new EmployeeModel
                {
                    Id = 4,
                    FirstName = "Inactive2",
                    LastName = "HR",
                    DepartmentId = 2,
                    Position = "HR",
                    DateOfEmployment = new DateTime(2018, 1, 1),
                    IsActive = false
                }
            );

            await db.SaveChangesAsync();

            var service = CreateService(db);

            var filter = new EmployeeFilter
            {
                IsActive = false   
            };

            // Act
            var result = await service.FilterEmployees(filter);
            var list = result.ToList();

            // Inactive only 
            Assert.NotNull(list);
            Assert.Equal(2, list.Count);          
            Assert.All(list, e => Assert.False(e.IsActive));

           
            Assert.Equal(4, await db.Employees.CountAsync());
        }
        
        // Test ID: EMP-FLT-003 | Priority: Medium
        [Fact]
        public async Task FilterEmployees_ByMinYearsOfService_ReturnsEmployeesAboveMin()
        {
            // Arrange
            var db = CreateInMemoryDbContext();

            db.Departments.Add(new DepartmentModel
            {
                Id = 1,
                Name = "IT",
                DepartmentSupervisor = "Boss"
            });

            db.Employees.AddRange(
                new EmployeeModel
                {
                    Id = 1,
                    FirstName = "Emp1",
                    LastName = "Y1",
                    DepartmentId = 1,
                    Position = "Dev",
                    DateOfEmployment = new DateTime(2023, 1, 1),
                    IsActive = true,
                    
                },
                new EmployeeModel
                {
                    Id = 2,
                    FirstName = "Emp2",
                    LastName = "Y3",
                    DepartmentId = 1,
                    Position = "Dev",
                    DateOfEmployment = new DateTime(2018, 1, 1),
                    IsActive = true,
                    
                },
                new EmployeeModel
                {
                    Id = 3,
                    FirstName = "Emp3",
                    LastName = "Y5",
                    DepartmentId = 1,
                    Position = "Dev",
                    DateOfEmployment = new DateTime(2016, 1, 1),
                    IsActive = false,
                    
                }
            );

            await db.SaveChangesAsync();

            var service = CreateService(db);

            var filter = new EmployeeFilter
            {
                MinYearsOfService = 3
            };

            // Act
            var result = await service.FilterEmployees(filter);
            var list = result.ToList();

            // Assert
            Assert.NotNull(list);
            Assert.Equal(2, list.Count);          

           
            Assert.All(list, e => Assert.True(e.YearsOfService >= 3));

            Assert.DoesNotContain(list, e => e.Id == 1);
        }

        // Test ID: EMP-FLT-004 | Priority: Medium
        [Fact]
        public async Task FilterEmployees_ByMultipleCriteria_ReturnsMatchingEmployees()
        {
            // Arrange
            var db = CreateInMemoryDbContext();

            db.Departments.AddRange(
                new DepartmentModel { Id = 1, Name = "IT", DepartmentSupervisor = "Boss" },
                new DepartmentModel { Id = 2, Name = "HR", DepartmentSupervisor = "Manager" }
            );

            db.Employees.AddRange(
                // Matches all conditions
                new EmployeeModel
                {
                    Id = 1,
                    FirstName = "A",
                    LastName = "IT-Dev-Active",
                    Position = "Developer",
                    DepartmentId = 1,
                    DateOfEmployment = new DateTime(2020, 1, 1),
                    IsActive = true
                },
                
                new EmployeeModel
                {
                    Id = 2,
                    FirstName = "B",
                    LastName = "HR-Dev-Active",
                    Position = "Developer",
                    DepartmentId = 2,
                    DateOfEmployment = new DateTime(2020, 1, 1),
                    IsActive = true
                },
                
                new EmployeeModel
                {
                    Id = 3,
                    FirstName = "C",
                    LastName = "IT-Dev-Inactive",
                    Position = "Developer",
                    DepartmentId = 1,
                    DateOfEmployment = new DateTime(2020, 1, 1),
                    IsActive = false
                },
                
                new EmployeeModel
                {
                    Id = 4,
                    FirstName = "D",
                    LastName = "HR-HR-Active",
                    Position = "HR",
                    DepartmentId = 2,
                    DateOfEmployment = new DateTime(2020, 1, 1),
                    IsActive = true
                }
            );

            await db.SaveChangesAsync();

            var service = CreateService(db);

            
            var filter = new EmployeeFilter
            {
                DepartmentId = 1,
                Position = "Developer",
                IsActive = true
            };

            // Act
            var result = await service.FilterEmployees(filter);
            var list = result.ToList();

            // Assert
            Assert.Single(list);        
            var emp = list.First();

            Assert.Equal(1, emp.Id);
            Assert.Equal("Developer", emp.Position);
            Assert.Equal(1, emp.DepartmentId);
            Assert.True(emp.IsActive);


            Assert.DoesNotContain(list, e => e.Id == 2);
            Assert.DoesNotContain(list, e => e.Id == 3);
            Assert.DoesNotContain(list, e => e.Id == 4);
        }

    }
}
