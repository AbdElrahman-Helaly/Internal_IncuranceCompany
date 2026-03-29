using System.ComponentModel.DataAnnotations;

namespace MCIApi.Application.GeneralPrograms.DTOs
{
    public class GeneralProgramReadDto
    {
        public int Id { get; set; }
        public int ProgramNameId { get; set; }
        public string? ProgramName { get; set; }
        public decimal? Limit { get; set; }
        public int? RoomTypeId { get; set; }
        public string? Note { get; set; }
        public int PolicyId { get; set; }
    }

    public class GeneralProgramCreateDto
    {
        [Required(ErrorMessage = "ProgramNameId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "ProgramNameId must be greater than 0")]
        public int ProgramNameId { get; set; }

        public decimal? Limit { get; set; }

        public int? RoomTypeId { get; set; }

        [StringLength(1000, ErrorMessage = "Note must not exceed 1000 characters")]
        public string? Note { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "PolicyId is required")]
        public int PolicyId { get; set; }
    }

    public class GeneralProgramUpdateDto : GeneralProgramCreateDto
    {
    }
}

