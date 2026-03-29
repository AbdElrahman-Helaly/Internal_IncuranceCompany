using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MCIApi.Application.Common;
using MCIApi.Application.ProviderLocations.DTOs;
using MCIApi.Application.ProviderLocations.Interfaces;
using MCIApi.Domain.Entities;
using MCIApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MCIApi.Infrastructure.Services
{
    public class ProviderLocationService : IProviderLocationService
    {
        private readonly AppDbContext _context;

        public ProviderLocationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<IReadOnlyCollection<ProviderLocationListDto>>> GetAllAsync(int providerId, string lang, CancellationToken cancellationToken = default)
        {
            var provider = await _context.Providers.AsNoTracking().FirstOrDefaultAsync(p => p.Id == providerId && !p.IsDeleted, cancellationToken);
            if (provider == null)
                return ServiceResult<IReadOnlyCollection<ProviderLocationListDto>>.Fail(ServiceErrorType.NotFound, "ProviderNotFound");

            var locations = await _context.ProviderLocations
                .AsNoTracking()
                .Where(l => l.ProviderId == providerId && !l.IsDeleted)
                .ToListAsync(cancellationToken);

            var data = locations.Select(l => MapDto(l, provider, lang)).ToList().AsReadOnly();
            return ServiceResult<IReadOnlyCollection<ProviderLocationListDto>>.Ok(data);
        }

        public async Task<ServiceResult<ProviderLocationListDto>> GetByIdAsync(int providerId, int id, string lang, CancellationToken cancellationToken = default)
        {
            var provider = await _context.Providers.AsNoTracking().FirstOrDefaultAsync(p => p.Id == providerId && !p.IsDeleted, cancellationToken);
            if (provider == null)
                return ServiceResult<ProviderLocationListDto>.Fail(ServiceErrorType.NotFound, "ProviderNotFound");

            var location = await _context.ProviderLocations
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id && l.ProviderId == providerId && !l.IsDeleted, cancellationToken);

            if (location == null)
                return ServiceResult<ProviderLocationListDto>.Fail(ServiceErrorType.NotFound, "LocationNotFound");

            return ServiceResult<ProviderLocationListDto>.Ok(MapDto(location, provider, lang));
        }

        public async Task<ServiceResult<ProviderLocationListDto>> CreateAsync(int providerId, ProviderLocationCreateDto dto, string lang, CancellationToken cancellationToken = default)
        {
            var provider = await _context.Providers.FirstOrDefaultAsync(p => p.Id == providerId && !p.IsDeleted, cancellationToken);
            if (provider == null)
                return ServiceResult<ProviderLocationListDto>.Fail(ServiceErrorType.NotFound, "ProviderNotFound");

            var location = new ProviderLocation
            {
                ProviderId = providerId,
                GovernmentId = dto.GovernmentId,
                CityId = dto.CityId,
                StreetAr = dto.StreetAr,
                StreetEn = dto.StreetEn,
                PrimaryMobile = dto.PrimaryMobile,
                SecondaryMobile = dto.SecondaryMobile,
                PrimaryLandline = dto.PrimaryLandline,
                SecondaryLandline = dto.SecondaryLandline,
                GoogleMapsUrl = dto.GoogleMapsUrl,
                PortalEmail = dto.PortalEmail,
                PortalPassword = dto.PortalPassword,
                AllowChronic = dto.AllowChronic,
                IsDeleted = false
            };

            _context.ProviderLocations.Add(location);
            await _context.SaveChangesAsync(cancellationToken);

            return ServiceResult<ProviderLocationListDto>.Ok(MapDto(location, provider, lang));
        }

        public async Task<ServiceResult<ProviderLocationListDto>> UpdateAsync(int providerId, int id, ProviderLocationUpdateDto dto, string lang, CancellationToken cancellationToken = default)
        {
            var provider = await _context.Providers.FirstOrDefaultAsync(p => p.Id == providerId && !p.IsDeleted, cancellationToken);
            if (provider == null)
                return ServiceResult<ProviderLocationListDto>.Fail(ServiceErrorType.NotFound, "ProviderNotFound");

            var location = await _context.ProviderLocations.FirstOrDefaultAsync(l => l.Id == id && l.ProviderId == providerId && !l.IsDeleted, cancellationToken);
            if (location == null)
                return ServiceResult<ProviderLocationListDto>.Fail(ServiceErrorType.NotFound, "LocationNotFound");

            location.GovernmentId = dto.GovernmentId;
            location.CityId = dto.CityId;
            location.StreetAr = dto.StreetAr;
            location.StreetEn = dto.StreetEn;
            location.PrimaryMobile = dto.PrimaryMobile;
            location.SecondaryMobile = dto.SecondaryMobile;
            location.PrimaryLandline = dto.PrimaryLandline;
            location.SecondaryLandline = dto.SecondaryLandline;
            location.GoogleMapsUrl = dto.GoogleMapsUrl;
            location.PortalEmail = dto.PortalEmail;
            location.PortalPassword = dto.PortalPassword;
            location.AllowChronic = dto.AllowChronic;

            await _context.SaveChangesAsync(cancellationToken);

            return ServiceResult<ProviderLocationListDto>.Ok(MapDto(location, provider, lang));
        }

        public async Task<ServiceResult> DeleteAsync(int providerId, int id, string lang, CancellationToken cancellationToken = default)
        {
            var location = await _context.ProviderLocations.FirstOrDefaultAsync(l => l.Id == id && l.ProviderId == providerId && !l.IsDeleted, cancellationToken);
            if (location == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "LocationNotFound");

            location.IsDeleted = true;
            await _context.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok();
        }

        public async Task<ServiceResult<ProviderLocationListDto>> ToggleAllowChronicAsync(int providerId, int id, string lang, CancellationToken cancellationToken = default)
        {
            var provider = await _context.Providers.FirstOrDefaultAsync(p => p.Id == providerId && !p.IsDeleted, cancellationToken);
            if (provider == null)
                return ServiceResult<ProviderLocationListDto>.Fail(ServiceErrorType.NotFound, "ProviderNotFound");

            var location = await _context.ProviderLocations.FirstOrDefaultAsync(l => l.Id == id && l.ProviderId == providerId && !l.IsDeleted, cancellationToken);
            if (location == null)
                return ServiceResult<ProviderLocationListDto>.Fail(ServiceErrorType.NotFound, "LocationNotFound");

            location.AllowChronic = !location.AllowChronic;
            await _context.SaveChangesAsync(cancellationToken);

            return ServiceResult<ProviderLocationListDto>.Ok(MapDto(location, provider, lang));
        }

        private static ProviderLocationListDto MapDto(ProviderLocation location, Provider provider, string lang) => new()
        {
            Id = location.Id,
            ProviderId = location.ProviderId,
            ProviderName = lang == "ar" ? provider.NameAr : provider.NameEn,
            GovernmentId = location.GovernmentId,
            CityId = location.CityId,
            StreetAr = location.StreetAr,
            StreetEn = location.StreetEn,
            PrimaryMobile = location.PrimaryMobile,
            SecondaryMobile = location.SecondaryMobile,
            PrimaryLandline = location.PrimaryLandline,
            SecondaryLandline = location.SecondaryLandline,
            GoogleMapsUrl = location.GoogleMapsUrl,
            PortalEmail = location.PortalEmail,
            AllowChronic = location.AllowChronic
        };
    }
}

