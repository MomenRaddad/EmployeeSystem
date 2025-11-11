using EmployeeSystem.Data;
using EmployeeSystem.Dtos;
using EmployeeSystem.Models;
using EmployeeSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSystem.Services
{
    public class EmployeeService(AppDbContext db) : IEmployeeService
    {
     

        private static int CalcYears(DateTime start, DateTime? end = null)
        {
            var to = end ?? DateTime.Today;
            var years = to.Year - start.Year;
            if (to.Month < start.Month || (to.Month == start.Month && to.Day < start.Day)) years--;
            return years < 0 ? 0 : years;
        }

        public async Task<IEnumerable<EmployeeModel>> GetAll()
        {

           return await db.Employees.AsNoTracking().ToListAsync();

        }



        public async Task<EmployeeModel?> GetById(int id)
            => await db.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);


        public async Task<EmployeeModel> Create(EmployeeModel input)
        {
            var depExists = await db.Departments.AnyAsync(d => d.Id == input.DepartmentId);
            if (!depExists) throw new InvalidOperationException("Department not found.");

            input.YearsOfService = CalcYears(input.DateOfEmployment, input.EndOfServiceDate);
            input.IsActive = !input.EndOfServiceDate.HasValue;

            await db.Employees.AddAsync(input);
            await db.SaveChangesAsync();
            return input;
        }

        
        public async Task<bool> Update(int id, EmployeeModel input)
        {
            var e = await db.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if (e is null) return false;

            var depExists = await db.Departments.AnyAsync(d => d.Id == input.DepartmentId);
            if (!depExists) throw new InvalidOperationException("Department not found.");

        

            e.FirstName = input.FirstName;
            e.LastName = input.LastName;
            e.DateOfBirth = input.DateOfBirth;
            e.DateOfEmployment = input.DateOfEmployment;
            e.EndOfServiceDate = input.EndOfServiceDate;
            e.YearsOfService = CalcYears(input.DateOfEmployment, input.EndOfServiceDate);
            e.Position = input.Position;
            e.DepartmentId = input.DepartmentId;
            e.IsActive = input.IsActive;

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string? Error, bool NotFound)> UpdatePartial(int id, UpdateEmployeeDto input)
        {
            var e = await db.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if (e is null) return (false, null, true);

            if (!string.IsNullOrWhiteSpace(input.FirstName)) e.FirstName = input.FirstName;
            if (!string.IsNullOrWhiteSpace(input.LastName)) e.LastName = input.LastName;
            if (input.DateOfBirth.HasValue) e.DateOfBirth = input.DateOfBirth.Value;
            if (input.DateOfEmployment.HasValue) e.DateOfEmployment = input.DateOfEmployment.Value;
            if (input.EndOfServiceDate.HasValue) e.EndOfServiceDate = input.EndOfServiceDate.Value;
            if (!string.IsNullOrWhiteSpace(input.Position)) e.Position = input.Position;

            if (input.DepartmentId.HasValue)
            {
                var depExists = await db.Departments.AnyAsync(d => d.Id == input.DepartmentId.Value);
                if (!depExists) return (false, "Department not found.", false);
                e.DepartmentId = input.DepartmentId.Value;
            }

            if (input.IsActive.HasValue) e.IsActive = input.IsActive.Value;

            e.YearsOfService = CalcYears(e.DateOfEmployment, e.EndOfServiceDate);

            await db.SaveChangesAsync();
            return (true, null, false);
        }

        public async Task<bool> Delete(int id)
        {
            var e = await db.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if (e is null) return false;

            db.Employees.Remove(e);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Deactivate(int id, DateTime endDate)
        {
            var e = await db.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if (e is null) return false;

            e.IsActive = false;
            e.EndOfServiceDate = endDate;
            e.YearsOfService = CalcYears(e.DateOfEmployment, endDate);

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<EmployeeModel>> FilterEmployees(EmployeeFilter filter)
        {
            var query = db.Employees.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Position))
            {
                query = query.Where(e => e.Position.ToLower() == filter.Position.ToLower());
            }
            if(filter.EmployeeId.HasValue)
            {
                query = query.Where(e => e.Id == filter.EmployeeId.Value);
            }
            if (filter.DepartmentId.HasValue)
            {
                query = query.Where(e => e.DepartmentId == filter.DepartmentId.Value);
            }
            if (filter.MinYearsOfService.HasValue)
            {
                query = query.Where(e => e.YearsOfService >= filter.MinYearsOfService.Value);
            }
            if (filter.IsActive.HasValue)
            {
                query = query.Where(e => e.IsActive == filter.IsActive.Value);
            }
            return await query.ToListAsync();
        }
    }
}
