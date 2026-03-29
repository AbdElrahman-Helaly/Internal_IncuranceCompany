using System;

namespace MCIApi.Domain.Entities
{
    public class MemberPolicyInfo
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public MemberInfo? Member { get; set; }
        public int PolicyId { get; set; }
        public Policy? Policy { get; set; }
        public int ProgramId { get; set; }
        public GeneralProgram? Program { get; set; }
        public int? BranchId { get; set; }
        public Branch? Branch { get; set; }
        public int RelationId { get; set; }
        public Relation? Relation { get; set; }
        public int? HeadOfFamilyId { get; set; }
        public bool IsVip { get; set; }
        public string? JobTitle { get; set; }
        public string? Notes { get; set; }
        public string? Address { get; set; }
        public bool CardPrinted { get; set; }
        public DateTime AddDate { get; set; } = DateTime.Now;
        public string? CodeAtCompany { get; set; }
        public string? ImageUrl { get; set; }
        public string? AppPassword { get; set; }
        public string? FirebaseToken { get; set; }
        public string? Email { get; set; }
        public bool IsHr { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? DeleteDate { get; set; }
        public bool IsExpired { get; set; }
        public int TotalApprovals { get; set; }
        public int TotalClaims { get; set; }
        public int TotalRefund { get; set; }
    }
}


