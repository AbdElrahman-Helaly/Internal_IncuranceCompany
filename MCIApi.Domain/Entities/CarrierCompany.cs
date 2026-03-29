using System;
using System.Collections.Generic;

namespace MCIApi.Domain.Entities
{
    public class CarrierCompany
    {
        public int Id { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public ICollection<Policy> Policies { get; set; } = new List<Policy>();
    }
}

