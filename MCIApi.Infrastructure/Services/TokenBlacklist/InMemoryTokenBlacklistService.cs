using System.Collections.Concurrent;
using MCIApi.Application.TokenBlacklist;

namespace MCIApi.Infrastructure.Services.TokenBlacklist
{
    public class InMemoryTokenBlacklistService : ITokenBlacklistService, IDisposable
    {
        private static readonly ConcurrentDictionary<string, DateTime> _blacklistedTokens = new();
        private readonly Timer _cleanupTimer;

        public InMemoryTokenBlacklistService()
        {
            _cleanupTimer = new Timer(_ => CleanupExpiredTokens(), null, TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(10));
        }

        public Task InvalidateTokenAsync(string token)
        {
            if (!string.IsNullOrWhiteSpace(token))
                _blacklistedTokens[token] = DateTime.Now.AddHours(1);

            return Task.CompletedTask;
        }

        public Task<bool> IsTokenBlacklistedAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return Task.FromResult(true);

            if (_blacklistedTokens.TryGetValue(token, out var expiry))
            {
                if (expiry > DateTime.Now)
                    return Task.FromResult(true);

                _blacklistedTokens.TryRemove(token, out _);
            }

            return Task.FromResult(false);
        }

        private void CleanupExpiredTokens()
        {
            var now = DateTime.Now;
            var expired = _blacklistedTokens.Where(t => t.Value <= now).Select(t => t.Key).ToList();
            foreach (var token in expired)
                _blacklistedTokens.TryRemove(token, out _);
        }

        public void Dispose()
        {
            _cleanupTimer?.Dispose();
        }
    }
}


