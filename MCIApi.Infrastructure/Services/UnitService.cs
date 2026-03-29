using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MCIApi.Application.Common;
using MCIApi.Application.Units.DTOs;
using MCIApi.Application.Units.Interfaces;
using MCIApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MCIApi.Infrastructure.Services
{
    public class UnitService : IUnitService
    {
        private readonly AppDbContext _context;

        public UnitService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<IReadOnlyList<UnitListDto>>> GetAllUnit1Async(string lang, CancellationToken cancellationToken = default)
        {
            var units = await _context.Unit1s
                .Where(u => !u.IsDeleted)
                .OrderBy(u => u.NameEn)
                .Select(u => new UnitListDto
                {
                    Id = u.Id,
                    Name = lang == "ar" ? u.NameAr : u.NameEn
                })
                .ToListAsync(cancellationToken);

            return ServiceResult<IReadOnlyList<UnitListDto>>.Ok(units);
        }

        public async Task<ServiceResult<IReadOnlyList<UnitListDto>>> GetAllUnit2Async(string lang, CancellationToken cancellationToken = default)
        {
            var units = await _context.Unit2s
                .Where(u => !u.IsDeleted)
                .OrderBy(u => u.NameEn)
                .Select(u => new UnitListDto
                {
                    Id = u.Id,
                    Name = lang == "ar" ? u.NameAr : u.NameEn
                })
                .ToListAsync(cancellationToken);

            return ServiceResult<IReadOnlyList<UnitListDto>>.Ok(units);
        }
    }
}

