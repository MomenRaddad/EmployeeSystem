using System.Collections.Generic;
using System.Linq;
using EmployeeSystem.Data;
using EmployeeSystem.Models;
using EmployeeSystem.Services.Interfaces;

namespace EmployeeSystem.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly InMemoryStore _db;

        public DepartmentService(InMemoryStore db)
        {
            _db = db;
        }

        public IEnumerable<DepartmentModel> GetAll() => _db.Departments;

        public DepartmentModel? GetById(int id) =>
            _db.Departments.FirstOrDefault(d => d.Id == id);

        public DepartmentModel Create(DepartmentModel input)
        {
            input.Id = _db.NextDepartmentId();
            _db.Departments.Add(input);
            Save();
            return input;
        }

        public bool Update(int id, DepartmentModel input)
        {
            var d = _db.Departments.FirstOrDefault(x => x.Id == id);
            if (d is null) return false;

            d.Name = input.Name;
            d.DepartmentSupervisor = input.DepartmentSupervisor;

            Save();
            return true;
        }

        public bool Delete(int id)
        {
            if (_db.Employees.Any(e => e.DepartmentId == id))
                return false;

            var d = _db.Departments.FirstOrDefault(x => x.Id == id);
            if (d is null) return false;

            _db.Departments.Remove(d);
            Save();
            return true;
        }

        public IEnumerable<EmployeeModel> GetEmployees(int departmentId) =>
            _db.Employees.Where(e => e.DepartmentId == departmentId);

        private void Save() => _db.SaveToDisk();
    }
}
