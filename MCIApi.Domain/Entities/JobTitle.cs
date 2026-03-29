using System.Collections.Generic;

namespace MCIApi.Domain.Entities
{
    public class JobTitle
    {
        public int Id { get; set; }
        public required string NameAr { get; set; }
        public required string NameEn { get; set; }
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}


