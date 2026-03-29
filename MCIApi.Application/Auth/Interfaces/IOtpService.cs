namespace MCIApi.Application.Auth.Interfaces
{
    public interface IOtpService
    {
        string GenerateOtp(int length);
        Task SaveOtpAsync(string otp, string phoneNumber);
        Task<bool> ValidateOtpAsync(string otp, string phoneNumber);
        Task<bool> IsOtpVerifiedAsync(string phoneNumber);
    }
}


