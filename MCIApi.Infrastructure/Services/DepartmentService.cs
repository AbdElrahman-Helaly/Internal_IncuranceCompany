using System.Linq;
using MCIApi.Application.Common;
using MCIApi.Application.Departments.DTOs;
using MCIApi.Application.Departments.Interfaces;
using MCIApi.Domain.Abstractions;
using MCIApi.Domain.Entities;

namespace MCIApi.Infrastructure.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<IReadOnlyList<DepartmentListItemDto>>> GetAllAsync(string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<Department>();
            var departments = await repo.ListAsync(cancellationToken);
            var result = departments
                .OrderBy(d => d.Id)
                .Select(d => new DepartmentListItemDto
                {
                    Id = d.Id,
                    Name = lang == "ar" ? d.NameAr : d.NameEn
                })
                .ToList()
                .AsReadOnly();

            return ServiceResult<IReadOnlyList<DepartmentListItemDto>>.Ok(result);
        }

        public async Task<ServiceResult<DepartmentDetailDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<Department>();
            var department = await repo.GetByIdAsync(id, cancellationToken);
            if (department == null)
                return ServiceResult<DepartmentDetailDto>.Fail(ServiceErrorType.NotFound, "DepartmentNotFound");

            var dto = new DepartmentDetailDto
            {
                Id = department.Id,
                NameAr = department.NameAr,
                NameEn = department.NameEn
            };

            return ServiceResult<DepartmentDetailDto>.Ok(dto);
        }

        public async Task<ServiceResult<DepartmentDetailDto>> CreateAsync(DepartmentCreateDto dto, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<Department>();
            var departments = await repo.ListAsync(cancellationToken);

            if (departments.Any(d => d.NameAr.Trim().Equals(dto.NameAr.Trim(), StringComparison.OrdinalIgnoreCase)))
                return ServiceResult<DepartmentDetailDto>.Fail(ServiceErrorType.Conflict, "DepartmentArabicNameExists");

            if (departments.Any(d => d.NameEn.Trim().Equals(dto.NameEn.Trim(), StringComparison.OrdinalIgnoreCase)))
                return ServiceResult<DepartmentDetailDto>.Fail(ServiceErrorType.Conflict, "DepartmentEnglishNameExists");

            var entity = new Department
            {
                NameAr = dto.NameAr.Trim(),
                NameEn = dto.NameEn.Trim()
            };

            await repo.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = new DepartmentDetailDto
            {
                Id = entity.Id,
                NameAr = entity.NameAr,
                NameEn = entity.NameEn
            };

            return ServiceResult<DepartmentDetailDto>.Ok(result);
        }

        public async Task<ServiceResult> UpdateAsync(int id, DepartmentCreateDto dto, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<Department>();
            var department = await repo.GetByIdAsync(id, cancellationToken);
            if (department == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "DepartmentNotFound");

            var departments = await repo.ListAsync(cancellationToken);
            if (departments.Any(d => d.Id != id && d.NameAr.Trim().Equals(dto.NameAr.Trim(), StringComparison.OrdinalIgnoreCase)))
                return ServiceResult.Fail(ServiceErrorType.Conflict, "DepartmentArabicNameExists");

            if (departments.Any(d => d.Id != id && d.NameEn.Trim().Equals(dto.NameEn.Trim(), StringComparison.OrdinalIgnoreCase)))
                return ServiceResult.Fail(ServiceErrorType.Conflict, "DepartmentEnglishNameExists");

            department.NameAr = dto.NameAr.Trim();
            department.NameEn = dto.NameEn.Trim();
            repo.Update(department);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeleteAsync(int id, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<Department>();
            var department = await repo.GetByIdAsync(id, cancellationToken);
            if (department == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "DepartmentNotFound");

            repo.Delete(department);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}


