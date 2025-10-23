using System.ComponentModel.DataAnnotations;

namespace EmployeeSystem.Models
{
    public class EmployeeModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50)]

        public string LastName { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfEmployment { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndOfServiceDate { get; set; }

        [Range(0, 50)]
        public int YearsOfService { get; set; } 

        [Required]
        [StringLength(100)]
        public string Position { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        public bool IsActive { get; set; }

    }
}
