using System.Linq;
using MCIApi.Application.Common;
using MCIApi.Application.InsuranceCompanies.DTOs;
using MCIApi.Application.InsuranceCompanies.Interfaces;
using MCIApi.Domain.Abstractions;
using MCIApi.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace MCIApi.Infrastructure.Services
{
    public class InsuranceCompanyService : IInsuranceCompanyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _environment;

        public InsuranceCompanyService(IUnitOfWork unitOfWork, IWebHostEnvironment environment)
        {
            _unitOfWork = unitOfWork;
            _environment = environment;
        }

        public async Task<ServiceResult<IReadOnlyList<InsuranceCompanyReadDto>>> GetAllAsync(string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<InsuranceCompany>();
            var companies = await repo.ListAsync(cancellationToken);
            var data = companies
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.EnName)
                .Select(c => Map(c, lang))
                .ToList()
                .AsReadOnly();

            return ServiceResult<IReadOnlyList<InsuranceCompanyReadDto>>.Ok(data);
        }

        public async Task<ServiceResult<InsuranceCompanyReadDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<InsuranceCompany>();
            var company = await repo.GetByIdAsync(id, cancellationToken);
            if (company == null || company.IsDeleted)
                return ServiceResult<InsuranceCompanyReadDto>.Fail(ServiceErrorType.NotFound, "InsuranceCompanyNotFound");

            return ServiceResult<InsuranceCompanyReadDto>.Ok(Map(company, lang));
        }

        public async Task<ServiceResult<InsuranceCompanyReadDto>> CreateAsync(InsuranceCompanyCreateDto dto, int employeeId, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<InsuranceCompany>();

            var entity = new InsuranceCompany
            {
                ArName = dto.ArName.Trim(),
                EnName = dto.EnName.Trim(),
                CreatedAt = DateTime.Now,
                CreatedBy = employeeId,
                IsDeleted = false
            };

            if (dto.ImageFile != null)
                entity.ImagePath = await SaveImageAsync(dto.ImageFile, cancellationToken);

            await repo.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult<InsuranceCompanyReadDto>.Ok(Map(entity, lang));
        }

        public async Task<ServiceResult<InsuranceCompanyReadDto>> UpdateAsync(int id, InsuranceCompanyUpdateDto dto, int employeeId, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<InsuranceCompany>();
            var entity = await repo.GetByIdAsync(id, cancellationToken);
            if (entity == null || entity.IsDeleted)
                return ServiceResult<InsuranceCompanyReadDto>.Fail(ServiceErrorType.NotFound, "InsuranceCompanyNotFound");

            if (!string.IsNullOrWhiteSpace(dto.ArName))
                entity.ArName = dto.ArName.Trim();

            if (!string.IsNullOrWhiteSpace(dto.EnName))
                entity.EnName = dto.EnName.Trim();

            if (dto.DeleteImage && !string.IsNullOrEmpty(entity.ImagePath))
            {
                DeleteImage(entity.ImagePath);
                entity.ImagePath = null;
            }

            if (dto.ImageFile != null)
            {
                if (!string.IsNullOrEmpty(entity.ImagePath))
                    DeleteImage(entity.ImagePath);

                entity.ImagePath = await SaveImageAsync(dto.ImageFile, cancellationToken);
            }

            entity.UpdatedBy = employeeId;
            entity.UpdatedAt = DateTime.Now;
            repo.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult<InsuranceCompanyReadDto>.Ok(Map(entity, lang));
        }

        public async Task<ServiceResult> DeleteAsync(int id, int employeeId, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<InsuranceCompany>();
            var entity = await repo.GetByIdAsync(id, cancellationToken);
            if (entity == null || entity.IsDeleted)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "InsuranceCompanyNotFound");

            entity.IsDeleted = true;
            entity.UpdatedBy = employeeId;
            entity.UpdatedAt = DateTime.Now;
            repo.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }

        private InsuranceCompanyReadDto Map(InsuranceCompany entity, string lang) => new InsuranceCompanyReadDto
        {
            Id = entity.Id,
            Name = lang == "en" ? entity.EnName : entity.ArName,
            ImageUrl = entity.ImagePath,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            CreatedBy = entity.CreatedBy,
            UpdatedBy = entity.UpdatedBy
        };

        private async Task<string> SaveImageAsync(IFormFile file, CancellationToken cancellationToken)
        {
            var uploadsRoot = Path.Combine(_environment.WebRootPath ?? Path.Combine(AppContext.BaseDirectory, "wwwroot"), "uploads", "insurance");
            if (!Directory.Exists(uploadsRoot))
                Directory.CreateDirectory(uploadsRoot);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsRoot, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream, cancellationToken);

            return $"/uploads/insurance/{fileName}";
        }

        private void DeleteImage(string? imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return;

            var filePath = Path.Combine(_environment.WebRootPath ?? Path.Combine(AppContext.BaseDirectory, "wwwroot"), imagePath.TrimStart('/', '\\'));
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}

