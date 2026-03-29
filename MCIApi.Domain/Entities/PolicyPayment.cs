using System;

namespace MCIApi.Domain.Entities
{
    public class PolicyPayment
    {
        public int Id { get; set; }
        public int PolicyId { get; set; }
        public Policy? Policy { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PaymentValue { get; set; }
        public decimal ActualPaidValue { get; set; }
        public DateTime? ActualPaymentDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}

