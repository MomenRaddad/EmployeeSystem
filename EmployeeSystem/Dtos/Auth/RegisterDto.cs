using System.ComponentModel.DataAnnotations;

namespace EmployeeSystem.Dtos.Auth
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        
    }
}
