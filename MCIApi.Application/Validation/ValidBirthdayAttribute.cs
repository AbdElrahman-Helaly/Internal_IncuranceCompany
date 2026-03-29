using System;
using System.ComponentModel.DataAnnotations;

namespace MCIApi.Application.Validation
{
    public sealed class ValidBirthdayAttribute : ValidationAttribute
    {
        private const int MaxAgeYears = 150;
        private const int MinAgeYears = 0;

        public ValidBirthdayAttribute()
        {
            ErrorMessage = "Birthday must be a valid date, not in the future, and not more than {0} years ago.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // If value is null, it's valid (optional field)
            if (value == null)
                return ValidationResult.Success;

            DateOnly? dateValue = null;

            // Handle DateOnly type
            if (value is DateOnly dateOnly)
            {
                dateValue = dateOnly;
            }
            // Handle DateTime type
            else if (value is DateTime dateTime)
            {
                dateValue = DateOnly.FromDateTime(dateTime);
            }
            // Handle string type (if needed)
            else if (value is string dateString && DateOnly.TryParse(dateString, out var parsedDate))
            {
                dateValue = parsedDate;
            }
            else
            {
                return new ValidationResult("Invalid date format.");
            }

            if (!dateValue.HasValue)
                return new ValidationResult("Invalid date format.");

            var today = DateOnly.FromDateTime(DateTime.Today);
            var date = dateValue.Value;

            // Check if date is in the future
            if (date > today)
            {
                return new ValidationResult("Birthday cannot be in the future.");
            }

            // Check if date is too old (more than MaxAgeYears years ago)
            var minDate = today.AddYears(-MaxAgeYears);
            if (date < minDate)
            {
                return new ValidationResult($"Birthday cannot be more than {MaxAgeYears} years ago.");
            }

            // Check if date is too recent (negative age - less than MinAgeYears)
            var maxDate = today.AddYears(-MinAgeYears);
            if (date > maxDate)
            {
                return new ValidationResult("Birthday must be in the past.");
            }

            return ValidationResult.Success;
        }
    }
}

