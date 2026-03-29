namespace MCIApi.Domain.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Mobile { get; set; }
        public required string Email { get; set; }
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
        public int? JobTitleId { get; set; }
        public JobTitle? JobTitle { get; set; }
        public bool IsDeleted { get; set; }
        public string? ImageUrl { get; set; }
        public string? IdentityUserId { get; set; }
        public required string PasswordHash { get; set; }
    }
}


