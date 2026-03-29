using System;

namespace MCIApi.Domain.Entities
{
    public class RefundRule
    {
        public int Id { get; set; }
        public int? ReimbursementTypeId { get; set; }
        public ReimbursementType? ReimbursementType { get; set; }
        public ApplyOn ApplyOn { get; set; }
        public int? ProgramId { get; set; }
        public Programs? Program { get; set; }
        public int? PricelistId { get; set; }
        public ProviderPriceList? Pricelist { get; set; }
        public ApplyBy ApplyBy { get; set; }
        public decimal? MaxValue { get; set; }
        public decimal? ReimbursementPercentage { get; set; }
        public string? Notes { get; set; }
        public int PolicyId { get; set; }
        public Policy? Policy { get; set; }
    }
}

