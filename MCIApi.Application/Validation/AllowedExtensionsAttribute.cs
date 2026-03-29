using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace MCIApi.Application.Validation
{
    public sealed class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly HashSet<string> _extensions;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions.Select(e => e.ToLowerInvariant()).ToHashSet();
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
                if (extension == null || !_extensions.Contains(extension))
                {
                    return new ValidationResult(ErrorMessage ?? "This file extension is not allowed.");
                }
            }

            return ValidationResult.Success;
        }
    }
}


