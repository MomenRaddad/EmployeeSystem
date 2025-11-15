using EmployeeSystem.Data;
using EmployeeSystem.Dtos;
using EmployeeSystem.Models;
using EmployeeSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EmployeeSystem.Services
{
    public class EmployeeService(AppDbContext db, ILogger<EmployeeService> logger) : IEmployeeService
    {
        public async Task<IEnumerable<EmployeeModel>> GetAll()
        {
            logger.LogInformation("[SERVICE] GetAll employees started");

            var employees = await db.Employees
                .AsNoTracking()
                .ToListAsync();

            logger.LogInformation("[SERVICE] GetAll employees completed. Count={Count}", employees.Count);

            return employees;
        }

        public async Task<EmployeeModel?> GetById(int id)
        {
            logger.LogInformation("[SERVICE] GetById called for EmployeeId={EmployeeId}", id);

            var e = await db.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (e is null)
            {
                logger.LogWarning("[SERVICE] GetById: employee {EmployeeId} not found", id);
            }

            return e;
        }

        public async Task<EmployeeModel> Create(EmployeeModel input)
        {
            logger.LogInformation(
                "[SERVICE] Starting employee creation. FirstName={FirstName}, LastName={LastName}, DepartmentId={DepartmentId}",
                input.FirstName,
                input.LastName,
                input.DepartmentId);

            var depExists = await db.Departments.AnyAsync(d => d.Id == input.DepartmentId);
            if (!depExists)
            {
                logger.LogWarning(
                    "[SERVICE] Employee creation failed. DepartmentId {DepartmentId} not found",
                    input.DepartmentId);

                throw new InvalidOperationException("Department not found.");
            }

            input.IsActive = !input.EndOfServiceDate.HasValue;

            await db.Employees.AddAsync(input);
            await db.SaveChangesAsync();

            logger.LogInformation(
                "[SERVICE] Employee created. EmployeeId={EmployeeId}, DepartmentId={DepartmentId}",
                input.Id,
                input.DepartmentId);

            return input;
        }

        public async Task<bool> Update(int id, EmployeeModel input)
        {
            logger.LogInformation("[SERVICE] Update employee {EmployeeId} started", id);

            var e = await db.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if (e is null)
            {
                logger.LogWarning("[SERVICE] Update failed: employee {EmployeeId} not found", id);
                return false;
            }

            var depExists = await db.Departments.AnyAsync(d => d.Id == input.DepartmentId);
            if (!depExists)
            {
                logger.LogWarning(
                    "[SERVICE] Update failed for employee {EmployeeId}: DepartmentId {DepartmentId} not found",
                    id,
                    input.DepartmentId);

                throw new InvalidOperationException("Department not found.");
            }

            e.FirstName = input.FirstName;
            e.LastName = input.LastName;
            e.DateOfBirth = input.DateOfBirth;
            e.DateOfEmployment = input.DateOfEmployment;
            e.EndOfServiceDate = input.EndOfServiceDate;
            e.Position = input.Position;
            e.DepartmentId = input.DepartmentId;
            e.IsActive = input.IsActive;

            await db.SaveChangesAsync();

            logger.LogInformation("[SERVICE] Employee {EmployeeId} updated successfully", id);
            return true;
        }

        public async Task<(bool Success, string? Error, bool NotFound)> UpdatePartial(int id, UpdateEmployeeDto input)
        {
            logger.LogInformation("[SERVICE] Patch employee {EmployeeId} started", id);

            var e = await db.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if (e is null)
            {
                logger.LogWarning("[SERVICE] Patch failed: employee {EmployeeId} not found", id);
                return (false, null, true);
            }

            if (!string.IsNullOrWhiteSpace(input.FirstName)) e.FirstName = input.FirstName;
            if (!string.IsNullOrWhiteSpace(input.LastName)) e.LastName = input.LastName;
            if (input.DateOfBirth.HasValue) e.DateOfBirth = input.DateOfBirth.Value;
            if (input.DateOfEmployment.HasValue) e.DateOfEmployment = input.DateOfEmployment.Value;
            if (input.EndOfServiceDate.HasValue) e.EndOfServiceDate = input.EndOfServiceDate.Value;
            if (!string.IsNullOrWhiteSpace(input.Position)) e.Position = input.Position;

            if (input.DepartmentId.HasValue)
            {
                var depExists = await db.Departments.AnyAsync(d => d.Id == input.DepartmentId.Value);
                if (!depExists)
                {
                    logger.LogWarning(
                        "[SERVICE] Patch failed for employee {EmployeeId}: DepartmentId {DepartmentId} not found",
                        id,
                        input.DepartmentId.Value);

                    return (false, "Department not found.", false);
                }

                e.DepartmentId = input.DepartmentId.Value;
            }

            if (input.IsActive.HasValue) e.IsActive = input.IsActive.Value;

            await db.SaveChangesAsync();

            logger.LogInformation("[SERVICE] Employee {EmployeeId} patched successfully", id);
            return (true, null, false);
        }

        public async Task<bool> Delete(int id)
        {
            logger.LogInformation("[SERVICE] Delete employee {EmployeeId} started", id);

            var e = await db.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if (e is null)
            {
                logger.LogWarning("[SERVICE] Delete failed: employee {EmployeeId} not found", id);
                return false;
            }

            db.Employees.Remove(e);
            await db.SaveChangesAsync();

            logger.LogInformation("[SERVICE] Employee {EmployeeId} deleted", id);
            return true;
        }

        public async Task<bool> Deactivate(int id, DateTime? endDate)
        {
            logger.LogInformation(
                "[SERVICE] Deactivate employee {EmployeeId} started (EndDate={EndDate})",
                id,
                endDate);

            var e = await db.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if (e is null)
            {
                logger.LogWarning("[SERVICE] Deactivate failed: employee {EmployeeId} not found", id);
                return false;
            }

            e.IsActive = false;
            e.EndOfServiceDate = endDate ?? DateTime.Now;

            await db.SaveChangesAsync();

            logger.LogInformation(
                "[SERVICE] Employee {EmployeeId} deactivated (EndDate={EndDate})",
                id,
                e.EndOfServiceDate);

            return true;
        }

        public async Task<IEnumerable<EmployeeModel>> FilterEmployees(EmployeeFilter filter)
        {
            logger.LogInformation("[SERVICE] FilterEmployees started with {@Filter}", filter);

            var query = db.Employees.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Position))
            {
                query = query.Where(e => e.Position.ToLower() == filter.Position.ToLower());
            }
            if (filter.EmployeeId.HasValue)
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

            var data = await query.ToListAsync();

            logger.LogInformation("[SERVICE] FilterEmployees completed. Count={Count}", data.Count);
            return data;
        }
    }
}
