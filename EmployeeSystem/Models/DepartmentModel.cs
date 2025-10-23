using System.ComponentModel.DataAnnotations;

namespace EmployeeSystem.Models
{
    public class DepartmentModel
    {
        [Key]

        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string DepartmentSupervisor { get; set; }
    }
}
