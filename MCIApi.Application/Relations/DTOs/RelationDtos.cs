using System.ComponentModel.DataAnnotations;

namespace MCIApi.Application.Relations.DTOs
{
    public class RelationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class RelationCreateDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;
    }

    public class RelationUpdateDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;
    }
}

