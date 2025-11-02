using System;
using System.Collections.Generic;
using System.Linq;
using EmployeeSystem.Data;
using EmployeeSystem.Models;
using EmployeeSystem.Services.Interfaces;

namespace EmployeeSystem.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly InMemoryStore _db;

        public EmployeeService(InMemoryStore db)
        {
        private static int CalYearsOfService(DateTime start, DateTime? end = null)
        {
            var to = end ?? DateTime.Today;
            int years = to.Year - start.Year;

            if (to.Month < start.Month || (to.Month == start.Month && to.Day < start.Day))
                years--;

            return years < 0 ? 0 : years;
        }

        public IEnumerable<EmployeeModel> GetAll() => _db.Employees;
        public IEnumerable<EmployeeModel> GetActive() => _db.Employees.Where(e => e.IsActive);
        public IEnumerable<EmployeeModel> GetInactive() => _db.Employees.Where(e => !e.IsActive);

        public EmployeeModel? GetById(int id) => _db.Employees.FirstOrDefault(e => e.Id == id);

        public EmployeeModel Create(EmployeeModel input)
        {
            if (!_db.Departments.Any(d => d.Id == input.DepartmentId))
                throw new InvalidOperationException("Department not found.");

            input.Id = _db.NextEmployeeId();

            
            if (input.EndOfServiceDate.HasValue)
            {
                input.YearsOfService = CalYearsOfService(input.DateOfEmployment, input.EndOfServiceDate);
                input.IsActive = false;
            else
            {
                input.YearsOfService = CalYearsOfService(input.DateOfEmployment, input.EndOfServiceDate);

                input.IsActive = true;

            _db.Employees.Add(input);
            Save();

            return input;
        }

        public bool Update(int id, EmployeeModel input)
        {
            var e = _db.Employees.FirstOrDefault(x => x.Id == id);
            if (e is null) return false;

            if (!_db.Departments.Any(d => d.Id == input.DepartmentId))
                throw new InvalidOperationException("Department not found.");

            e.FirstName = input.FirstName;
            e.LastName = input.LastName;
            e.DateOfBirth = input.DateOfBirth;
            e.DateOfEmployment = input.DateOfEmployment;
            e.EndOfServiceDate = input.EndOfServiceDate;
            e.YearsOfService = CalYearsOfService(input.DateOfEmployment, input.EndOfServiceDate);
            e.Position = input.Position;
            e.DepartmentId = input.DepartmentId;
            e.IsActive = input.IsActive;

            Save();
            return true;
        }
        public (bool Success, string? Error, bool NotFound) UpdatePartial(int id, UpdateEmployeeDto input)
        {
            var e = db.Employees.FirstOrDefault(x => x.Id == id);
            if (e is null)
                return (false, null, true);  

           
            if (!string.IsNullOrWhiteSpace(input.FirstName))
                e.FirstName = input.FirstName;

            
            if (!string.IsNullOrWhiteSpace(input.LastName))
                e.LastName = input.LastName;

            
            if (input.DateOfBirth.HasValue)
                e.DateOfBirth = input.DateOfBirth.Value;


            if (input.DateOfEmployment.HasValue)
            {
                e.DateOfEmployment = input.DateOfEmployment.Value;

            }
            if (input.EndOfServiceDate.HasValue)
            {
                e.EndOfServiceDate = input.EndOfServiceDate.Value; }


 

            
            if (!string.IsNullOrWhiteSpace(input.Position))
                e.Position = input.Position;

            
            if (input.DepartmentId.HasValue)
            {
                bool depExists = db.Departments.Any(d => d.Id == input.DepartmentId.Value);
                if (!depExists)
                    return (false, "Department not found.", false);

                e.DepartmentId = input.DepartmentId.Value;
            }

           
            if (input.IsActive.HasValue)
                e.IsActive = input.IsActive.Value;

           Save();
            return (true, null, false);
        }

        public bool Delete(int id)
        {
            var e = _db.Employees.FirstOrDefault(x => x.Id == id);
            if (e is null) return false;

            _db.Employees.Remove(e);
            Save();
            return true;
        }

        public IEnumerable<EmployeeModel> GetByDepartmentId(int departmentId) =>
            _db.Employees.Where(e => e.DepartmentId == departmentId);

        public IEnumerable<EmployeeModel> GetByPosition(string position) =>
            _db.Employees.Where(e => string.Equals(e.Position, position, StringComparison.OrdinalIgnoreCase));

        public IEnumerable<EmployeeModel> GetWithMinYears(int minYears) =>
            _db.Employees.Where(e => e.YearsOfService >= minYears);

        public bool Deactivate(int id, DateTime endDate)
        {
            var e = _db.Employees.FirstOrDefault(x => x.Id == id);
            if (e is null) return false;

            e.IsActive = false;
            e.EndOfServiceDate = endDate;
            e.YearsOfService = CalYearsOfService(e.DateOfEmployment, endDate);

            Save();
            return true;
        }

      
        private void Save() => _db.SaveToDisk();
    }
}
