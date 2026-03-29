namespace MCIApi.Domain.Entities
{
    public class ProviderDiscount
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public Provider? Provider { get; set; }
        public string DiscountType { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }
}

