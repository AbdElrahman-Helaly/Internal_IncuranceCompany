using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MCIApi.Application.CPTs.DTOs
{
    public class CPTFilterDto
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string? SearchColumn { get; set; }
        public string? Search { get; set; }
    }

    public class CPTListItemDto
    {
        public int Id { get; set; }
        public string ArName { get; set; } = string.Empty;
        public string EnName { get; set; } = string.Empty;
        public string CPTCode { get; set; } = string.Empty;
        public string? CPTDescription { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? ICHI { get; set; }
        public int CountInPriceList { get; set; }
    }

    public class CPTPagedResultDto
    {
        public int Total { get; set; }
        public int CurrentPage { get; set; }
        public int Limit { get; set; }
        public int TotalPages { get; set; }
        public IReadOnlyCollection<CPTListItemDto> Data { get; set; } = Array.Empty<CPTListItemDto>();
    }

    public class CPTCreateDto
    {
        [Required]
        [MaxLength(200)]
        public string ArName { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string EnName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string CPTCode { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? CPTDescription { get; set; }

        [Required]
        public int StatusId { get; set; }

        public int? CategoryId { get; set; }

        [MaxLength(50)]
        public string? ICHI { get; set; }
    }
}

