using System;
using System.Collections.Generic;

namespace MCIApi.Domain.Entities
{
    public class MemberInfo
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Client? Client { get; set; }
        public int? BranchId { get; set; }
        public Branch? Branch { get; set; }
        public int? ProgramId { get; set; }
        public Programs? Program { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public int StatusId { get; set; } = 1;
        public Status? Status { get; set; }
        public string? JobTitle { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool? IsMale { get; set; }
        public string? NationalId { get; set; }
        public string? Notes { get; set; }
        public string? PrivateNotes { get; set; }
        public string VipStatus { get; set; } = "No";
        public string? MemberImage { get; set; }
        public DateTime? ActivatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public ICollection<MemberPolicyInfo> MemberPolicies { get; set; } = new List<MemberPolicyInfo>();
    }
}

