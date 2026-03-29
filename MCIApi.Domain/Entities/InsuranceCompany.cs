using System;

namespace MCIApi.Domain.Entities
{
    public class InsuranceCompany
    {
        public int Id { get; set; }
        public string ArName { get; set; } = string.Empty;
        public string EnName { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

