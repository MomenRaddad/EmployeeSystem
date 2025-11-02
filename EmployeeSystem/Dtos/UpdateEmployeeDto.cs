namespace EmployeeSystem.Dtos
{
    public class UpdateEmployeeDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? DepartmentId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateOfEmployment { get; set; }
        public DateTime? EndOfServiceDate { get; set; }
        public string? Position { get; set; }
        public bool? IsActive { get; set; }
    }
}
