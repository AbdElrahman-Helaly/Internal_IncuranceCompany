using System;

namespace MCIApi.Domain.Entities
{
    public class ClientMemberSnapshot
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Client? Client { get; set; }
        public int? BranchId { get; set; }
        public Branch? Branch { get; set; }
        public string? BranchName { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime? Birthday { get; set; }
        public int? Age { get; set; }
        public string? ClientName { get; set; }
        public string? ProgramName { get; set; }
        public int? ProgramId { get; set; }
        public Programs? Program { get; set; }
        public string StatusName { get; set; } = "Active";
        public int StatusId { get; set; }
        public Status? Status { get; set; }
        public int LevelId { get; set; }
        public MemberLevel? Level { get; set; }
        public string? LevelName { get; set; }
        public int VipStatusId { get; set; }
        public VipStatus? VipStatus { get; set; }
        public string? VipStatusName { get; set; }
        public string? Mobile { get; set; }
        public bool? IsMale { get; set; }
        public string? JobTitle { get; set; }
        public string? NationalId { get; set; }
        public string? CompanyCode { get; set; }
        public string? HofCode { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

