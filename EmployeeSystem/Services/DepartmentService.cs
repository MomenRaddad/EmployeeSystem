using EmployeeSystem.Data;
using EmployeeSystem.Migrations;
using EmployeeSystem.Models;
using EmployeeSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace EmployeeSystem.Services
{
    public class DepartmentService(AppDbContext db, ILogger<DepartmentService> logger,IMemoryCache cache) : IDepartmentService
    {
        public async Task<IEnumerable<DepartmentModel>> GetAll()
        {
            const string cacheKey = "departments_all";
            logger.LogInformation("[SERVICE] GetAll departments started");
            if (cache.TryGetValue(cacheKey, out IEnumerable<DepartmentModel>? cachedDepts))
            {
                logger.LogInformation("[SERVICE] GetAll departments completed from cache. Count={Count}", cachedDepts.Count());
                return cachedDepts;
            }
            logger.LogInformation("[CACHE] Miss - Key='{Key}'", cacheKey);

            var depts = await db.Departments
                .AsNoTracking()
                .ToListAsync();
            
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
                .SetSlidingExpiration(TimeSpan.FromMinutes(2));

            cache.Set("departments_all", depts, cacheOptions);
            logger.LogInformation("[CACHE] SET - Key='{Key}', Absolute={Abs}min, Sliding={Slide}min", cacheKey, 5, 2);
            logger.LogInformation("[SERVICE] GetAll departments completed. Count={Count}", depts.Count);

            return depts;
        }

        public async Task<DepartmentModel?> GetById(int id)
        {
            var cacheKey = $"Department_Id:{id}";


            logger.LogInformation("[SERVICE] Get department by id {DepartmentId} started", id);
            if(cache.TryGetValue(cacheKey, out DepartmentModel? cachedDept))
            {
                logger.LogInformation("[SERVICE] Get department by id {DepartmentId} completed from cache", id);
                return cachedDept;
            }
 
                logger.LogInformation("[CACHE] Miss - Key='{Key}'", cacheKey);


            var d = await db.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);


            if (d is null)
            {
                logger.LogWarning("[SERVICE] Get department by id {DepartmentId} failed: not found", id);
                return d;

            }
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(3))
                .SetSlidingExpiration(TimeSpan.FromMinutes(1));

            cache.Set(cacheKey, d, cacheOptions);
            logger.LogInformation("[CACHE] SET - Key='{Key}', Absolute={Abs}min, Sliding={Slide}min", cacheKey,3,1);
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
            cache.Remove("departments_all");
            logger.LogInformation("[CACHE] REMOVE - Key='departments_all'");
            logger.LogInformation("[CACHE] REMOVE - Key='Employees:Id:{DepartmentId}'", input.Id);
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
            cache.Remove("departments_all");
            cache.Remove($"Department_Id:{id});
            logger.LogInformation("[CACHE] REMOVE - Key='departments_all'");
            logger.LogInformation("[CACHE] REMOVE - Key='Employees:Id:{DepartmentId}'", id);
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

            cache.Remove("departments_all");
            cache.Remove($"Department_Id:{id}");

            logger.LogInformation("[CACHE] REMOVE - Key='departments_all'");

            logger.LogInformation("[CACHE] REMOVE - Key='Employees:Id:{DepartmentId}'", id);
            logger.LogInformation("[SERVICE] Department {DepartmentId} deleted", id);
            return true;
        }

        public async Task<IEnumerable<EmployeeModel>> GetEmployees(int departmentId)
        {


            logger.LogInformation(
                "[SERVICE] Get employees for DepartmentId={DepartmentId} started",
                departmentId);

            var cacheKey = $"DeptEmployees:Id:{departmentId}";
            if (cache.TryGetValue(cacheKey, out IEnumerable<EmployeeModel>? cachedEmps))
            {
                logger.LogInformation(
                "[SERVICE] Get employees for DepartmentId={DepartmentId} completed from cache. Count={Count}",
                departmentId,
                cachedEmps.Count());

                return cachedEmps;
            }
          logger.LogInformation("[CACHE] Miss - Key='DeptEmployees:Id:{DepartmentId}'", departmentId);

            var employees = await db.Employees
                .AsNoTracking()
                .Where(e => e.DepartmentId == departmentId)
                .ToListAsync();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(4))
                .SetSlidingExpiration(TimeSpan.FromMinutes(2))
                .SetPriority(CacheItemPriority.Low);

            cache.Set(cacheKey, employees, cacheOptions);
            logger.LogInformation("[CACHE] SET - Key='{Key}', Absolute={Abs}min, Sliding={Slide}min", cacheKey, 4, 2);
            logger.LogInformation("[CACHE] SET - Key='DeptEmployees:Id:{DepartmentId}'", departmentId);
            logger.LogInformation(
                "[SERVICE] Get employees for DepartmentId={DepartmentId} completed. Count={Count}",
                departmentId,
                employees.Count);

            return employees;
        }
    }
}
