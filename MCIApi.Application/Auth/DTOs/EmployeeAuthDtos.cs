using System.ComponentModel.DataAnnotations;

namespace MCIApi.Application.Auth.DTOs
{
    public class EmployeeLoginDto
    {
        [Phone]
        public required string Mobile { get; set; }

        public required string Password { get; set; }
    }

    public class EmployeeLoginResultDto
    {
        public string Token { get; set; } = string.Empty;
        public int EmployeeId { get; set; }
        public string Mobile { get; set; } = string.Empty;
    }
}

