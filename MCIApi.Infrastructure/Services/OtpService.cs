using MCIApi.Application.Auth.Interfaces;
using MCIApi.Domain.Abstractions;
using MCIApi.Domain.Entities;
using MCIApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace MCIApi.Infrastructure.Services
{
    public class OtpService : IOtpService
    {
        private readonly IMemoryCache _cache;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _context;

        public OtpService(IMemoryCache cache, IUnitOfWork unitOfWork, AppDbContext context)
        {
            _cache = cache;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public string GenerateOtp(int length)
        {
            var random = new Random();
            return new string(Enumerable.Range(0, length)
                .Select(_ => (char)('0' + random.Next(0, 10)))
                .ToArray());
        }

        public async Task SaveOtpAsync(string otp, string phoneNumber)
        {
            _cache.Set($"OTP_{phoneNumber}", otp, TimeSpan.FromMinutes(3));

            var repo = _unitOfWork.Repository<TobOtp>();
            var entity = new TobOtp
            {
                OTP = otp,
                PhoneNumber = phoneNumber,
                RequestTime = DateTime.Now,
                ExpireAt = DateTime.Now.AddMinutes(3),
                IsVerified = false
            };

            await repo.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> ValidateOtpAsync(string otp, string phoneNumber)
        {
            if (_cache.TryGetValue($"OTP_{phoneNumber}", out string? cachedOtp) && cachedOtp == otp)
            {
                _cache.Remove($"OTP_{phoneNumber}");
                // Also mark as verified in database
                await MarkOtpAsVerifiedInDbAsync(otp, phoneNumber);
                return true;
            }

            var now = DateTime.Now;
            var otpRecord = await _context.TobOtps
                .Where(o => o.PhoneNumber == phoneNumber && 
                           o.OTP == otp && 
                           !o.IsVerified &&
                           o.ExpireAt > now)
                .OrderByDescending(o => o.RequestTime)
                .FirstOrDefaultAsync();

            if (otpRecord == null)
                return false;

            otpRecord.IsVerified = true;
            _context.TobOtps.Update(otpRecord);
            await _unitOfWork.SaveChangesAsync();
            
            // Remove from cache if exists
            _cache.Remove($"OTP_{phoneNumber}");

            return true;
        }

        private async Task MarkOtpAsVerifiedInDbAsync(string otp, string phoneNumber)
        {
            var now = DateTime.Now;
            var otpRecord = await _context.TobOtps
                .Where(o => o.PhoneNumber == phoneNumber && 
                           o.OTP == otp && 
                           !o.IsVerified &&
                           o.ExpireAt > now)
                .OrderByDescending(o => o.RequestTime)
                .FirstOrDefaultAsync();

            if (otpRecord != null)
            {
                otpRecord.IsVerified = true;
                _context.TobOtps.Update(otpRecord);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> IsOtpVerifiedAsync(string phoneNumber)
        {
            var now = DateTime.Now;
            var cutoffTime = now.AddMinutes(-10);

            // Use query with filter instead of loading all records
            var hasVerifiedOtp = await _context.TobOtps
                .AnyAsync(o => o.PhoneNumber == phoneNumber &&
                              o.IsVerified &&
                              o.RequestTime >= cutoffTime);

            return hasVerifiedOtp;
        }
    }
}


