using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EmployeeSystem.Models
{
    public class EmployeeModel
    {
        [Key]
        [SwaggerSchema(ReadOnly = true)]
        public int Id { get; set; }
        [SwaggerSchema(ReadOnly = true)]

        public Guid PublicId { get; set; } = Guid.NewGuid();

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
        [SwaggerSchema(ReadOnly = true)]

        [NotMapped]
        public int YearsOfService
        {
            get
            {
                DateTime endDate = EndOfServiceDate ?? DateTime.Now;

                int years = endDate.Year - DateOfEmployment.Year;

                if (endDate.Month < DateOfEmployment.Month ||
                   (endDate.Month == DateOfEmployment.Month && endDate.Day < DateOfEmployment.Day))
                {
                    years--;
                }

                return years < 0 ? 0 : years;
            }
        }

        [Required]
        [StringLength(100)]
        public string Position { get; set; }

        [Required]
        public int DepartmentId { get; set; }


        public bool IsActive { get; set; }
        [JsonIgnore]
        public DepartmentModel? Department { get; set; }

    }
}
