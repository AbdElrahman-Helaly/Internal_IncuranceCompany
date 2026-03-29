using System.Collections.Generic;

namespace MCIApi.Domain.Entities
{
    public class Branch
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Client? Client { get; set; }
        public string? ArabicName { get; set; }
        public string? EnglishName { get; set; }
        public string Status { get; set; } = "Active";
        public int? BranchStatusId { get; set; }
        public Status? BranchStatus { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<MemberInfo> MemberInfos { get; set; } = new List<MemberInfo>();
    }
}


