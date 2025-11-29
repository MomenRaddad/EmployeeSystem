using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace EmployeeSystem.Tests.EmployeeServiceTests
{
    public class GetAllTests:TestBase
    {

        // Test ID: EMP-GET-003 | Priority: Medium

        [Fact]
        public async Task GetAll_WhenEmployeesExist_ReturnsAllEmployees()
        {
            //Arrange
            var db = CreateInMemoryDbContext();
            //Seed database with at least 3 employees
            db.Employees.AddRange(new List<EmployeeSystem.Models.EmployeeModel>
            {
                new EmployeeSystem.Models.EmployeeModel
                {
                    Id = 1,
                    FirstName = "Alice",
                    LastName = "Smith",
                    DepartmentId = 1,
                    IsActive = true,
                    Position = "Developer",
                    DateOfEmployment = new System.DateTime(2021, 5, 1),
                },
                new EmployeeSystem.Models.EmployeeModel
                {
                    Id = 2,
                    FirstName = "Bob",
                    LastName = "Johnson",
                    DepartmentId = 2,
                    IsActive = true,
                    Position = "Designer",
                    DateOfEmployment = new System.DateTime(2020, 3, 15),
                },
                new EmployeeSystem.Models.EmployeeModel
                {
                    Id = 3,
                    FirstName = "Charlie",
                    LastName = "Brown",
                    DepartmentId = 1,
                    IsActive = false,
                    Position = "Manager",
                    DateOfEmployment = new System.DateTime(2019, 7, 30),
                }
            });
            await db.SaveChangesAsync();
            
            //Act
            var result = await CreateService(db).GetAll();
        
        //Assert
            Assert.NotNull(result);
            var employeeList = Assert.IsType<List<EmployeeSystem.Models.EmployeeModel>>(result);
            Assert.Equal(3, employeeList.Count);

        }
   
    
    }
}
