namespace MCIApi.Domain.Entities
{
    public class ProviderPriceListService
    {
        public int Id { get; set; }
        public int ProviderPriceListId { get; set; }
        public ProviderPriceList? ProviderPriceList { get; set; }
        public int CptId { get; set; }
        public CPT? CPT { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public bool IsPriceApproval { get; set; }
    }
}

