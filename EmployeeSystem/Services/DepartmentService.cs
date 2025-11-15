using EmployeeSystem.Data;
using EmployeeSystem.Models;
using EmployeeSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EmployeeSystem.Services
{
    public class DepartmentService(AppDbContext db, ILogger<DepartmentService> logger) : IDepartmentService
    {
        public async Task<IEnumerable<DepartmentModel>> GetAll()
        {
            logger.LogInformation("[SERVICE] GetAll departments started");

            var depts = await db.Departments
                .AsNoTracking()
                .ToListAsync();

            logger.LogInformation("[SERVICE] GetAll departments completed. Count={Count}", depts.Count);

            return depts;
        }

        public async Task<DepartmentModel?> GetById(int id)
        {
            logger.LogInformation("[SERVICE] Get department by id {DepartmentId} started", id);

            var d = await db.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            if (d is null)
            {
                logger.LogWarning("[SERVICE] Get department by id {DepartmentId} failed: not found", id);
            }

            return d;
        }

        public async Task<DepartmentModel> Create(DepartmentModel input)
        {
            logger.LogInformation(
                "[SERVICE] Creating department. Name={Name}, Supervisor={Supervisor}",
                input.Name,
                input.DepartmentSupervisor);

            await db.Departments.AddAsync(input);
            await db.SaveChangesAsync();

            logger.LogInformation(
                "[SERVICE] Department created. DepartmentId={DepartmentId}, Name={Name}",
                input.Id,
                input.Name);

            return input;
        }

        public async Task<bool> Update(int id, DepartmentModel input)
        {
            logger.LogInformation("[SERVICE] Update department {DepartmentId} started", id);

            var d = await db.Departments.FirstOrDefaultAsync(x => x.Id == id);
            if (d is null)
            {
                logger.LogWarning("[SERVICE] Update department {DepartmentId} failed: not found", id);
                return false;
            }

            d.Name = input.Name;
            d.DepartmentSupervisor = input.DepartmentSupervisor;

            await db.SaveChangesAsync();

            logger.LogInformation("[SERVICE] Department {DepartmentId} updated successfully", id);
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            logger.LogInformation("[SERVICE] Delete department {DepartmentId} started", id);

            var hasEmployees = await db.Employees.AnyAsync(e => e.DepartmentId == id);
            if (hasEmployees)
            {
                logger.LogWarning(
                    "[SERVICE] Delete department {DepartmentId} failed: department has employees",
                    id);
                return false;
            }

            var d = await db.Departments.FirstOrDefaultAsync(x => x.Id == id);
            if (d is null)
            {
                logger.LogWarning("[SERVICE] Delete department {DepartmentId} failed: not found", id);
                return false;
            }

            db.Departments.Remove(d);
            await db.SaveChangesAsync();

            logger.LogInformation("[SERVICE] Department {DepartmentId} deleted", id);
            return true;
        }

        public async Task<IEnumerable<EmployeeModel>> GetEmployees(int departmentId)
        {
            logger.LogInformation(
                "[SERVICE] Get employees for DepartmentId={DepartmentId} started",
                departmentId);

            var employees = await db.Employees
                .AsNoTracking()
                .Where(e => e.DepartmentId == departmentId)
                .ToListAsync();

            logger.LogInformation(
                "[SERVICE] Get employees for DepartmentId={DepartmentId} completed. Count={Count}",
                departmentId,
                employees.Count);

            return employees;
        }
    }
}
