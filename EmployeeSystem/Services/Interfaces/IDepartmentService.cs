using EmployeeSystem.Models;

namespace EmployeeSystem.Services.Interfaces
{
    public interface IDepartmentService
    {
        IEnumerable<DepartmentModel> GetAll();
        DepartmentModel? GetById(int id);

        DepartmentModel Create(DepartmentModel input);
        bool Update(int id, DepartmentModel input);
        bool Delete(int id); 

        IEnumerable<EmployeeModel> GetEmployees(int departmentId);
    }
}
