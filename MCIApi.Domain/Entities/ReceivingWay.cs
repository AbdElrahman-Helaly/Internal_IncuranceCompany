namespace MCIApi.Domain.Entities
{
    public class ReceivingWay
    {
        public int Id { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }
}

