using System.Collections.Generic;

namespace MCIApi.Domain.Entities
{
    public class GeneralProgram
    {
        public int Id { get; set; }
        public int ProgramNameId { get; set; }
        public Programs? ProgramName { get; set; }
        public decimal? Limit { get; set; }
        public int? RoomTypeId { get; set; }
        public RoomType? RoomType { get; set; }
        public string? Note { get; set; }
        public int PolicyId { get; set; }
        public Policy? Policy { get; set; }
        public ICollection<ServiceClassDetail> ServiceClassDetails { get; set; } = new List<ServiceClassDetail>();
    }
}

