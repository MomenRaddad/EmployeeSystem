using EmployeeSystem.Models;
using System.Threading.Tasks;
using Xunit;

namespace EmployeeSystem.Tests.EmployeeServiceTests
{
    public class GetByIdTests: TestBase
    {
        // Test ID: EMP-GET-001  | Priority: High

        [Fact]
        public async Task GetById_WhenEmployeeExists_ReturnsEmployee()
        {

            var db = CreateInMemoryDbContext();

            db.Employees.Add(new EmployeeModel
            {
                Id = 1,
                FirstName = "Momen",
                LastName = "Raddad",
                DepartmentId = 1,
                IsActive = true,
                Position = "HR",
                DateOfEmployment = new DateTime(2020, 1, 1),
            });

            await db.SaveChangesAsync();

            var service = CreateService(db);

            var result = await service.GetById(1);

            Assert.NotNull(result);
            Assert.Equal(1, result!.Id);
            Assert.Equal("Momen", result.FirstName);
        }
        // Test ID: EMP-GET-002 | Priority: High
        [Fact]
        public async Task GetById_WhenEmployeeDoesNotExist_ReturnsNull()
        {
            // Arrange
            var db = CreateInMemoryDbContext();
            db.Employees.Add(new EmployeeModel
            {
                Id = 1,
                FirstName = "Ali",
                LastName = "Ahmad",
                DepartmentId = 2,
                IsActive = true,
                Position = "QA",
                DateOfEmployment = new DateTime(2020, 1, 1),
            });
            await db.SaveChangesAsync();

            var service = CreateService(db);
            var result = await service.GetById(9999);

            Assert.Null(result);
        }
    
    }
}
 