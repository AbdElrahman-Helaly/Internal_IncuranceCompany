namespace MCIApi.Domain.Entities
{
    public class ProviderExtraFinanceInfo
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public Provider? Provider { get; set; }
        public int ProviderTypeId { get; set; }
        public string? TaxNum { get; set; }
        public string? FullAddress { get; set; }
        public int GovernmentId { get; set; }
        public Government? Government { get; set; }
        public int CityId { get; set; }
        public City? City { get; set; }
        public string? Area { get; set; }
        public string? StreetNum { get; set; }
        public string? BuildingNum { get; set; }
        public string? OfficeNum { get; set; }
        public string? Landmark { get; set; }
        public string? PostalCode { get; set; }
        public bool IsDeleted { get; set; }
    }
}

