using EmployeeSystem.Data;
using EmployeeSystem.Dtos;
using EmployeeSystem.Models;
using EmployeeSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EmployeeSystem.Services
{
    public class EmployeeService(AppDbContext db, ILogger<EmployeeService> logger, IMemoryCache cache) : IEmployeeService
    {
        public async Task<IEnumerable<EmployeeModel>> GetAll()
        {
            logger.LogInformation("[SERVICE] GetAll employees started");

            var cacheKey = $"Employees:All";

            if (cache.TryGetValue(cacheKey, out IEnumerable<EmployeeModel>? cachedEmployees))
            {
                logger.LogInformation("[SERVICE] GetAll employees retrieved from cache. Count={Count}", cachedEmployees.Count());
                return cachedEmployees;
            }
            logger.LogInformation("[CACHE] Miss - Key='Employees:All'");


            var cacheOptions = new MemoryCacheEntryOptions().
                SetAbsoluteExpiration(TimeSpan.FromMinutes(10)).
                SetSlidingExpiration(TimeSpan.FromMinutes(3)).
                SetPriority(CacheItemPriority.High);

            var employees = await db.Employees
                .AsNoTracking()
                .ToListAsync();

            cache.Set(cacheKey, employees, cacheOptions);

            logger.LogInformation("[CACHE] SET - Key='{Key}', Absolute={Abs}min, Sliding={Slide}min", cacheKey, 15, 3);

            logger.LogInformation("[SERVICE] GetAll employees completed. Count={Count}", employees.Count);

            return employees;
        }

        public async Task<EmployeeModel?> GetById(int id)
        {
            logger.LogInformation("[SERVICE] GetById called for EmployeeId={EmployeeId}", id);
            var cacheKey = $"Employees:Id:{id}";

            if (cache.TryGetValue(cacheKey, out EmployeeModel? cachedEmployee))
            {
                logger.LogInformation("[SERVICE] GetById employee {EmployeeId} retrieved from cache", id);
                return cachedEmployee;
            }

            logger.LogInformation("[CACHE] Miss - Key='{Key}'", cacheKey);



            var e = await db.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);


            if (e is null)
            {
                logger.LogWarning("[SERVICE] GetById: employee {EmployeeId} not found", id);
                return e;
            }
            var cacheOptions = new MemoryCacheEntryOptions().
                 SetAbsoluteExpiration(TimeSpan.FromMinutes(7))
                .SetSlidingExpiration(TimeSpan.FromMinutes(3));

            cache.Set(cacheKey, e, cacheOptions);
            logger.LogInformation("[CACHE] SET - Key='{Key}', Absolute={Abs}min, Sliding={Slide}min", cacheKey, 10, 1);

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
            cache.Remove("Employees:All");
            logger.LogInformation("[CACHE] REMOVE - Key='Employees:All' due to new employee creation");
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
            cache.Remove("Employees:All");
            cache.Remove($"Employees:Id:{id}");

            logger.LogInformation("[CACHE] REMOVE - Key='Employees:All' due to employee {EmployeeId} update", id);
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
            cache.Remove("Employees:All");
            cache.Remove($"Employees:Id:{id}");
            logger.LogInformation("[CACHE] REMOVE - Key='Employees:All' due to employee {EmployeeId} patch", id);
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
            cache.Remove("Employees:All");
            logger.LogInformation("[CACHE] REMOVE - Key='Employees:All' due to employee {EmployeeId} deletion", id);
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
            cache.Remove("Employees:All");
            cache.Remove($"Employees:Id:{id}");
            logger.LogInformation("[CACHE] REMOVE - Key='Employees:All' due to employee {EmployeeId} deactivation", id);
            logger.LogInformation("[CACHE] REMOVE - Key='Employees:Id:{EmployeeId}' due to deactivation", id);

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

            if (filter.IsActive.HasValue)
            {
                query = query.Where(e => e.IsActive == filter.IsActive.Value);
            }

            var data = await query.ToListAsync();

            if (filter.MinYearsOfService.HasValue)
            {
                data = data.Where(e => e.YearsOfService >= filter.MinYearsOfService.Value).ToList();
            }
            logger.LogInformation("[SERVICE] FilterEmployees completed. Count={Count}", data.Count);
            return data;
        }

    }
}
