using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EmployeeSystem.Models
{
    public class DepartmentModel
    {
        [Key]
        [SwaggerSchema(ReadOnly = true)]
        public int Id { get; set; }

        public Guid PublicId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string DepartmentSupervisor { get; set; }
        [JsonIgnore]
        public  ICollection<EmployeeModel> Employees { get; set; } = new List<EmployeeModel>();
    }
}
