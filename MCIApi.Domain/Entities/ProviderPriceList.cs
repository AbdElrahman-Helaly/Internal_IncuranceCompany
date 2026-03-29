using System;
using System.Collections.Generic;

namespace MCIApi.Domain.Entities
{
    public class ProviderPriceList
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public Provider? Provider { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        // Extended fields for full provider structure
        public string? Name { get; set; }
        public decimal NormalDiscount { get; set; }
        public decimal AdditionalDiscount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string? Notes { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<ProviderPriceListService> Services { get; set; } = new List<ProviderPriceListService>();
    }
}

