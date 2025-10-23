using System;
using System.Collections.Generic;
using EmployeeSystem.Models;

namespace EmployeeSystem.Services.Interfaces
{
    public interface IEmployeeService
    {
        IEnumerable<EmployeeModel> GetAll();
        IEnumerable<EmployeeModel> GetActive();
        IEnumerable<EmployeeModel> GetInactive();
        EmployeeModel? GetById(int id);

        EmployeeModel Create(EmployeeModel input);
        bool Update(int id, EmployeeModel input);
        bool Delete(int id);

        IEnumerable<EmployeeModel> GetByDepartmentId(int departmentId);
        IEnumerable<EmployeeModel> GetByPosition(string position);
        IEnumerable<EmployeeModel> GetWithMinYears(int minYears);

        bool Deactivate(int id, DateTime endDate);
    }
}
