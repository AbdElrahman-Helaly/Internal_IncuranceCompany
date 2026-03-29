using System.Collections.Generic;

namespace MCIApi.Domain.Entities
{
    public class City
    {
        public int Id { get; set; }
        public int GovernmentId { get; set; }
        public Government? Government { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }

        public ICollection<ProviderLocation> Locations { get; set; } = new List<ProviderLocation>();
    }
}


