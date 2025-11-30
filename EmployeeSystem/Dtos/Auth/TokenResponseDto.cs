namespace EmployeeSystem.Dtos.Auth
{
    public class TokenResponseDto
    {
        public string Token { get; set; }
        public string Role { get; set; } 
        public DateTime Expiration { get; set; }
    }
}
