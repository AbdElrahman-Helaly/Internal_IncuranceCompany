using System;

namespace MCIApi.Domain.Entities
{
    public class ServiceClassDetail
    {
        public int Id { get; set; }
        public int ProgramId { get; set; }
        public GeneralProgram? Program { get; set; }
        public int ServiceClassId { get; set; }
        public ServiceClass? ServiceClass { get; set; }
        public ServiceLimitType ServiceLimitType { get; set; }
        public int? PoolId { get; set; }
        public Pool? Pool { get; set; }
        public decimal? ServiceLimit { get; set; }
        public int? MemberCount { get; set; }
        public decimal? MemberPercentage { get; set; }
        public int? ApplyTo { get; set; } // Apply To # Service
        public decimal? Copayment { get; set; }
        public string? Notes { get; set; }
        public bool OnlyRefund { get; set; }
    }
}

