using System;

namespace MCIApi.Domain.Entities
{
    public enum ApplyOn
    {
        Member = 1,
        Contract = 2,
        Program = 3
    }

    public enum ApplyBy
    {
        Member = 1,
        Contract = 2,
        Program = 3,
        Pricelist = 4
    }

    public class Pool
    {
        public int Id { get; set; }
        public int PoolTypeId { get; set; }
        public PoolType? PoolType { get; set; }
        public ApplyOn ApplyOn { get; set; }
        public int? ApplyTo { get; set; }
        public decimal? PoolLimit { get; set; }
        public int? MemberCount { get; set; }
        public decimal? PercentageOfMember { get; set; }
        public bool IsLimitExceed { get; set; }
        public string? Notes { get; set; }
        public int PolicyId { get; set; }
        public Policy? Policy { get; set; }
        public ICollection<Approval> Approvals { get; set; } = new List<Approval>();
    }
}

