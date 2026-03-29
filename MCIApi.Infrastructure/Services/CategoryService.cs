using System.Linq;
using MCIApi.Application.Categories.DTOs;
using MCIApi.Application.Categories.Interfaces;
using MCIApi.Application.Common;
using MCIApi.Domain.Abstractions;
using MCIApi.Domain.Entities;

namespace MCIApi.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<IReadOnlyList<CategoryReadDto>>> GetAllAsync(string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<Category>();
            var categories = await repo.ListAsync(cancellationToken);
            var result = categories
                .OrderBy(c => c.Name)
                .Select(MapToReadDto)
                .ToList()
                .AsReadOnly();

            return ServiceResult<IReadOnlyList<CategoryReadDto>>.Ok(result);
        }

        public async Task<ServiceResult<CategoryReadDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<Category>();
            var category = await repo.GetByIdAsync(id, cancellationToken);
            if (category == null)
                return ServiceResult<CategoryReadDto>.Fail(ServiceErrorType.NotFound, "CategoryNotFound");

            return ServiceResult<CategoryReadDto>.Ok(MapToReadDto(category));
        }

        public async Task<ServiceResult<CategoryReadDto>> CreateAsync(CategoryCreateDto dto, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<Category>();
            var categories = await repo.ListAsync(cancellationToken);
            if (categories.Any(c => c.Name.Equals(dto.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
                return ServiceResult<CategoryReadDto>.Fail(ServiceErrorType.Conflict, "CategoryNameExists");

            var entity = new Category
            {
                Name = dto.Name.Trim()
            };

            await repo.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult<CategoryReadDto>.Ok(MapToReadDto(entity));
        }

        public async Task<ServiceResult<CategoryReadDto>> UpdateAsync(int id, CategoryUpdateDto dto, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<Category>();
            var category = await repo.GetByIdAsync(id, cancellationToken);
            if (category == null)
                return ServiceResult<CategoryReadDto>.Fail(ServiceErrorType.NotFound, "CategoryNotFound");

            var categories = await repo.ListAsync(cancellationToken);
            if (categories.Any(c => c.Id != id && c.Name.Equals(dto.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
                return ServiceResult<CategoryReadDto>.Fail(ServiceErrorType.Conflict, "CategoryNameExists");

            category.Name = dto.Name.Trim();
            repo.Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult<CategoryReadDto>.Ok(MapToReadDto(category));
        }

        public async Task<ServiceResult> DeleteAsync(int id, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<Category>();
            var category = await repo.GetByIdAsync(id, cancellationToken);
            if (category == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "CategoryNotFound");

            repo.Delete(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }

        private static CategoryReadDto MapToReadDto(Category category)
        {
            return new CategoryReadDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }
    }
}

