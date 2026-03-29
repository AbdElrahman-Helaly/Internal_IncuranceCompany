using System;

namespace MCIApi.Domain.Entities
{
    public class PolicyAttachment
    {
        public int Id { get; set; }
        public int PolicyId { get; set; }
        public Policy? Policy { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
    }
}

