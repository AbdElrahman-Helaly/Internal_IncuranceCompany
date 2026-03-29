using System.Collections.Generic;

namespace MCIApi.Domain.Entities
{
    public class Government
    {
        public int Id { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }

        public ICollection<City> Cities { get; set; } = new List<City>();
        public ICollection<ProviderLocation> Locations { get; set; } = new List<ProviderLocation>();
    }
}


