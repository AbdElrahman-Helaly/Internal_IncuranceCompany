using MCIApi.Application.Claims.DTOs;
using MCIApi.Application.Claims.Interfaces;
using MCIApi.Application.Common;
using MCIApi.Application.Common.Interfaces;
using MCIApi.Domain.Abstractions;
using MCIApi.Domain.Entities;
using MCIApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace MCIApi.Infrastructure.Services
{
    public class ClaimService : IClaimService
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorage;
        private readonly UserManager<IdentityUser> _userManager;

        public ClaimService(AppDbContext context, IUnitOfWork unitOfWork, IFileStorageService fileStorage, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
            _userManager = userManager;
        }

        public async Task<ServiceResult<ClaimPagedResultDto>> GetAllAsync(int page, int limit, CancellationToken cancellationToken = default)
        {
            if (page <= 0) page = 1;
            if (limit <= 0) limit = 10;

            var query = _context.Claims
                .AsNoTracking()
                .Include(c => c.Batch)
                    .ThenInclude(b => b.Provider)
                .Include(c => c.Member)
                .Where(c => !c.IsDeleted)
                .OrderByDescending(c => c.Id);

            var totalClaims = await query.CountAsync(cancellationToken);
            var data = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(c => new ClaimListItemDto
                {
                    Id = c.Id,
                    BatchId = c.BatchId,
                    BatchReceiveDate = c.Batch != null ? c.Batch.ReceiveDate : default,
                    Amount = c.Amount,
                    CreatedBy = c.CreatedBy,
                    CreatedAt = c.CreatedAt,
                    InvoiceFile = c.InvoiceFile,
                    ClaimFormFile = c.ClaimFormFile,
                    ApprovalFile = c.ApprovalFile,
                    FirstSerial = c.FirstSerial,
                    LastSerial = c.LastSerial,
                    ProviderId = c.Batch != null ? c.Batch.ProviderId : 0,
                    ProviderName = c.Batch != null && c.Batch.Provider != null ? c.Batch.Provider.NameEn : null,
                    ServiceDate = c.ServiceDate,
                    MemberId = c.MemberId,
                    MemberName = c.Member != null ? c.Member.FullName : null,
                    NationalId = c.Member != null ? c.Member.NationalId : null,
                    ApprovalNo = c.ApprovalNo
                })
                .ToListAsync(cancellationToken);

            var dto = new ClaimPagedResultDto
            {
                TotalClaims = totalClaims,
                CurrentPage = page,
                Limit = limit,
                TotalPages = (int)Math.Ceiling(totalClaims / (double)limit),
                Data = data
            };

            return ServiceResult<ClaimPagedResultDto>.Ok(dto);
        }

        public async Task<ServiceResult<ClaimDetailDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var claim = await _context.Claims
                .AsNoTracking()
                .Include(c => c.Batch)
                    .ThenInclude(b => b.Provider)
                .Include(c => c.Member)
                .Include(c => c.Services)
                    .ThenInclude(s => s.ServiceClass)
                .Include(c => c.Diagnostics)
                    .ThenInclude(d => d.Diagnostic)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);

            if (claim == null)
                return ServiceResult<ClaimDetailDto>.Fail(ServiceErrorType.NotFound, "ClaimNotFound");

            // Get ReviewedByName from AspNetUsers
            string? reviewedByName = null;
            if (!string.IsNullOrEmpty(claim.ReviewedBy))
            {
                var reviewedByUser = await _userManager.FindByIdAsync(claim.ReviewedBy);
                reviewedByName = reviewedByUser?.UserName;
            }

            var dto = new ClaimDetailDto
            {
                Id = claim.Id,
                BatchId = claim.BatchId,
                BatchReceiveDate = claim.Batch != null ? claim.Batch.ReceiveDate : default,
                Amount = claim.Amount,
                CreatedBy = claim.CreatedBy,
                CreatedAt = claim.CreatedAt,
                InvoiceFile = claim.InvoiceFile,
                ClaimFormFile = claim.ClaimFormFile,
                ApprovalFile = claim.ApprovalFile,
                FirstSerial = claim.FirstSerial,
                LastSerial = claim.LastSerial,
                ProviderId = claim.Batch != null ? claim.Batch.ProviderId : 0,
                ProviderName = claim.Batch != null && claim.Batch.Provider != null ? claim.Batch.Provider.NameEn : null,
                ServiceDate = claim.ServiceDate,
                MemberId = claim.MemberId,
                MemberName = claim.Member != null ? claim.Member.FullName : null,
                NationalId = claim.Member != null ? claim.Member.NationalId : null,
                ApprovalNo = claim.ApprovalNo,
                InternalNote = claim.InternalNote,
                Services = claim.Services.Select(s =>
                {
                    var total = (s.Price * s.Qty) - (s.Price * s.Qty * s.Copayment / 100);
                    return new ClaimServiceClassReadDto
                    {
                        Id = s.Id,
                        ServiceClassId = s.ServiceClassId,
                        ServiceClassName = s.ServiceClass != null ? s.ServiceClass.NameEn : null,
                        CtoNameId = s.CtoNameId,
                        Price = s.Price,
                        Qty = s.Qty,
                        Copayment = s.Copayment,
                        StatusId = s.StatusId,
                        StatusName = s.StatusId.ToString(), // Can be localized later
                        ReasonId = s.ReasonId,
                        Total = total
                    };
                }).ToList(),
                DiagnosticIds = claim.Diagnostics.Select(d => d.DiagnosticId).ToList(),
                DiagnosticNames = claim.Diagnostics.Select(d => d.Diagnostic != null ? d.Diagnostic.NameEn : null).Where(n => n != null).ToList()!,
                Reviewed = claim.Reviewed,
                ReviewedBy = claim.ReviewedBy,
                ReviewedAt = claim.ReviewedAt,
                ReviewedByName = reviewedByName
            };

            return ServiceResult<ClaimDetailDto>.Ok(dto);
        }

        public async Task<ServiceResult<ClaimCreateResultDto>> CreateAsync(ClaimCreateDto dto, string userId, CancellationToken cancellationToken = default)
        {
            var batch = await _context.Batches.FirstOrDefaultAsync(b => b.Id == dto.BatchId && !b.IsDeleted, cancellationToken);
            if (batch == null)
                return ServiceResult<ClaimCreateResultDto>.Fail(ServiceErrorType.Validation, "InvalidBatchId");

            // Handle MemberId lookup from NationalId if MemberId is not provided
            int? memberId = dto.MemberId;
            if (!memberId.HasValue && !string.IsNullOrWhiteSpace(dto.NationalId))
            {
                var member = await _context.MemberInfos
                    .FirstOrDefaultAsync(m => m.NationalId == dto.NationalId && !m.IsDeleted, cancellationToken);
                if (member != null)
                    memberId = member.Id;
                else
                    return ServiceResult<ClaimCreateResultDto>.Fail(ServiceErrorType.Validation, "MemberNotFoundByNationalId");
            }

            var repo = _unitOfWork.Repository<Claim>();
            var claim = new Claim
            {
                BatchId = dto.BatchId,
                Amount = dto.Amount,
                CreatedBy = userId,
                CreatedAt = DateTime.Now,
                FirstSerial = dto.FirstSerial,
                LastSerial = dto.LastSerial,
                IsDeleted = false,
                ServiceDate = dto.ServiceDate,
                MemberId = memberId,
                ApprovalNo = dto.ApprovalNo?.Trim(),
                InternalNote = dto.InternalNote?.Trim()
            };

            claim.InvoiceFile = await _fileStorage.SaveAsync(dto.InvoiceFile, "claims", cancellationToken);
            claim.ClaimFormFile = await _fileStorage.SaveAsync(dto.ClaimFormFile, "claims", cancellationToken);
            claim.ApprovalFile = await _fileStorage.SaveAsync(dto.ApprovalFile, "claims", cancellationToken);

            // Validate Services BEFORE saving Claim
            if (dto.Services != null && dto.Services.Any())
            {
                // First, validate ServiceClassIds exist in ServiceClasses table
                var requestedServiceClassIds = dto.Services.Select(s => s.ServiceClassId).Distinct().ToList();
                var validServiceClassIds = await _context.ServiceClasses
                    .Where(sc => requestedServiceClassIds.Contains(sc.Id) && !sc.IsDeleted)
                    .Select(sc => sc.Id)
                    .ToListAsync(cancellationToken);

                var invalidServiceClassIds = requestedServiceClassIds
                    .Except(validServiceClassIds)
                    .ToList();

                if (invalidServiceClassIds.Any())
                    return ServiceResult<ClaimCreateResultDto>.Fail(ServiceErrorType.Validation, $"InvalidServiceClassIds: {string.Join(", ", invalidServiceClassIds)}. These ServiceClassIds do not exist in the database.");

                // Get Policy Services for the member (same logic as GetServicesByMemberNationalIdAsync)
                var memberForValidation = await _context.MemberInfos
                    .Include(m => m.Client)
                    .FirstOrDefaultAsync(m => !m.IsDeleted && 
                                              ((memberId.HasValue && m.Id == memberId.Value) || 
                                               (!string.IsNullOrWhiteSpace(dto.NationalId) && m.NationalId == dto.NationalId)), 
                                         cancellationToken);

                if (memberForValidation == null)
                    return ServiceResult<ClaimCreateResultDto>.Fail(ServiceErrorType.NotFound, "MemberNotFound");

                if (memberForValidation.ClientId == 0)
                    return ServiceResult<ClaimCreateResultDto>.Fail(ServiceErrorType.Validation, "MemberHasNoClient");

                // Get all Policies for this Client
                var policyIds = await _context.Policies
                    .Where(p => p.ClientId == memberForValidation.ClientId && !p.IsDeleted)
                    .Select(p => p.Id)
                    .ToListAsync(cancellationToken);

                if (!policyIds.Any())
                    return ServiceResult<ClaimCreateResultDto>.Fail(ServiceErrorType.NotFound, "NoPoliciesFound");

                // Get all ServiceClassDetails for these policies (through GeneralProgram)
                var serviceDetails = await _context.ServiceClassDetails
                    .Include(scd => scd.ServiceClass)
                    .Include(scd => scd.Program)
                    .Where(scd => scd.Program != null && policyIds.Contains(scd.Program.PolicyId))
                    .ToListAsync(cancellationToken);

                var availableServiceClassIds = serviceDetails
                    .Where(scd => scd.ServiceClass != null && !scd.ServiceClass.IsDeleted)
                    .Select(scd => scd.ServiceClass!.Id)
                    .Distinct()
                    .ToHashSet();

                // Validate that all requested ServiceClassIds exist in Policy Services
                var missingServiceClassIds = requestedServiceClassIds
                    .Where(id => !availableServiceClassIds.Contains(id))
                    .ToList();

                if (missingServiceClassIds.Any())
                    return ServiceResult<ClaimCreateResultDto>.Fail(ServiceErrorType.Validation, 
                        $"ServiceClassIdsNotInPolicy: {string.Join(", ", missingServiceClassIds)}. These services are not available in the member's policy.");
            }

            // Save Claim first to get the ID
            await repo.AddAsync(claim, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Add Services to Claim after saving (so we have ClaimId)
            if (dto.Services != null && dto.Services.Any())
            {
                foreach (var serviceDto in dto.Services)
                {
                    claim.Services.Add(new ClaimServiceClass
                    {
                        ClaimId = claim.Id,
                        ServiceClassId = serviceDto.ServiceClassId,
                        CtoNameId = serviceDto.CtoNameId,
                        Price = serviceDto.Price,
                        Qty = serviceDto.Qty,
                        Copayment = serviceDto.Copayment,
                        StatusId = serviceDto.StatusId,
                        ReasonId = serviceDto.ReasonId
                    });
                }
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            // Add Diagnostics
            if (dto.DiagnosticIds != null && dto.DiagnosticIds.Any())
            {
                var validDiagnosticIds = await _context.Diagnostics
                    .Where(d => dto.DiagnosticIds.Contains(d.Id) && !d.IsDeleted)
                    .Select(d => d.Id)
                    .ToListAsync(cancellationToken);

                var invalidDiagnosticIds = dto.DiagnosticIds.Except(validDiagnosticIds).ToList();
                if (invalidDiagnosticIds.Any())
                    return ServiceResult<ClaimCreateResultDto>.Fail(ServiceErrorType.Validation, $"InvalidDiagnosticIds: {string.Join(", ", invalidDiagnosticIds)}");

                foreach (var diagnosticId in dto.DiagnosticIds)
                {
                    claim.Diagnostics.Add(new ClaimDiagnostic
                    {
                        ClaimId = claim.Id,
                        DiagnosticId = diagnosticId
                    });
                }
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            await UpdateBatchTotalsAsync(claim.BatchId, cancellationToken);

            // New Claim starts as not reviewed
            claim.Reviewed = false;
            claim.ReviewedBy = null;
            claim.ReviewedAt = null;
            _context.Claims.Update(claim);
            await _context.SaveChangesAsync(cancellationToken);

            return ServiceResult<ClaimCreateResultDto>.Ok(new ClaimCreateResultDto { Id = claim.Id });
        }

        public async Task<ServiceResult<ClaimUpdateResultDto>> UpdateAsync(int id, ClaimUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var claim = await _context.Claims
                .Include(c => c.Services)
                .Include(c => c.Diagnostics)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
            if (claim == null)
                return ServiceResult<ClaimUpdateResultDto>.Fail(ServiceErrorType.NotFound, "ClaimNotFound");

            if (dto.Amount.HasValue)
                claim.Amount = dto.Amount.Value;

            if (dto.InvoiceFile != null)
                claim.InvoiceFile = await _fileStorage.SaveAsync(dto.InvoiceFile, "claims", cancellationToken);

            if (dto.ClaimFormFile != null)
                claim.ClaimFormFile = await _fileStorage.SaveAsync(dto.ClaimFormFile, "claims", cancellationToken);

            if (dto.ApprovalFile != null)
                claim.ApprovalFile = await _fileStorage.SaveAsync(dto.ApprovalFile, "claims", cancellationToken);

            if (dto.FirstSerial != null)
                claim.FirstSerial = dto.FirstSerial;

            if (dto.LastSerial != null)
                claim.LastSerial = dto.LastSerial;

            if (dto.IsDeleted.HasValue)
                claim.IsDeleted = dto.IsDeleted.Value;

            // New fields
            if (dto.ServiceDate.HasValue)
                claim.ServiceDate = dto.ServiceDate;

            if (dto.MemberId.HasValue)
                claim.MemberId = dto.MemberId;
            else if (!string.IsNullOrWhiteSpace(dto.NationalId))
            {
                var member = await _context.MemberInfos
                    .FirstOrDefaultAsync(m => m.NationalId == dto.NationalId && !m.IsDeleted, cancellationToken);
                if (member != null)
                    claim.MemberId = member.Id;
                else
                    return ServiceResult<ClaimUpdateResultDto>.Fail(ServiceErrorType.Validation, "MemberNotFoundByNationalId");
            }

            if (dto.ApprovalNo != null)
                claim.ApprovalNo = dto.ApprovalNo.Trim();

            if (dto.InternalNote != null)
                claim.InternalNote = dto.InternalNote.Trim();

            // Update Services - Update/Create pattern (like UPDATE POLICY)
            if (dto.Services != null && dto.Services.Any())
            {
                var existingServiceIds = claim.Services.Select(s => s.Id).ToHashSet();
                
                // Validate ServiceClassIds that are provided (for both update and create)
                var serviceClassIds = dto.Services
                    .Where(s => s.ServiceClassId.HasValue)
                    .Select(s => s.ServiceClassId!.Value)
                    .Distinct()
                    .ToList();
                
                if (serviceClassIds.Any())
                {
                    var validServiceClassIds = await _context.ServiceClasses
                        .Where(sc => serviceClassIds.Contains(sc.Id) && !sc.IsDeleted)
                        .Select(sc => sc.Id)
                        .ToListAsync(cancellationToken);

                    var invalidServiceClassIds = serviceClassIds.Except(validServiceClassIds).ToList();
                    if (invalidServiceClassIds.Any())
                        return ServiceResult<ClaimUpdateResultDto>.Fail(ServiceErrorType.Validation, $"InvalidServiceClassIds: {string.Join(", ", invalidServiceClassIds)}. These ServiceClassIds do not exist in the database.");

                    // Get Policy Services for the member (same logic as GetServicesByMemberNationalIdAsync)
                    // Get MemberId from claim (use updated value if provided in dto)
                    var memberIdForValidation = claim.MemberId;
                    if (dto.MemberId.HasValue)
                        memberIdForValidation = dto.MemberId.Value;
                    else if (!string.IsNullOrWhiteSpace(dto.NationalId))
                    {
                        var memberByNationalId = await _context.MemberInfos
                            .FirstOrDefaultAsync(m => m.NationalId == dto.NationalId && !m.IsDeleted, cancellationToken);
                        if (memberByNationalId != null)
                            memberIdForValidation = memberByNationalId.Id;
                    }

                    if (!memberIdForValidation.HasValue)
                        return ServiceResult<ClaimUpdateResultDto>.Fail(ServiceErrorType.Validation, "MemberId or NationalId is required to validate services");

                    var memberForValidation = await _context.MemberInfos
                        .Include(m => m.Client)
                        .FirstOrDefaultAsync(m => m.Id == memberIdForValidation.Value && !m.IsDeleted, cancellationToken);

                    if (memberForValidation == null)
                        return ServiceResult<ClaimUpdateResultDto>.Fail(ServiceErrorType.NotFound, "MemberNotFound");

                    if (memberForValidation.ClientId == 0)
                        return ServiceResult<ClaimUpdateResultDto>.Fail(ServiceErrorType.Validation, "MemberHasNoClient");

                    // Get all Policies for this Client
                    var policyIds = await _context.Policies
                        .Where(p => p.ClientId == memberForValidation.ClientId && !p.IsDeleted)
                        .Select(p => p.Id)
                        .ToListAsync(cancellationToken);

                    if (!policyIds.Any())
                        return ServiceResult<ClaimUpdateResultDto>.Fail(ServiceErrorType.NotFound, "NoPoliciesFound");

                    // Get all ServiceClassDetails for these policies (through GeneralProgram)
                    var serviceDetails = await _context.ServiceClassDetails
                        .Include(scd => scd.ServiceClass)
                        .Include(scd => scd.Program)
                        .Where(scd => scd.Program != null && policyIds.Contains(scd.Program.PolicyId))
                        .ToListAsync(cancellationToken);

                    var availableServiceClassIds = serviceDetails
                        .Where(scd => scd.ServiceClass != null && !scd.ServiceClass.IsDeleted)
                        .Select(scd => scd.ServiceClass!.Id)
                        .Distinct()
                        .ToHashSet();

                    // Validate that all requested ServiceClassIds exist in Policy Services
                    var missingServiceClassIds = serviceClassIds
                        .Where(id => !availableServiceClassIds.Contains(id))
                        .ToList();

                    if (missingServiceClassIds.Any())
                        return ServiceResult<ClaimUpdateResultDto>.Fail(ServiceErrorType.Validation, 
                            $"ServiceClassIdsNotInPolicy: {string.Join(", ", missingServiceClassIds)}. These services are not available in the member's policy.");
                }

                foreach (var serviceDto in dto.Services)
                {
                    if (serviceDto.Id.HasValue && existingServiceIds.Contains(serviceDto.Id.Value))
                    {
                        // Update existing service - only update fields that are provided
                        var service = claim.Services.First(s => s.Id == serviceDto.Id.Value);
                        
                        // Update only if value is provided
                        if (serviceDto.ServiceClassId.HasValue)
                            service.ServiceClassId = serviceDto.ServiceClassId.Value;
                        if (serviceDto.CtoNameId.HasValue)
                            service.CtoNameId = serviceDto.CtoNameId.Value;
                        if (serviceDto.Price.HasValue)
                            service.Price = serviceDto.Price.Value;
                        if (serviceDto.Qty.HasValue)
                            service.Qty = serviceDto.Qty.Value;
                        if (serviceDto.Copayment.HasValue)
                            service.Copayment = serviceDto.Copayment.Value;
                        if (serviceDto.StatusId.HasValue)
                            service.StatusId = serviceDto.StatusId.Value;
                        if (serviceDto.ReasonId.HasValue)
                            service.ReasonId = serviceDto.ReasonId.Value;
                    }
                    else
                    {
                        // Create new service - requires ServiceClassId, Price, Qty, and StatusId
                        if (!serviceDto.ServiceClassId.HasValue)
                            return ServiceResult<ClaimUpdateResultDto>.Fail(ServiceErrorType.Validation, "ServiceClassId is required when creating a new service");
                        if (!serviceDto.Price.HasValue)
                            return ServiceResult<ClaimUpdateResultDto>.Fail(ServiceErrorType.Validation, "Price is required when creating a new service");
                        if (!serviceDto.Qty.HasValue)
                            return ServiceResult<ClaimUpdateResultDto>.Fail(ServiceErrorType.Validation, "Qty is required when creating a new service");
                        if (!serviceDto.StatusId.HasValue)
                            return ServiceResult<ClaimUpdateResultDto>.Fail(ServiceErrorType.Validation, "StatusId is required when creating a new service");

                        claim.Services.Add(new ClaimServiceClass
                        {
                            ClaimId = claim.Id,
                            ServiceClassId = serviceDto.ServiceClassId.Value,
                            CtoNameId = serviceDto.CtoNameId,
                            Price = serviceDto.Price.Value,
                            Qty = serviceDto.Qty.Value,
                            Copayment = serviceDto.Copayment ?? 0,
                            StatusId = serviceDto.StatusId.Value,
                            ReasonId = serviceDto.ReasonId
                        });
                    }
                }
            }

            // Update Diagnostics - Add only new (like UPDATE POLICY)
            if (dto.DiagnosticIds != null && dto.DiagnosticIds.Any())
            {
                var existingDiagnosticIds = claim.Diagnostics.Select(d => d.DiagnosticId).ToHashSet();
                var newDiagnosticIds = dto.DiagnosticIds.Except(existingDiagnosticIds).ToList();

                if (newDiagnosticIds.Any())
                {
                    // Validate new DiagnosticIds
                    var validDiagnosticIds = await _context.Diagnostics
                        .Where(d => newDiagnosticIds.Contains(d.Id) && !d.IsDeleted)
                        .Select(d => d.Id)
                        .ToListAsync(cancellationToken);

                    var invalidDiagnosticIds = newDiagnosticIds.Except(validDiagnosticIds).ToList();
                    if (invalidDiagnosticIds.Any())
                        return ServiceResult<ClaimUpdateResultDto>.Fail(ServiceErrorType.Validation, $"InvalidDiagnosticIds: {string.Join(", ", invalidDiagnosticIds)}");

                    // Add only new diagnostics
                    foreach (var diagnosticId in validDiagnosticIds)
                    {
                        claim.Diagnostics.Add(new ClaimDiagnostic
                        {
                            ClaimId = claim.Id,
                            DiagnosticId = diagnosticId
                        });
                    }
                }
            }

            _context.Claims.Update(claim);
            await _context.SaveChangesAsync(cancellationToken);

            await UpdateBatchTotalsAsync(claim.BatchId, cancellationToken);

            return ServiceResult<ClaimUpdateResultDto>.Ok(new ClaimUpdateResultDto());
        }

        public async Task<ServiceResult<ClaimDeleteResultDto>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<Claim>();
            var claim = await repo.GetByIdAsync(id, cancellationToken);
            if (claim == null || claim.IsDeleted)
                return ServiceResult<ClaimDeleteResultDto>.Fail(ServiceErrorType.NotFound, "ClaimNotFound");

            claim.IsDeleted = true;
            repo.Update(claim);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await UpdateBatchTotalsAsync(claim.BatchId, cancellationToken);

            return ServiceResult<ClaimDeleteResultDto>.Ok(new ClaimDeleteResultDto());
        }

        public async Task<ServiceResult<ClaimByBatchPagedResultDto>> GetByBatchIdAsync(int batchId, int page, int limit, string? searchColumn, string? search, CancellationToken cancellationToken = default)
        {
            if (page <= 0) page = 1;
            if (limit <= 0) limit = 10;

            var query = _context.Claims
                .AsNoTracking()
                .Include(c => c.Member)
                .Include(c => c.Batch)
                .Where(c => c.BatchId == batchId && !c.IsDeleted);

            // Apply search if provided
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.Trim().ToLowerInvariant();
                query = ApplyClaimSearch(query, searchColumn, searchTerm);
            }

            var totalClaims = await query.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(totalClaims / (double)limit);

            if (totalClaims == 0)
            {
                return ServiceResult<ClaimByBatchPagedResultDto>.Ok(new ClaimByBatchPagedResultDto
                {
                    TotalClaims = 0,
                    CurrentPage = page,
                    Limit = limit,
                    TotalPages = 0,
                    Data = Array.Empty<ClaimByBatchListItemDto>()
                });
            }

            // Apply pagination at database level
            var pagedClaims = await query
                .OrderBy(c => c.Id)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync(cancellationToken);
            // 1. ??? ?? ??? UserIds ?????????
            var reviewerIds = pagedClaims
                .Where(c => c.ReviewedBy != null)
                .Select(c => c.ReviewedBy)
                .Distinct()
                .ToList();

            // 2. Query ????? ??? ??? Users
            var reviewers = await _context.Users
                .Where(u => reviewerIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.UserName, cancellationToken);

            // Calculate Serial based on page position
            var startSerial = (page - 1) * limit + 1;
            var list = pagedClaims
                .Select((c, index) => new ClaimByBatchListItemDto
                {
                    Serial = startSerial + index,
                    Id = c.Id,
                    MemberId = c.MemberId,
                    MemberName = c.Member != null ? c.Member.FullName : null,
                    ServiceDate = c.ServiceDate,
                    RequestedAmount = c.Amount,
                    TotalAmount = c.Amount,
                    GrandAmount = c.Amount,
                    IsReviewed = c.Reviewed,
                    Status = c.Reviewed ? "Reviewed" : "Need Review",
                    ReviewedBy = c.ReviewedBy != null && reviewers.TryGetValue(c.ReviewedBy, out var username) ? username : null,
                    ReviewedAt = c.ReviewedAt
                })
                .ToList()
                .AsReadOnly();

            var result = new ClaimByBatchPagedResultDto
            {
                TotalClaims = totalClaims,
                CurrentPage = page,
                Limit = limit,
                TotalPages = totalPages,
                Data = list
            };

            return ServiceResult<ClaimByBatchPagedResultDto>.Ok(result);
        }

        public async Task<ServiceResult<ClaimReviewResponseDto>> MarkAsReviewedAsync(int id, string userId, CancellationToken cancellationToken = default)
        {
            // Validate user exists in AspNetUsers
            string? actualUserId = null;
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                
                // If not found by ID, try to find by UserName (in case userId is actually a username)
                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(userId);
                }
                
                // If still not found, check if userId might be an Employee ID (numeric string)
                if (user == null && int.TryParse(userId, out var employeeId))
                {
                    var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId && !e.IsDeleted, cancellationToken);
                    if (employee != null && !string.IsNullOrEmpty(employee.IdentityUserId))
                    {
                        user = await _userManager.FindByIdAsync(employee.IdentityUserId);
                        if (user != null)
                        {
                            actualUserId = employee.IdentityUserId; // Update userId to the correct Identity User ID
                        }
                    }
                }
                else if (user != null)
                {
                    actualUserId = user.Id;
                }

                if (actualUserId == null)
                {
                    return ServiceResult<ClaimReviewResponseDto>.Fail(ServiceErrorType.Validation, $"InvalidUserId: User with ID/Name '{userId}' not found in AspNetUsers.");
                }
            }

            var repo = _unitOfWork.Repository<Claim>();
            var claim = await repo.GetByIdAsync(id, cancellationToken);
            if (claim == null || claim.IsDeleted)
                return ServiceResult<ClaimReviewResponseDto>.Fail(ServiceErrorType.NotFound, "ClaimNotFound");

            // Update Claim to Reviewed
            claim.Reviewed = true;
            claim.ReviewedBy = actualUserId;
            claim.ReviewedAt = DateTime.Now;

            repo.Update(claim);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Get reviewed by user name for response
            var reviewedByUser = await _userManager.FindByIdAsync(actualUserId ?? string.Empty);
            var reviewedByName = reviewedByUser?.UserName ?? actualUserId ?? string.Empty;

            var response = new ClaimReviewResponseDto
            {
                Message = "Claim marked as reviewed successfully",
                Id = claim.Id,
                Reviewed = claim.Reviewed,
                ReviewedBy = reviewedByName,
                ReviewedAt = claim.ReviewedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty
            };

            return ServiceResult<ClaimReviewResponseDto>.Ok(response);
        }

        private static IQueryable<Claim> ApplyClaimSearch(IQueryable<Claim> query, string? searchColumn, string searchTerm)
        {
            // If no searchColumn specified, perform wildcard search across all fields
            if (string.IsNullOrWhiteSpace(searchColumn))
            {
                return ApplyWildcardClaimSearch(query, searchTerm);
            }

            var column = searchColumn.Trim().ToLowerInvariant();
            var isNumeric = int.TryParse(searchTerm, out var numericValue);

            return column switch
            {
                "id" or "claimid" => ApplyClaimIdSearch(query, searchTerm, isNumeric, numericValue),
                "memberid" => ApplyMemberIdSearch(query, searchTerm, isNumeric, numericValue),
                "membername" or "name" or "fullname" => query.Where(c => c.Member != null && c.Member.FullName != null && c.Member.FullName.ToLower().Contains(searchTerm)),
                "nationalid" => query.Where(c => c.Member != null && c.Member.NationalId != null && c.Member.NationalId.ToLower().Contains(searchTerm)),
                "approvalno" or "approval_no" => query.Where(c => c.ApprovalNo != null && c.ApprovalNo.ToLower().Contains(searchTerm)),
                "status" or "reviewed" => ApplyStatusSearch(query, searchTerm),
                _ => ApplyWildcardClaimSearch(query, searchTerm) // Default to wildcard if column not recognized
            };
        }

        private static IQueryable<Claim> ApplyClaimIdSearch(IQueryable<Claim> query, string searchTerm, bool isNumeric, int numericValue)
        {
            if (isNumeric)
            {
                return query.Where(c => c.Id == numericValue);
            }
            // If not numeric, search as string
            return query.Where(c => c.Id.ToString().Contains(searchTerm));
        }

        private static IQueryable<Claim> ApplyMemberIdSearch(IQueryable<Claim> query, string searchTerm, bool isNumeric, int numericValue)
        {
            if (isNumeric)
            {
                return query.Where(c => c.MemberId.HasValue && c.MemberId.Value == numericValue);
            }
            // If not numeric, search as string
            return query.Where(c => c.MemberId.HasValue && c.MemberId.Value.ToString().Contains(searchTerm));
        }

        private static IQueryable<Claim> ApplyStatusSearch(IQueryable<Claim> query, string searchTerm)
        {
            if ("reviewed".Contains(searchTerm))
            {
                return query.Where(c => c.Reviewed);
            }
            if ("need review".Contains(searchTerm) || "needreview".Contains(searchTerm))
            {
                return query.Where(c => !c.Reviewed);
            }
            // Default: search in both
            return query.Where(c =>
                (c.Reviewed && "reviewed".Contains(searchTerm)) ||
                (!c.Reviewed && "need review".Contains(searchTerm))
            );
        }

        private static IQueryable<Claim> ApplyWildcardClaimSearch(IQueryable<Claim> query, string searchTerm)
        {
            var isNumeric = int.TryParse(searchTerm, out var numericValue);

            return query.Where(c =>
                // Search in Member Name
                (c.Member != null && c.Member.FullName != null && c.Member.FullName.ToLower().Contains(searchTerm)) ||
                // Search in NationalId
                (c.Member != null && c.Member.NationalId != null && c.Member.NationalId.ToLower().Contains(searchTerm)) ||
                // Search in ApprovalNo
                (c.ApprovalNo != null && c.ApprovalNo.ToLower().Contains(searchTerm)) ||
                // Search in MemberId (if search is numeric)
                (isNumeric && c.MemberId.HasValue && c.MemberId.Value == numericValue) ||
                // Search in Claim Id (if search is numeric)
                (isNumeric && c.Id == numericValue) ||
                // Search in Status
                (c.Reviewed && "reviewed".Contains(searchTerm)) ||
                (!c.Reviewed && "need review".Contains(searchTerm))
            );
        }

        public async Task<ServiceResult<byte[]>> ExportToExcelAsync(int batchId, string? searchColumn, string? search, CancellationToken cancellationToken = default)
        {
            var query = _context.Claims
                .AsNoTracking()
                .Include(c => c.Member)
                .Include(c => c.Batch)
                .Where(c => c.BatchId == batchId && !c.IsDeleted);

            // Apply search if provided
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.Trim().ToLowerInvariant();
                query = ApplyClaimSearch(query, searchColumn, searchTerm);
            }

            var claims = await query
                .OrderBy(c => c.Id)
                .ToListAsync(cancellationToken);

            // Set EPPlus license for non-commercial use
            try
            {
                ExcelPackage.License.SetNonCommercialPersonal("MCI API");
            }
            catch (InvalidOperationException)
            {
                // License already set, continue
            }
            catch (Exception)
            {
                // Handle other potential exceptions during license setting
            }

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Claims");

            // Set headers (English for now, can add Arabic support later)
            var headers = new[] 
            { 
                "Serial", 
                "Claim ID", 
                "Member ID", 
                "Member Name", 
                "National ID", 
                "Service Date", 
                "Requested Amount", 
                "Total Amount", 
                "Grand Amount", 
                "Status", 
                "Reviewed By", 
                "Reviewed At",
                "Approval No"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // Add data rows
            for (int i = 0; i < claims.Count; i++)
            {
                var claim = claims[i];
                var row = i + 2;

                worksheet.Cells[row, 1].Value = i + 1; // Serial
                worksheet.Cells[row, 2].Value = claim.Id;
                worksheet.Cells[row, 3].Value = claim.MemberId ?? (object)DBNull.Value;
                worksheet.Cells[row, 4].Value = claim.Member?.FullName ?? string.Empty;
                worksheet.Cells[row, 5].Value = claim.Member?.NationalId ?? string.Empty;
                worksheet.Cells[row, 6].Value = claim.ServiceDate?.ToString("yyyy-MM-dd") ?? string.Empty;
                worksheet.Cells[row, 7].Value = claim.Amount; // RequestedAmount
                worksheet.Cells[row, 8].Value = claim.Amount; // TotalAmount
                worksheet.Cells[row, 9].Value = claim.Amount; // GrandAmount
                worksheet.Cells[row, 10].Value = claim.Reviewed ? "Reviewed" : "Need Review";
                worksheet.Cells[row, 11].Value = claim.ReviewedBy ?? string.Empty;
                worksheet.Cells[row, 12].Value = claim.ReviewedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty;
                worksheet.Cells[row, 13].Value = claim.ApprovalNo ?? string.Empty;

                // Format numeric columns
                worksheet.Cells[row, 7].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[row, 8].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[row, 9].Style.Numberformat.Format = "#,##0.00";
            }

            // Auto-fit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var excelBytes = package.GetAsByteArray();
            return ServiceResult<byte[]>.Ok(excelBytes);
        }

        private async Task UpdateBatchTotalsAsync(int batchId, CancellationToken cancellationToken)
        {
            var batch = await _context.Batches.FirstOrDefaultAsync(b => b.Id == batchId, cancellationToken);
            if (batch == null)
                return;

            batch.ReceivedClaimsCount = await _context.Claims.CountAsync(c => c.BatchId == batchId && !c.IsDeleted, cancellationToken);
            batch.ReceivedTotalAmount = await _context.Claims
                .Where(c => c.BatchId == batchId && !c.IsDeleted)
                .SumAsync(c => c.Amount, cancellationToken);

            _context.Batches.Update(batch);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

