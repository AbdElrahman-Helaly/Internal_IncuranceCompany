using System.Linq;
using MCIApi.Application.Common;
using MCIApi.Application.ProviderCategories.DTOs;
using MCIApi.Application.ProviderCategories.Interfaces;
using MCIApi.Domain.Abstractions;
using MCIApi.Domain.Entities;

namespace MCIApi.Infrastructure.Services
{
    public class ProviderCategoryService : IProviderCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProviderCategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<IReadOnlyList<ProviderCategoryDto>>> GetAllAsync(string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<ProviderCategory>();
            var items = await repo.ListAsync(cancellationToken);
            var data = items
                .OrderBy(c => c.NameEn)
                .Where(c => !c.IsDeleted)
                .Select(Map)
                .ToList()
                .AsReadOnly();

            return ServiceResult<IReadOnlyList<ProviderCategoryDto>>.Ok(data);
        }

        public async Task<ServiceResult<ProviderCategoryDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<ProviderCategory>();
            var category = await repo.GetByIdAsync(id, cancellationToken);
            if (category == null || category.IsDeleted)
                return ServiceResult<ProviderCategoryDto>.Fail(ServiceErrorType.NotFound, "CategoryNotFound");

            return ServiceResult<ProviderCategoryDto>.Ok(Map(category));
        }

        public async Task<ServiceResult<ProviderCategoryDto>> CreateAsync(ProviderCategoryCreateDto dto, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<ProviderCategory>();
            var all = await repo.ListAsync(cancellationToken);
            if (all.Any(c => !c.IsDeleted && c.NameAr.Trim().Equals(dto.NameAr.Trim(), StringComparison.OrdinalIgnoreCase)))
                return ServiceResult<ProviderCategoryDto>.Fail(ServiceErrorType.Conflict, "CategoryArabicNameExists");

            if (all.Any(c => !c.IsDeleted && c.NameEn.Trim().Equals(dto.NameEn.Trim(), StringComparison.OrdinalIgnoreCase)))
                return ServiceResult<ProviderCategoryDto>.Fail(ServiceErrorType.Conflict, "CategoryEnglishNameExists");

            var entity = new ProviderCategory
            {
                NameAr = dto.NameAr.Trim(),
                NameEn = dto.NameEn.Trim()
            };

            await repo.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult<ProviderCategoryDto>.Ok(Map(entity));
        }

        public async Task<ServiceResult<ProviderCategoryDto>> UpdateAsync(int id, ProviderCategoryUpdateDto dto, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<ProviderCategory>();
            var category = await repo.GetByIdAsync(id, cancellationToken);
            if (category == null || category.IsDeleted)
                return ServiceResult<ProviderCategoryDto>.Fail(ServiceErrorType.NotFound, "CategoryNotFound");

            var all = await repo.ListAsync(cancellationToken);
            if (all.Any(c => c.Id != id && !c.IsDeleted && c.NameAr.Trim().Equals(dto.NameAr.Trim(), StringComparison.OrdinalIgnoreCase)))
                return ServiceResult<ProviderCategoryDto>.Fail(ServiceErrorType.Conflict, "CategoryArabicNameExists");

            if (all.Any(c => c.Id != id && !c.IsDeleted && c.NameEn.Trim().Equals(dto.NameEn.Trim(), StringComparison.OrdinalIgnoreCase)))
                return ServiceResult<ProviderCategoryDto>.Fail(ServiceErrorType.Conflict, "CategoryEnglishNameExists");

            category.NameAr = dto.NameAr.Trim();
            category.NameEn = dto.NameEn.Trim();
            repo.Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult<ProviderCategoryDto>.Ok(Map(category));
        }

        public async Task<ServiceResult> DeleteAsync(int id, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<ProviderCategory>();
            var category = await repo.GetByIdAsync(id, cancellationToken);
            if (category == null || category.IsDeleted)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "CategoryNotFound");

            category.IsDeleted = true;
            repo.Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }

        private static ProviderCategoryDto Map(ProviderCategory entity) => new ProviderCategoryDto
        {
            Id = entity.Id,
            NameAr = entity.NameAr,
            NameEn = entity.NameEn
        };
    }
}

