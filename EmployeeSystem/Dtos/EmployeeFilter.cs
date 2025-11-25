using System.ComponentModel.DataAnnotations;

namespace EmployeeSystem.Dtos
{
    public class EmployeeFilter
    {
        public int? DepartmentId { get; set; }
        public int? EmployeeId { get; set; }
        public bool? IsActive { get; set; }
        public int? MinYearsOfService { get; set; }
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Position must contain only letters.")]
        public string? Position { get; set; }

    }
}
