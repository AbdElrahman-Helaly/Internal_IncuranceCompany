namespace MCIApi.Application.Localization
{
    public interface ILocalizationHelper
    {
        string GetString(string key, string lang);

        // Auth-related messages
        string UserRegistered(string lang);
        string InvalidLoginAttempt(string lang);
        string UserNotFound(string lang);
        string OtpSent(string lang);
        string InvalidOrExpiredOtp(string lang);
        string OtpVerified(string lang);
        string PasswordResetSuccess(string lang);
        string PasswordChangeFailed(string lang);
        string PasswordChangeSuccess(string lang);
        string OtpAlreadyVerified(string lang);
        string OtpVerificationFailed(string lang);
        string AccountAlreadyExists(string lang);
        string OtpNotVerified(string lang);
        string LogoutSuccessful(string lang);
        string TokenMissing(string lang);
        string InvalidCredentials(string lang);

        // Identity error messages
        string PasswordTooShort(string lang);
        string PasswordRequiresNonAlphanumeric(string lang);
        string PasswordRequiresLower(string lang);
        string PasswordRequiresUpper(string lang);
        string PasswordRequiresDigit(string lang);
        string DuplicateUserName(string lang);

        // Department module messages (for future controllers)
        string DepartmentCreated(string lang);
        string DepartmentUpdated(string lang);
        string DepartmentDeleted(string lang);
        string DepartmentNotFound(string lang);
        string ArabicNameInvalid(string lang);
        string EnglishNameInvalid(string lang);
    }
}


