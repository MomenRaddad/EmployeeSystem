using EmployeeSystem.Models;

namespace EmployeeSystem.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentModel>> GetAll();
        Task<DepartmentModel?> GetById(int id);

        Task<DepartmentModel> Create(DepartmentModel input);
        Task<bool> Update(int id, DepartmentModel input);
        Task<bool> Delete(int id);

        Task<IEnumerable<EmployeeModel>> GetEmployees(int departmentId);
    }
}
