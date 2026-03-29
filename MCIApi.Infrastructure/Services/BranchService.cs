using System;
using System.Linq;
using MCIApi.Application.Branches.DTOs;
using MCIApi.Application.Branches.Interfaces;
using MCIApi.Application.Common;
using MCIApi.Domain.Abstractions;
using MCIApi.Domain.Entities;
using MCIApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MCIApi.Infrastructure.Services
{
    public class BranchService : IBranchService
    {
        private static readonly string[] AllowedStatuses = { "Active", "DeActive" };

        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public BranchService(AppDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<BranchPagedResultDto>> GetAllAsync(string lang, int page, int limit, CancellationToken cancellationToken = default)
        {
            if (page < 1) page = 1;
            if (limit < 1) limit = 5;

            var query = _context.Branches
                .AsNoTracking()
                .Where(b => !b.IsDeleted);

            var totalBranches = await query.CountAsync(cancellationToken);

            var data = await query
                .OrderByDescending(b => b.Id)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(b => new BranchListItemDto
                {
                    ClientId = b.ClientId,
                    ClientName = lang == "en"
                        ? (b.Client != null && !string.IsNullOrEmpty(b.Client.EnglishName) ? b.Client.EnglishName! : "Unknown")
                        : (b.Client != null && !string.IsNullOrEmpty(b.Client.ArabicName) ? b.Client.ArabicName! : "غير معروف"),
                    BranchId = b.Id,
                    BranchName = lang == "en"
                        ? (b.EnglishName ?? "Unknown")
                        : (b.ArabicName ?? "غير معروف"),
                    Status = b.Status,
                    MemberCount = _context.MemberInfos.Count(m => m.BranchId.HasValue && m.BranchId.Value == b.Id && !m.IsDeleted)
                })
                .ToListAsync(cancellationToken);

            var totalPages = (int)Math.Ceiling(totalBranches / (double)limit);

            var result = new BranchPagedResultDto
            {
                TotalBranches = totalBranches,
                CurrentPage = page,
                Limit = limit,
                TotalPages = totalPages,
                Data = data
            };

            return ServiceResult<BranchPagedResultDto>.Ok(result);
        }

        public async Task<ServiceResult<BranchDetailDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default)
        {
            var branch = await _context.Branches
                .AsNoTracking()
                .Where(b => b.Id == id && !b.IsDeleted)
                .Select(b => new BranchDetailDto
                {
                    ClientId = b.ClientId,
                    ClientName = lang == "en"
                        ? (b.Client != null && !string.IsNullOrEmpty(b.Client.EnglishName) ? b.Client.EnglishName! : "Unknown Client")
                        : (b.Client != null && !string.IsNullOrEmpty(b.Client.ArabicName) ? b.Client.ArabicName! : "عميل غير معروف"),
                    BranchId = b.Id,
                    BranchName = lang == "en"
                        ? (b.EnglishName ?? "Unknown Branch")
                        : (b.ArabicName ?? "فرع غير معروف"),
                    Status = b.Status,
                    MemberCount = _context.MemberInfos.Count(m => m.BranchId.HasValue && m.BranchId.Value == b.Id && !m.IsDeleted)
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (branch == null)
                return ServiceResult<BranchDetailDto>.Fail(ServiceErrorType.NotFound, "BranchNotFound");

            return ServiceResult<BranchDetailDto>.Ok(branch);
        }

        public async Task<ServiceResult<BranchDetailDto>> CreateAsync(CreateBranchDto dto, string lang, CancellationToken cancellationToken = default)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == dto.ClientId && !c.IsDeleted, cancellationToken);
            if (client == null)
                return ServiceResult<BranchDetailDto>.Fail(ServiceErrorType.Validation, "ClientNotFound");

            var repo = _unitOfWork.Repository<Branch>();

            var entity = new Branch
            {
                ClientId = dto.ClientId,
                ArabicName = dto.ArabicName,
                EnglishName = dto.EnglishName,
                Status = dto.Status ?? "Active",
                IsDeleted = false
            };

            await repo.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var branchName = lang == "en"
                ? (entity.EnglishName ?? "Unknown")
                : (entity.ArabicName ?? "غير معروف");

            var detail = new BranchDetailDto
            {
                ClientId = entity.ClientId,
                ClientName = lang == "en"
                    ? (client.EnglishName ?? "Unknown")
                    : (client.ArabicName ?? "غير معروف"),
                BranchId = entity.Id,
                BranchName = branchName,
                Status = entity.Status,
                MemberCount = 0
            };

            return ServiceResult<BranchDetailDto>.Ok(detail);
        }

        public async Task<ServiceResult<BranchDetailDto>> CreateForClientAsync(int clientId, CreateBranchForClientDto dto, string lang, CancellationToken cancellationToken = default)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == clientId && !c.IsDeleted, cancellationToken);
            if (client == null)
                return ServiceResult<BranchDetailDto>.Fail(ServiceErrorType.Validation, "ClientNotFound");

            // Validate BranchStatusId if provided
            if (dto.BranchStatusId.HasValue)
            {
                var statusExists = await _context.Statuses.AnyAsync(s => s.Id == dto.BranchStatusId.Value, cancellationToken);
                if (!statusExists)
                    return ServiceResult<BranchDetailDto>.Fail(ServiceErrorType.Validation, "BranchStatusNotFound");
            }

            var repo = _unitOfWork.Repository<Branch>();

            // Use BranchName for both ArabicName and EnglishName
            var entity = new Branch
            {
                ClientId = clientId,
                ArabicName = dto.BranchName,
                EnglishName = dto.BranchName,
                Status = "Active", // Default status
                BranchStatusId = dto.BranchStatusId,
                IsDeleted = false
            };

            await repo.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Get status name from BranchStatus if available, otherwise use Status string
            var statusName = entity.Status;
            if (entity.BranchStatusId.HasValue)
            {
                var branchStatus = await _context.Statuses
                    .FirstOrDefaultAsync(s => s.Id == entity.BranchStatusId.Value, cancellationToken);
                if (branchStatus != null)
                {
                    statusName = lang == "en" ? branchStatus.NameEn : branchStatus.NameAr;
                }
            }

            var detail = new BranchDetailDto
            {
                ClientId = entity.ClientId,
                ClientName = lang == "en"
                    ? (client.EnglishName ?? "Unknown")
                    : (client.ArabicName ?? "غير معروف"),
                BranchId = entity.Id,
                BranchName = dto.BranchName,
                Status = statusName,
                MemberCount = 0
            };

            return ServiceResult<BranchDetailDto>.Ok(detail);
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateBranchDto dto, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<Branch>();
            var branch = await repo.GetByIdAsync(id, cancellationToken);
            if (branch == null || branch.IsDeleted)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "BranchNotFound");

            if (dto.ClientId.HasValue)
            {
                var clientExists = await _context.Clients.AnyAsync(c => c.Id == dto.ClientId.Value && !c.IsDeleted, cancellationToken);
                if (!clientExists)
                    return ServiceResult.Fail(ServiceErrorType.Validation, "ClientNotFound");
                branch.ClientId = dto.ClientId.Value;
            }

            if (!string.IsNullOrWhiteSpace(dto.ArabicName))
                branch.ArabicName = dto.ArabicName;

            if (!string.IsNullOrWhiteSpace(dto.EnglishName))
                branch.EnglishName = dto.EnglishName;

            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                if (!AllowedStatuses.Contains(dto.Status))
                    return ServiceResult.Fail(ServiceErrorType.Validation, "InvalidStatus");
                branch.Status = dto.Status;
            }

            repo.Update(branch);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeleteAsync(int id, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<Branch>();
            var branch = await repo.GetByIdAsync(id, cancellationToken);
            if (branch == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "BranchNotFound");

            branch.IsDeleted = true;
            repo.Update(branch);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}


