using System;

namespace MCIApi.Domain.Entities
{
    public class ProviderFinancialClearance
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public Provider? Provider { get; set; }
        public DateTime Date { get; set; }
        public bool IsDeleted { get; set; }
    }
}

