using EmployeeSystem.Dtos;
using EmployeeSystem.Models;

namespace EmployeeSystem.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeModel>> GetAll();
        Task<IEnumerable<EmployeeModel>> GetActive();
        Task<IEnumerable<EmployeeModel>> GetInactive();
        Task<EmployeeModel?> GetById(int id);

        Task<EmployeeModel> Create(EmployeeModel input);
        Task<bool> Update(int id, EmployeeModel input);
        Task<(bool Success, string? Error, bool NotFound)> UpdatePartial(int id, UpdateEmployeeDto input);

        Task<bool> Delete(int id);
        Task<IEnumerable<EmployeeModel>> GetByDepartmentId(int departmentId);
        Task<IEnumerable<EmployeeModel>> GetByPosition(string position);
        Task<IEnumerable<EmployeeModel>> GetWithMinYears(int minYears);

        Task<bool> Deactivate(int id, DateTime endDate);
    }
}
