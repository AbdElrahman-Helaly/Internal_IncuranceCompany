namespace MCIApi.Application.TokenBlacklist
{
    public interface ITokenBlacklistService
    {
        Task InvalidateTokenAsync(string token);
        Task<bool> IsTokenBlacklistedAsync(string token);
    }
}


