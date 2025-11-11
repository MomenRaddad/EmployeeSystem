using EmployeeSystem.Dtos;

using EmployeeSystem.Models;

namespace EmployeeSystem.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeModel>> GetAll();

        Task<EmployeeModel?> GetById(int id);

        Task<EmployeeModel> Create(EmployeeModel input);
        Task<bool> Update(int id, EmployeeModel input);
        Task<(bool Success, string? Error, bool NotFound)> UpdatePartial(int id, UpdateEmployeeDto input);

        Task<bool> Delete(int id);



        Task<IEnumerable<EmployeeModel>> FilterEmployees(EmployeeFilter filter);

        Task<bool> Deactivate(int id, DateTime? endDate);
    }
}
