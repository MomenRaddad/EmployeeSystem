using Microsoft.AspNetCore.Identity;

namespace EmployeeSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? EmployeeId { get; set; }

        public EmployeeModel? Employee { get; set; }
    }
}
