namespace MCIApi.Application.Approvals.DTOs
{
    public class PoolListDto
    {
        public int Id { get; set; }
        public int PoolTypeId { get; set; }
        public string? PoolTypeNameEn { get; set; }
        public string? PoolTypeNameAr { get; set; }
        public string? PoolTypeName { get; set; } // localized name (kept for compatibility)
        public int ApplyOn { get; set; } // ApplyOn enum value
        public int? ApplyTo { get; set; }
        public decimal? PoolLimit { get; set; }
        public int? MemberCount { get; set; }
        public decimal? PercentageOfMember { get; set; }
        public bool IsLimitExceed { get; set; }
        public string? Notes { get; set; }
        public int PolicyId { get; set; }
    }
    public class PoolList
    {
        public int Id { get; set; }
        public int PoolTypeId { get; set; }
        public string? PoolTypeNameEn { get; set; }
        public string? PoolTypeNameAr { get; set; }
       
    }

}

