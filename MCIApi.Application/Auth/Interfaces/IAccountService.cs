using MCIApi.Application.Auth.DTOs;

namespace MCIApi.Application.Auth.Interfaces
{
    public interface IAccountService
    {
        Task<RegisterResultDto> RegisterAsync(RegisterRequestDto model, string lang, CancellationToken cancellationToken = default);

        Task<LoginResultDto> LoginAsync(LoginRequestDto model, string lang, CancellationToken cancellationToken = default);
        Task<EmployeeLoginResultDto> LoginEmployeeAsync(EmployeeLoginDto model, string lang, CancellationToken cancellationToken = default);

        Task<bool> SendOtpAsync(string phoneNumber, string lang, CancellationToken cancellationToken = default);

        Task<bool> VerifyOtpAsync(VerifyOtpDto model, string lang, CancellationToken cancellationToken = default);

        Task<BasicResultDto> ResetPasswordAsync(ResetPasswordOnlyDto model, string lang, CancellationToken cancellationToken = default);

        Task<BasicResultDto> LogoutAsync(string token, string lang, CancellationToken cancellationToken = default);
    }
}


