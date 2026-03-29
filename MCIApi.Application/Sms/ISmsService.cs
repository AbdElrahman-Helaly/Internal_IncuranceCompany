namespace MCIApi.Application.Sms
{
    public interface ISmsService
    {
        Task<bool> SendOtpSmsAsync(string mobile, string otp);
    }
}


