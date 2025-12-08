using EmployeeSystem.Data;
using EmployeeSystem.Models;
using EmployeeSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSystem.Services;

public class DepartmentService(
    AppDbContext db,
    ILogger<DepartmentService> logger,
    IAppCache cache
) : IDepartmentService
{
    private const string AllDepartmentsCacheKey = "departments:all";

    private static string DeptByIdKey(int id) => $"department:{id}";
    private static string DeptEmployeesKey(int id) => $"department:{id}:employees";

    public async Task<IEnumerable<DepartmentModel>> GetAll()
    {
        logger.LogInformation("[SERVICE] GetAll departments started");

        var cached = await cache.GetAsync<List<DepartmentModel>>(AllDepartmentsCacheKey);
        if (cached is { Count: > 0 })
        {
            logger.LogInformation(
                "[SERVICE] GetAll departments served from CACHE. Count={Count}",
                cached.Count);

            return cached;
        }

        var depts = await db.Departments
    .AsNoTracking()
    .ToListAsync();

        logger.LogInformation(
            "[SERVICE] GetAll departments completed from DB. Count={Count}",
            depts.Count);

        await cache.SetAsync(AllDepartmentsCacheKey, depts, TimeSpan.FromMinutes(5));

        return depts;
    }

    public async Task<DepartmentModel?> GetById(int id)
    {
        logger.LogInformation("[SERVICE] Get department by id {DepartmentId} started", id);

        var cacheKey = DeptByIdKey(id);

        var cached = await cache.GetAsync<DepartmentModel>(cacheKey);
        if (cached is not null)
        {
            logger.LogInformation(
                "[SERVICE] Get department by id {DepartmentId} served from CACHE",
                id);

            return cached;
        }

        var d = await db.Departments
    .AsNoTracking()
    .FirstOrDefaultAsync(d => d.Id == id);

        if (d is null)
        {
            logger.LogWarning(
                "[SERVICE] Get department by id {DepartmentId} failed: not found",
                id);
            return null;
        }

        await cache.SetAsync(cacheKey, d, TimeSpan.FromMinutes(10));

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

        await cache.RemoveAsync(AllDepartmentsCacheKey);
        await cache.SetAsync(DeptByIdKey(input.Id), input, TimeSpan.FromMinutes(10));

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
            logger.LogWarning(
                "[SERVICE] Update department {DepartmentId} failed: not found",
                id);
            return false;
        }

        d.Name = input.Name;
        d.DepartmentSupervisor = input.DepartmentSupervisor;

        await db.SaveChangesAsync();

        await cache.RemoveAsync(AllDepartmentsCacheKey);
        await cache.RemoveAsync(DeptByIdKey(id));

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
            logger.LogWarning(
                "[SERVICE] Delete department {DepartmentId} failed: not found",
                id);
            return false;
        }

        db.Departments.Remove(d);
        await db.SaveChangesAsync();

        await cache.RemoveAsync(AllDepartmentsCacheKey);
        await cache.RemoveAsync(DeptByIdKey(id));
        await cache.RemoveAsync(DeptEmployeesKey(id));

        logger.LogInformation("[SERVICE] Department {DepartmentId} deleted", id);
        return true;
    }

    public async Task<IEnumerable<EmployeeModel>> GetEmployees(int departmentId)
    {
        logger.LogInformation(
            "[SERVICE] Get employees for DepartmentId={DepartmentId} started",
            departmentId);

        var cacheKey = DeptEmployeesKey(departmentId);

        var cached = await cache.GetAsync<List<EmployeeModel>>(cacheKey);
        if (cached is { Count: > 0 })
        {
            logger.LogInformation(
                "[SERVICE] Get employees for DepartmentId={DepartmentId} served from CACHE. Count={Count}",
                departmentId,
                cached.Count);

            return cached;
        }

        var employees = await db.Employees
    .AsNoTracking()
    .Where(e => e.DepartmentId == departmentId)
    .ToListAsync();

        logger.LogInformation(
            "[SERVICE] Get employees for DepartmentId={DepartmentId} completed from DB. Count={Count}",
            departmentId,
            employees.Count);

        await cache.SetAsync(cacheKey, employees, TimeSpan.FromMinutes(5));

        return employees;
    }
}
