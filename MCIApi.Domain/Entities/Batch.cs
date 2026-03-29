using System;
using System.Collections.Generic;

namespace MCIApi.Domain.Entities
{
    public class Batch
    {
        public int Id { get; set; }
        public DateTime ReceiveDate { get; set; }
        public int ProviderId { get; set; }
        public Provider? Provider { get; set; }
        public int? ReceivingWayId { get; set; }
        public ReceivingWay? ReceivingWay { get; set; }
        public int? ReasonId { get; set; }
        public Reason? Reason { get; set; }
        public int? BatchStatusId { get; set; }
        public BatchStatus? BatchStatus { get; set; }
        public int BatchDueDays { get; set; }
        public DateTime BatchDueDate { get; set; }
        public bool UploadOnPortal { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool Reviewed { get; set; }
        public bool IsDeleted { get; set; }
        public int ReceivedClaimsCount { get; set; }
        public decimal ReceivedTotalAmount { get; set; }
        public ICollection<Claim> Claims { get; set; } = new List<Claim>();
    }
}

