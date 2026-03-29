using System.Net.Http;
using System.Text;
using MCIApi.Application.Sms;

namespace MCIApi.Infrastructure.Sms
{
    public class SmsService : ISmsService
    {
        private readonly HttpClient _client;

        public SmsService(HttpClient client)
        {
            _client = client;
        }

        public async Task<bool> SendOtpSmsAsync(string mobile, string otp)
        {
            string text = $"MCI OTP is {otp}, valid for 3 minutes";
            string url = $"https://mci-backend.mediconsulteg.com/Message/SendSMS?text={text}&mobile={mobile}&isKhusm=true";

            int maxRetries = 3;

            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    var content = new StringContent("", Encoding.UTF8, "application/json");
                    var response = await _client.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                        return true;
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine($"SMS request timed out. Attempt {attempt + 1} of {maxRetries}");
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"SMS request failed: {ex.Message}");
                }

                await Task.Delay(1000);
            }

            return false;
        }
    }
}


