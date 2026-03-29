using MCIApi.Application.Common;
using MCIApi.Application.JobTitles.DTOs;
using MCIApi.Application.JobTitles.Interfaces;
using MCIApi.Domain.Abstractions;
using MCIApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MCIApi.Infrastructure.Services
{
    public class JobTitleService : IJobTitleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public JobTitleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<IReadOnlyList<JobTitleListItemDto>>> GetAllAsync(string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<JobTitle>();
            var jobTitles = await repo.ListAsync(cancellationToken);
            var result = jobTitles
                .OrderBy(j => j.Id)
                .Select(j => new JobTitleListItemDto
                {
                    Id = j.Id,
                    Name = lang.Equals("ar", StringComparison.OrdinalIgnoreCase) ? j.NameAr : j.NameEn
                })
                .ToList()
                .AsReadOnly();

            return ServiceResult<IReadOnlyList<JobTitleListItemDto>>.Ok(result);
        }

        public async Task<ServiceResult<JobTitleDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<JobTitle>();
            var jobTitle = await repo.GetByIdAsync(id, cancellationToken);
            if (jobTitle == null)
                return ServiceResult<JobTitleDto>.Fail(ServiceErrorType.NotFound, "JobTitleNotFound");

            var dto = new JobTitleDto
            {
                Id = jobTitle.Id,
                NameAr = jobTitle.NameAr,
                NameEn = jobTitle.NameEn
            };

            return ServiceResult<JobTitleDto>.Ok(dto);
        }

        public async Task<ServiceResult<JobTitleDto>> CreateAsync(CreateJobTitleDto dto, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<JobTitle>();
            var existing = await repo.ListAsync(cancellationToken);
            if (existing.Any(j => j.NameAr == dto.NameAr || j.NameEn == dto.NameEn))
                return ServiceResult<JobTitleDto>.Fail(ServiceErrorType.Conflict, "JobTitleAlreadyExists");

            var entity = new JobTitle
            {
                NameAr = dto.NameAr,
                NameEn = dto.NameEn
            };

            await repo.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = new JobTitleDto
            {
                Id = entity.Id,
                NameAr = entity.NameAr,
                NameEn = entity.NameEn
            };

            return ServiceResult<JobTitleDto>.Ok(result);
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateJobTitleDto dto, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<JobTitle>();
            var jobTitle = await repo.GetByIdAsync(id, cancellationToken);
            if (jobTitle == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "JobTitleNotFound");

            var list = await repo.ListAsync(cancellationToken);
            if (!string.IsNullOrWhiteSpace(dto.NameAr))
            {
                if (list.Any(j => j.Id != id && j.NameAr == dto.NameAr))
                    return ServiceResult.Fail(ServiceErrorType.Conflict, "JobTitleAlreadyExists");
                jobTitle.NameAr = dto.NameAr;
            }

            if (!string.IsNullOrWhiteSpace(dto.NameEn))
            {
                if (list.Any(j => j.Id != id && j.NameEn == dto.NameEn))
                    return ServiceResult.Fail(ServiceErrorType.Conflict, "JobTitleAlreadyExists");
                jobTitle.NameEn = dto.NameEn;
            }

            repo.Update(jobTitle);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeleteAsync(int id, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<JobTitle>();
            var jobTitle = await repo.GetByIdAsync(id, cancellationToken);
            if (jobTitle == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "JobTitleNotFound");

            repo.Delete(jobTitle);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}


