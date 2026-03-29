namespace MCIApi.Domain.Entities
{
    public class ProviderAccountant
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public Provider? Provider { get; set; }
        public string CommercialRegisterNum { get; set; } = string.Empty;
        public decimal AdminFeesPercentage { get; set; }
        public decimal Taxes { get; set; }
        public bool IsDeleted { get; set; }
    }
}

