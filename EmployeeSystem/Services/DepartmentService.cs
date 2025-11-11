using EmployeeSystem.Data;
using EmployeeSystem.Models;
using EmployeeSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSystem.Services
{
    public class DepartmentService(AppDbContext db) : IDepartmentService
    {
        public async Task<IEnumerable<DepartmentModel>> GetAll()
             => await db.Departments.AsNoTracking().ToListAsync();

        public async Task<DepartmentModel?> GetById(int id)
             => await db.Departments.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);

        public async Task<DepartmentModel> Create(DepartmentModel input)
        {
            await db.Departments.AddAsync(input);
            await db.SaveChangesAsync();
            return input;
        }

        public async Task<bool> Update(int id, DepartmentModel input)
        {
            var d = await db.Departments.FirstOrDefaultAsync(x => x.Id == id);
            if (d is null) return false;

            d.Name = input.Name;
            d.DepartmentSupervisor = input.DepartmentSupervisor;

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        
            {
                var hasEmployees = await db.Employees.AnyAsync(e => e.DepartmentId == id);
                if (hasEmployees) return false;

                var d = await db.Departments.FirstOrDefaultAsync(x => x.Id == id);
                if (d is null) return false;

                db.Departments.Remove(d);
                await db.SaveChangesAsync();
                return true;
            }

          public async Task<IEnumerable<EmployeeModel>> GetEmployees(int departmentId)
                 => await db.Employees
                    .AsNoTracking()
                    .Where(e => e.DepartmentId == departmentId)
                    .ToListAsync();
    }
}
