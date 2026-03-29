using System;
using System.Collections.Generic;

namespace MCIApi.Domain.Entities
{
    public enum PolicyStatus
    {
        Active,
        Pending,
        Expired
    }

    public class Policy
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public Client? Client { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public PolicyStatus Status { get; set; }
        public int PolicyTypeId { get; set; }
        public PolicyType? PolicyType { get; set; }
        public int CarrierCompanyId { get; set; }
        public CarrierCompany? CarrierCompany { get; set; }
        public bool IsCalculateUpperPeday { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? WarningOnPercentage { get; set; }
        public ICollection<GeneralProgram> GeneralPrograms { get; set; } = new List<GeneralProgram>();
        public ICollection<Pool> Pools { get; set; } = new List<Pool>();
        public ICollection<RefundRule> RefundRules { get; set; } = new List<RefundRule>();
        public ICollection<PolicyAttachment> Attachments { get; set; } = new List<PolicyAttachment>();
        public ICollection<PolicyPayment> Payments { get; set; } = new List<PolicyPayment>();
    }
}

