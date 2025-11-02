using Microsoft.EntityFrameworkCore;

namespace EmployeeSystem.Models
{
    public class AppDbContext: DbContext
    {
        
        public AppDbContext(DbContextOptions<AppDbContext> options)
          : base(options)
        { }
        public DbSet<DepartmentModel> Departments { get; set; } 
        public DbSet<EmployeeModel> Employees { get; set; } 
    }
}
