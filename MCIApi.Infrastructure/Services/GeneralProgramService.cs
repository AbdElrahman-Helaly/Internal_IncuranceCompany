using System.Linq;
using MCIApi.Application.Common;
using MCIApi.Application.GeneralPrograms.DTOs;
using MCIApi.Application.GeneralPrograms.Interfaces;
using MCIApi.Domain.Abstractions;
using MCIApi.Domain.Entities;
using MCIApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MCIApi.Infrastructure.Services
{
    public class GeneralProgramService : IGeneralProgramService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _context;

        public GeneralProgramService(IUnitOfWork unitOfWork, AppDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<ServiceResult<IReadOnlyList<GeneralProgramReadDto>>> GetAllAsync(string lang, CancellationToken cancellationToken = default)
        {
            var items = await _context.GeneralPrograms
                .AsNoTracking()
                .Include(gp => gp.ProgramName)
                .OrderBy(p => p.Id)
                .ToListAsync(cancellationToken);
            
            var data = items
                .Select(gp => Map(gp, lang))
                .ToList()
                .AsReadOnly();

            return ServiceResult<IReadOnlyList<GeneralProgramReadDto>>.Ok(data);
        }

        public async Task<ServiceResult<GeneralProgramReadDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default)
        {
            var entity = await _context.GeneralPrograms
                .AsNoTracking()
                .Include(gp => gp.ProgramName)
                .FirstOrDefaultAsync(gp => gp.Id == id, cancellationToken);
            
            if (entity == null)
                return ServiceResult<GeneralProgramReadDto>.Fail(ServiceErrorType.NotFound, "GeneralProgramNotFound");

            return ServiceResult<GeneralProgramReadDto>.Ok(Map(entity, lang));
        }

        public async Task<ServiceResult<GeneralProgramReadDto>> CreateAsync(GeneralProgramCreateDto dto, string lang, CancellationToken cancellationToken = default)
        {
            var policyRepo = _unitOfWork.Repository<Policy>();
            var policy = await policyRepo.GetByIdAsync(dto.PolicyId, cancellationToken);
            if (policy == null || policy.IsDeleted)
                return ServiceResult<GeneralProgramReadDto>.Fail(ServiceErrorType.Validation, "InvalidPolicy");

            // Validate ProgramNameId
            var programNameExists = await _context.Programs.AnyAsync(p => p.Id == dto.ProgramNameId && !p.IsDeleted, cancellationToken);
            if (!programNameExists)
                return ServiceResult<GeneralProgramReadDto>.Fail(ServiceErrorType.Validation, "ProgramNameNotFound");

            var repo = _unitOfWork.Repository<GeneralProgram>();
            var entity = new GeneralProgram
            {
                ProgramNameId = dto.ProgramNameId,
                Limit = dto.Limit,
                RoomTypeId = dto.RoomTypeId,
                Note = dto.Note?.Trim(),
                PolicyId = dto.PolicyId
            };

            await repo.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Reload with navigation property
            var createdEntity = await _context.GeneralPrograms
                .AsNoTracking()
                .Include(gp => gp.ProgramName)
                .FirstOrDefaultAsync(gp => gp.Id == entity.Id, cancellationToken);

            return ServiceResult<GeneralProgramReadDto>.Ok(Map(createdEntity!, lang));
        }

        public async Task<ServiceResult<GeneralProgramReadDto>> UpdateAsync(int id, GeneralProgramUpdateDto dto, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<GeneralProgram>();
            var entity = await repo.GetByIdAsync(id, cancellationToken);
            if (entity == null)
                return ServiceResult<GeneralProgramReadDto>.Fail(ServiceErrorType.NotFound, "GeneralProgramNotFound");

            var policyRepo = _unitOfWork.Repository<Policy>();
            var policy = await policyRepo.GetByIdAsync(dto.PolicyId, cancellationToken);
            if (policy == null || policy.IsDeleted)
                return ServiceResult<GeneralProgramReadDto>.Fail(ServiceErrorType.Validation, "InvalidPolicy");

            // Validate ProgramNameId
            var programNameExists = await _context.Programs.AnyAsync(p => p.Id == dto.ProgramNameId && !p.IsDeleted, cancellationToken);
            if (!programNameExists)
                return ServiceResult<GeneralProgramReadDto>.Fail(ServiceErrorType.Validation, "ProgramNameNotFound");

            entity.ProgramNameId = dto.ProgramNameId;
            entity.Limit = dto.Limit;
            entity.RoomTypeId = dto.RoomTypeId;
            entity.Note = dto.Note?.Trim();
            entity.PolicyId = dto.PolicyId;
            repo.Update(entity);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Reload with navigation property
            var updatedEntity = await _context.GeneralPrograms
                .AsNoTracking()
                .Include(gp => gp.ProgramName)
                .FirstOrDefaultAsync(gp => gp.Id == entity.Id, cancellationToken);

            return ServiceResult<GeneralProgramReadDto>.Ok(Map(updatedEntity!, lang));
        }

        public async Task<ServiceResult> DeleteAsync(int id, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<GeneralProgram>();
            var entity = await repo.GetByIdAsync(id, cancellationToken);
            if (entity == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "GeneralProgramNotFound");

            repo.Delete(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }

        private static GeneralProgramReadDto Map(GeneralProgram entity, string lang) => new GeneralProgramReadDto
        {
            Id = entity.Id,
            ProgramNameId = entity.ProgramNameId,
            ProgramName = entity.ProgramName != null ? (lang == "ar" ? entity.ProgramName.NameAr : entity.ProgramName.NameEn) : null,
            Limit = entity.Limit,
            RoomTypeId = entity.RoomTypeId,
            Note = entity.Note,
            PolicyId = entity.PolicyId
        };
    }
}

