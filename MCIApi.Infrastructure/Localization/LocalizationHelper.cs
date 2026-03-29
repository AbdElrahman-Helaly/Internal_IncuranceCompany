using System.Globalization;
using MCIApi.Application.Localization;
using MCIApi.Domain.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace MCIApi.Infrastructure.Localization
{
    public class LocalizationHelper : ILocalizationHelper
    {
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LocalizationHelper(
            IStringLocalizer<SharedResource> localizer,
            IHttpContextAccessor httpContextAccessor)
        {
            _localizer = localizer;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetString(string key, string lang)
        {
            var originalCulture = CultureInfo.CurrentCulture;
            var originalUICulture = CultureInfo.CurrentUICulture;

            try
            {
                SetCulture(lang);

                var value = _localizer[key];
                if (!value.ResourceNotFound)
                    return value.Value;

                if (lang?.ToLower() != "en")
                {
                    SetCulture("en");
                    var fallback = _localizer[key];
                    if (!fallback.ResourceNotFound)
                        return fallback.Value;
                }

                return key;
            }
            finally
            {
                CultureInfo.CurrentCulture = originalCulture;
                CultureInfo.CurrentUICulture = originalUICulture;
            }
        }

        private void SetCulture(string lang)
        {
            var culture = lang?.ToLower() == "ar" ? "ar-SA" : "en-US";
            var cultureInfo = new CultureInfo(culture);

            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;

            if (_httpContextAccessor.HttpContext != null)
                _httpContextAccessor.HttpContext.Items["Culture"] = cultureInfo;
        }

        public string UserRegistered(string lang) => GetString("UserRegistered", lang);
        public string InvalidLoginAttempt(string lang) => GetString("InvalidLoginAttempt", lang);
        public string UserNotFound(string lang) => GetString("UserNotFound", lang);
        public string OtpSent(string lang) => GetString("OtpSent", lang);
        public string InvalidOrExpiredOtp(string lang) => GetString("InvalidOrExpiredOtp", lang);
        public string OtpVerified(string lang) => GetString("OtpVerified", lang);
        public string PasswordResetSuccess(string lang) => GetString("PasswordResetSuccess", lang);
        public string PasswordChangeFailed(string lang) => GetString("PasswordChangeFailed", lang);
        public string PasswordChangeSuccess(string lang) => GetString("PasswordChangeSuccess", lang);
        public string OtpAlreadyVerified(string lang) => GetString("OtpAlreadyVerified", lang);
        public string OtpVerificationFailed(string lang) => GetString("OtpVerificationFailed", lang);
        public string AccountAlreadyExists(string lang) => GetString("AccountAlreadyExists", lang);
        public string OtpNotVerified(string lang) => GetString("OtpNotVerified", lang);
        public string LogoutSuccessful(string lang)
        {
            var value = GetString("LogoutSuccessful", lang);
            return value == "LogoutSuccessful" ? "Logout successful." : value;
        }
        public string TokenMissing(string lang)
        {
            var value = GetString("TokenMissing", lang);
            return value == "TokenMissing" ? "Authorization token is missing." : value;
        }
        public string InvalidCredentials(string lang)
        {
            var value = GetString("InvalidCredentials", lang);
            return value == "InvalidCredentials" ? "Invalid credentials." : value;
        }

        public string PasswordTooShort(string lang) => GetString("PasswordTooShort", lang);
        public string PasswordRequiresNonAlphanumeric(string lang) => GetString("PasswordRequiresNonAlphanumeric", lang);
        public string PasswordRequiresLower(string lang) => GetString("PasswordRequiresLower", lang);
        public string PasswordRequiresUpper(string lang) => GetString("PasswordRequiresUpper", lang);
        public string PasswordRequiresDigit(string lang) => GetString("PasswordRequiresDigit", lang);
        public string DuplicateUserName(string lang) => GetString("DuplicateUserName", lang);

        public string DepartmentCreated(string lang) => GetString("DepartmentCreated", lang);
        public string DepartmentUpdated(string lang) => GetString("DepartmentUpdated", lang);
        public string DepartmentDeleted(string lang) => GetString("DepartmentDeleted", lang);
        public string DepartmentNotFound(string lang) => GetString("DepartmentNotFound", lang);
        public string ArabicNameInvalid(string lang) => GetString("ArabicNameInvalid", lang);
        public string EnglishNameInvalid(string lang) => GetString("EnglishNameInvalid", lang);
    }
}


