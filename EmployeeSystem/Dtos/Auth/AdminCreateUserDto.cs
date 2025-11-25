using System.ComponentModel.DataAnnotations;

namespace EmployeeSystem.Dtos.Auth
{
    public class AdminCreateUserDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        public AppRole? Role { get; set; }
    }

}
