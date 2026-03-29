using MCIApi.Application.Batches.DTOs;
using MCIApi.Application.Batches.Interfaces;
using MCIApi.Application.Common;
using MCIApi.Domain.Abstractions;
using MCIApi.Domain.Entities;
using MCIApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MCIApi.Infrastructure.Services
{
    public class BatchService : IBatchService
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public BatchService(AppDbContext context, IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<ServiceResult<BatchPagedResultDto>> GetAllAsync(int page, int limit, string lang, CancellationToken cancellationToken = default)
        {
            if (page < 1) page = 1;
            if (limit < 1) limit = 10;

            var query = _context.Batches
                .AsNoTracking()
                .Include(b => b.Provider)
                .Include(b => b.ReceivingWay)
                .Include(b => b.Reason)
                .Include(b => b.BatchStatus)
                .Where(b => !b.IsDeleted)
                .OrderByDescending(b => b.Id);

            var totalBatches = await query.CountAsync(cancellationToken);
            var batches = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync(cancellationToken);

            var dto = new BatchPagedResultDto
            {
                TotalBatches = totalBatches,
                CurrentPage = page,
                Limit = limit,
                TotalPages = (int)Math.Ceiling(totalBatches / (double)limit),
                Data = batches
                    .Select(b => MapListItem(b, lang))
                    .ToList()
            };

            return ServiceResult<BatchPagedResultDto>.Ok(dto);
        }

        public async Task<ServiceResult<BatchDetailDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default)
        {
            var batch = await _context.Batches
                .AsNoTracking()
                .Include(b => b.Provider)
                .Include(b => b.ReceivingWay)
                .Include(b => b.BatchStatus)
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted, cancellationToken);

            if (batch == null)
                return ServiceResult<BatchDetailDto>.Fail(ServiceErrorType.NotFound, "BatchNotFound");

            return ServiceResult<BatchDetailDto>.Ok(MapDetail(batch, lang));
        }

        public async Task<ServiceResult<BatchCreateResponseDto>> CreateAsync(BatchCreateDto dto, string userId, CancellationToken cancellationToken = default)
        {
            // Validate user exists in AspNetUsers
            // First try to find by ID (GUID string)
            var user = await _userManager.FindByIdAsync(userId);
            
            // If not found by ID, try to find by UserName (in case userId is actually a username)
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(userId);
            }
            
            // If still not found, check if userId might be an Employee ID (numeric string)
            if (user == null && int.TryParse(userId, out var employeeId))
            {
                // Try to find Employee and get their IdentityUserId
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId && !e.IsDeleted, cancellationToken);
                if (employee != null && !string.IsNullOrEmpty(employee.IdentityUserId))
                {
                    // Use the IdentityUserId from Employee
                    user = await _userManager.FindByIdAsync(employee.IdentityUserId);
                    if (user != null)
                    {
                        userId = employee.IdentityUserId; // Update userId to the correct Identity User ID
                    }
                }
            }
            
            if (user == null)
            {
                return ServiceResult<BatchCreateResponseDto>.Fail(ServiceErrorType.Validation, $"InvalidUserId: User with ID/Name '{userId}' not found in AspNetUsers. Please ensure you are logged in with a User account (not Employee account) or the Employee is linked to an Identity User.");
            }

            // Ensure we use the actual User ID (GUID) from AspNetUsers, not Employee ID
            var actualUserId = user.Id;

            var providerExists = await _context.Providers.AnyAsync(p => p.Id == dto.ProviderId && !p.IsDeleted, cancellationToken);
            if (!providerExists)
                return ServiceResult<BatchCreateResponseDto>.Fail(ServiceErrorType.Validation, "InvalidProviderId");

            // Validate ReceivingWayId
            var receivingWayExists = await _context.ReceivingWays.AnyAsync(rw => rw.Id == dto.ReceivingWayId && !rw.IsDeleted, cancellationToken);
            if (!receivingWayExists)
                return ServiceResult<BatchCreateResponseDto>.Fail(ServiceErrorType.Validation, "InvalidReceivingWayId");

            // Validate BatchStatusId if provided, otherwise use default (Received = 1)
            int? batchStatusId = dto.BatchStatusId;
            if (!batchStatusId.HasValue)
            {
                // Default to "Received" (Id = 1) if not provided
                batchStatusId = 1;
            }

            var batchStatusExists = await _context.BatchStatuses.AnyAsync(bs => bs.Id == batchStatusId.Value && !bs.IsDeleted, cancellationToken);
            if (!batchStatusExists)
                return ServiceResult<BatchCreateResponseDto>.Fail(ServiceErrorType.Validation, "InvalidBatchStatusId");

            var repo = _unitOfWork.Repository<Batch>();
            var today = DateTime.Now.Date;

            var entity = new Batch
            {
                ReceiveDate = today,
                ProviderId = dto.ProviderId,
                CreatedBy = actualUserId, // Use the actual User ID from AspNetUsers
                CreatedAt = DateTime.Now,
                ReceivedClaimsCount = dto.ReceivedClaimsCount,
                ReceivedTotalAmount = dto.ReceivedTotalAmount,
                ReceivingWayId = dto.ReceivingWayId,
                ReasonId = dto.ReasonId,
                BatchStatusId = batchStatusId,
                BatchDueDays = dto.BatchDueDays,
                BatchDueDate = today.AddDays(dto.BatchDueDays),
                UploadOnPortal = dto.UploadOnPortal,
                Reviewed = dto.Reviewed,
                IsDeleted = false
            };

            await repo.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new BatchCreateResponseDto
            {
                Message = "Batch created successfully",
                Id = entity.Id,
                CreatedBy = actualUserId, // Use the actual User ID
                ReceiveDate = entity.ReceiveDate.ToString("yyyy-MM-dd"),
                CreatedAt = entity.CreatedAt.ToString("yyyy-MM-dd")
            };

            return ServiceResult<BatchCreateResponseDto>.Ok(response);
        }

        public async Task<ServiceResult<BatchUpdateResponseDto>> UpdateAsync(int id, BatchUpdateDto dto, string userId, CancellationToken cancellationToken = default)
        {
            // Validate user exists in AspNetUsers if userId is provided
            string? actualUserId = null;
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(userId);
                }
                
                if (user == null && int.TryParse(userId, out var employeeId))
                {
                    var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId && !e.IsDeleted, cancellationToken);
                    if (employee != null && !string.IsNullOrEmpty(employee.IdentityUserId))
                    {
                        user = await _userManager.FindByIdAsync(employee.IdentityUserId);
                        if (user != null)
                        {
                            actualUserId = employee.IdentityUserId;
                        }
                    }
                }
                else if (user != null)
                {
                    actualUserId = user.Id;
                }
                
                if (actualUserId == null)
                {
                    return ServiceResult<BatchUpdateResponseDto>.Fail(ServiceErrorType.Validation, $"InvalidUserId: User with ID/Name '{userId}' not found in AspNetUsers.");
                }
            }

            var repo = _unitOfWork.Repository<Batch>();
            var batch = await repo.GetByIdAsync(id, cancellationToken);
            if (batch == null || batch.IsDeleted)
                return ServiceResult<BatchUpdateResponseDto>.Fail(ServiceErrorType.NotFound, "BatchNotFound");

            var dueDateNeedsRecalc = false;

            if (dto.ReceiveDate.HasValue)
            {
                batch.ReceiveDate = dto.ReceiveDate.Value;
                dueDateNeedsRecalc = true;
            }

            if (dto.ProviderId.HasValue)
            {
                var providerExists = await _context.Providers.AnyAsync(p => p.Id == dto.ProviderId.Value && !p.IsDeleted, cancellationToken);
                if (!providerExists)
                    return ServiceResult<BatchUpdateResponseDto>.Fail(ServiceErrorType.Validation, "InvalidProviderId");
                batch.ProviderId = dto.ProviderId.Value;
            }

            if (dto.ReceivedClaimsCount.HasValue)
                batch.ReceivedClaimsCount = dto.ReceivedClaimsCount.Value;

            if (dto.ReceivedTotalAmount.HasValue)
                batch.ReceivedTotalAmount = dto.ReceivedTotalAmount.Value;

            if (dto.ReceivingWayId.HasValue)
            {
                var receivingWayExists = await _context.ReceivingWays.AnyAsync(rw => rw.Id == dto.ReceivingWayId.Value && !rw.IsDeleted, cancellationToken);
                if (!receivingWayExists)
                    return ServiceResult<BatchUpdateResponseDto>.Fail(ServiceErrorType.Validation, "InvalidReceivingWayId");
                batch.ReceivingWayId = dto.ReceivingWayId.Value;
            }

            if (dto.ReasonId.HasValue)
            {
                var reasonExists = await _context.Reasons.AnyAsync(r => r.Id == dto.ReasonId.Value && !r.IsDeleted, cancellationToken);
                if (!reasonExists)
                    return ServiceResult<BatchUpdateResponseDto>.Fail(ServiceErrorType.Validation, "InvalidReasonId");
                batch.ReasonId = dto.ReasonId.Value;
            }

            if (dto.BatchStatusId.HasValue)
            {
                var batchStatusExists = await _context.BatchStatuses.AnyAsync(bs => bs.Id == dto.BatchStatusId.Value && !bs.IsDeleted, cancellationToken);
                if (!batchStatusExists)
                    return ServiceResult<BatchUpdateResponseDto>.Fail(ServiceErrorType.Validation, "InvalidBatchStatusId");
                batch.BatchStatusId = dto.BatchStatusId.Value;
            }

            if (dto.BatchDueDays.HasValue && dto.BatchDueDays.Value > 0)
            {
                batch.BatchDueDays = dto.BatchDueDays.Value;
                dueDateNeedsRecalc = true;
            }

            if (dto.UploadOnPortal.HasValue)
                batch.UploadOnPortal = dto.UploadOnPortal.Value;

            if (dto.Reviewed.HasValue)
                batch.Reviewed = dto.Reviewed.Value;

            if (dto.IsDeleted.HasValue)
                batch.IsDeleted = dto.IsDeleted.Value;

            if (dueDateNeedsRecalc)
                batch.BatchDueDate = batch.ReceiveDate.AddDays(batch.BatchDueDays);

            batch.UpdatedBy = actualUserId; // Use the actual User ID
            batch.UpdatedAt = DateTime.Now;

            repo.Update(batch);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new BatchUpdateResponseDto
            {
                Message = "Batch updated successfully",
                UpdatedBy = actualUserId, // Use the actual User ID
                UpdatedAt = batch.UpdatedAt?.ToString("yyyy-MM-dd"),
                BatchDueDate = batch.BatchDueDate.ToString("yyyy-MM-dd")
            };

            return ServiceResult<BatchUpdateResponseDto>.Ok(response);
        }

        public async Task<ServiceResult<BatchDeleteResponseDto>> DeleteAsync(int id, string userId, CancellationToken cancellationToken = default)
        {
            // Validate user exists in AspNetUsers if userId is provided
            string? actualUserId = null;
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(userId);
                }
                
                if (user == null && int.TryParse(userId, out var employeeId))
                {
                    var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId && !e.IsDeleted, cancellationToken);
                    if (employee != null && !string.IsNullOrEmpty(employee.IdentityUserId))
                    {
                        user = await _userManager.FindByIdAsync(employee.IdentityUserId);
                        if (user != null)
                        {
                            actualUserId = employee.IdentityUserId;
                        }
                    }
                }
                else if (user != null)
                {
                    actualUserId = user.Id;
                }
                
                if (actualUserId == null)
                {
                    return ServiceResult<BatchDeleteResponseDto>.Fail(ServiceErrorType.Validation, $"InvalidUserId: User with ID/Name '{userId}' not found in AspNetUsers.");
                }
            }

            var repo = _unitOfWork.Repository<Batch>();
            var batch = await repo.GetByIdAsync(id, cancellationToken);
            if (batch == null || batch.IsDeleted)
                return ServiceResult<BatchDeleteResponseDto>.Fail(ServiceErrorType.NotFound, "BatchNotFound");

            batch.IsDeleted = true;
            batch.UpdatedBy = actualUserId; // Use the actual User ID
            batch.UpdatedAt = DateTime.Now;

            repo.Update(batch);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new BatchDeleteResponseDto
            {
                Message = "Batch deleted successfully",
                UpdatedBy = actualUserId, // Use the actual User ID
                UpdatedAt = batch.UpdatedAt?.ToString("yyyy-MM-dd")
            };

            return ServiceResult<BatchDeleteResponseDto>.Ok(response);
        }

        public async Task<ServiceResult<BatchReviewResponseDto>> MarkAsReviewedAsync(int id, string userId, CancellationToken cancellationToken = default)
        {
            // Validate user exists in AspNetUsers
            string? actualUserId = null;
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(userId);
                }
                
                if (user == null && int.TryParse(userId, out var employeeId))
                {
                    var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId && !e.IsDeleted, cancellationToken);
                    if (employee != null && !string.IsNullOrEmpty(employee.IdentityUserId))
                    {
                        user = await _userManager.FindByIdAsync(employee.IdentityUserId);
                        if (user != null)
                        {
                            actualUserId = employee.IdentityUserId;
                        }
                    }
                }
                else if (user != null)
                {
                    actualUserId = user.Id;
                }
                
                if (actualUserId == null)
                {
                    return ServiceResult<BatchReviewResponseDto>.Fail(ServiceErrorType.Validation, $"InvalidUserId: User with ID/Name '{userId}' not found in AspNetUsers.");
                }
            }

            var repo = _unitOfWork.Repository<Batch>();
            var batch = await repo.GetByIdAsync(id, cancellationToken);
            if (batch == null || batch.IsDeleted)
                return ServiceResult<BatchReviewResponseDto>.Fail(ServiceErrorType.NotFound, "BatchNotFound");

            // Update Batch to Reviewed
            batch.Reviewed = true;
            batch.UpdatedBy = actualUserId;
            batch.UpdatedAt = DateTime.Now;

            repo.Update(batch);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new BatchReviewResponseDto
            {
                Message = "Batch marked as reviewed successfully",
                Id = batch.Id,
                Reviewed = batch.Reviewed,
                ReviewedBy = actualUserId ?? string.Empty,
                ReviewedAt = batch.UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty
            };

            return ServiceResult<BatchReviewResponseDto>.Ok(response);
        }

        private string? GetUserName(string? userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;

            try
            {
                var user = _userManager.FindByIdAsync(userId).GetAwaiter().GetResult();
                return user?.UserName;
            }
            catch
            {
                return userId;
            }
        }

        private BatchListItemDto MapListItem(Batch batch, string lang) => new()
        {
            Id = batch.Id,
            ReceiveDate = batch.ReceiveDate.ToString("yyyy-MM-dd"),
            ProviderId = batch.ProviderId,
            ProviderName = batch.Provider?.NameEn,
            CreatedBy = batch.CreatedBy,
            CreatedByName = GetUserName(batch.CreatedBy),
            CreatedAt = batch.CreatedAt.ToString("yyyy-MM-dd"),
            UpdatedBy = batch.UpdatedBy,
            UpdatedAt = batch.UpdatedAt?.ToString("yyyy-MM-dd"),
            ReceivedClaimsCount = batch.ReceivedClaimsCount,
            ScanCount = batch.ReceivedClaimsCount, // scanned claims = existing claims in batch
            ReceivedTotalAmount = batch.ReceivedTotalAmount,
            ActualAmount = batch.ReceivedTotalAmount,
            ReceivingWayId = batch.ReceivingWayId,
            ReceivingWayName = batch.ReceivingWay != null ? (lang == "ar" ? batch.ReceivingWay.NameAr : batch.ReceivingWay.NameEn) : null,
            ReasonId = batch.ReasonId,
            ReasonName = batch.Reason != null ? (lang == "ar" ? batch.Reason.NameAr : batch.Reason.NameEn) : null,
            BatchStatusId = batch.BatchStatusId,
            BatchStatusName = batch.BatchStatus != null ? (lang == "ar" ? batch.BatchStatus.NameAr : batch.BatchStatus.NameEn) : null,
            BatchDueDays = batch.BatchDueDays,
            BatchDueDate = batch.BatchDueDate.ToString("yyyy-MM-dd"),
            UploadOnPortal = batch.UploadOnPortal ? "Yes" : "No",
            Reviewed = batch.Reviewed ? "Yes" : "No",
            NeedReview = batch.Reviewed ? "No" : "Yes"
        };

        private BatchDetailDto MapDetail(Batch batch, string lang) => new()
        {
            Id = batch.Id,
            ReceiveDate = batch.ReceiveDate.ToString("yyyy-MM-dd"),
            ProviderId = batch.ProviderId,
            ProviderName = batch.Provider?.NameEn,
            CreatedByName = GetUserName(batch.CreatedBy),
            ReceivedClaimsCount = batch.ReceivedClaimsCount,
            ReceivedTotalAmount = batch.ReceivedTotalAmount,
            ActualAmount = batch.ReceivedTotalAmount,
            ReceivingWayId = batch.ReceivingWayId,
            ReceivingWayName = batch.ReceivingWay != null ? (lang == "ar" ? batch.ReceivingWay.NameAr : batch.ReceivingWay.NameEn) : null,
            BatchDueDate = batch.BatchDueDate.ToString("yyyy-MM-dd"),
            UploadOnPortal = batch.UploadOnPortal ? "Yes" : "No",
            Reviewed = batch.Reviewed ? "Yes" : "No"
        };
    }
}

