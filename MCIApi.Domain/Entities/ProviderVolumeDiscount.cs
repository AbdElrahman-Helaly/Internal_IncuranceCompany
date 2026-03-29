namespace MCIApi.Domain.Entities
{
    public class ProviderVolumeDiscount
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public Provider? Provider { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public decimal LocalDiscount { get; set; }
        public decimal ImportDiscount { get; set; }
        public decimal Percentage { get; set; }
        public bool IsDeleted { get; set; }
    }
}

