using System;
using System.Collections.Generic;

namespace MCIApi.Domain.Entities
{
    public class Client
    {
        public int Id { get; set; }
        public string? ArabicName { get; set; }
        public string? EnglishName { get; set; }
        public string? ShortName { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public int TypeId { get; set; }
        public ClientType? Type { get; set; }

        public DateTime? PolicyStart { get; set; }
        public DateTime? PolicyExpire { get; set; }
        public int? ActivePolicyId { get; set; }
        public Policy? ActivePolicy { get; set; }
        public string? ImageUrl { get; set; }
        public int RefundDueDays { get; set; }

        public int StatusId { get; set; }
        public Status? Status { get; set; }

        public int? LevelId { get; set; }
        public MemberLevel? Level { get; set; }

        public bool IsDeleted { get; set; }
        public ICollection<Branch> Branches { get; set; } = new List<Branch>();
        public ICollection<ClientContact> Contacts { get; set; } = new List<ClientContact>();
        public ICollection<ClientContractInfo> Contracts { get; set; } = new List<ClientContractInfo>();
        public ICollection<ClientMemberSnapshot> Members { get; set; } = new List<ClientMemberSnapshot>();
    }
}


