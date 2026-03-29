using System.Collections.Generic;

namespace MCIApi.Domain.Entities
{
    public class ProviderCategory
    {
        public int Id { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public ICollection<Provider> Providers { get; set; } = new List<Provider>();
    }
}

