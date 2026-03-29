using MCIApi.Application.Approvals.DTOs;
using MCIApi.Application.Approvals.Interfaces;
using MCIApi.Application.Common;
using MCIApi.Application.Medicines.Helpers;
using MCIApi.Domain.Abstractions;
using MCIApi.Domain.Entities;
using MCIApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MCIApi.Infrastructure.Services
{
    public class ApprovalService : IApprovalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ApprovalService(IUnitOfWork unitOfWork, AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _userManager = userManager;
        }

        public async Task<ServiceResult<IReadOnlyList<ApprovalListDto>>> GetAllAsync(int page, int limit, string? searchTerm, string lang, CancellationToken cancellationToken = default)
        {
            var query = _context.Approvals
                .Include(a => a.Member)
                    .ThenInclude(m => m!.Client)
                .Include(a => a.Member)
                    .ThenInclude(m => m!.Program)
                .Include(a => a.Provider)
                .Include(a => a.ProviderLocation)
                .Include(a => a.AdditionalPool)
                .Include(a => a.Pool)
                .Include(a => a.Diagnostics)
                    .ThenInclude(ad => ad.Diagnostic)
                .Include(a => a.Services)
                    .ThenInclude(asc => asc.ServiceClass)
                .Include(a => a.Medicines)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.ToLower();
                query = query.Where(a =>
                    (a.Member != null && a.Member.NationalId != null && a.Member.NationalId.ToLower().Contains(search)) ||
                    (a.Member != null && a.Member.FullName != null && a.Member.FullName.ToLower().Contains(search)) ||
                    (a.Provider != null && a.Provider.NameEn != null && a.Provider.NameEn.ToLower().Contains(search)) ||
                    (a.Provider != null && a.Provider.NameAr != null && a.Provider.NameAr.ToLower().Contains(search)) ||
                    (a.ClaimFormNumber != null && a.ClaimFormNumber.ToLower().Contains(search)) ||
                    (a.Diagnostics.Any(ad => ad.Diagnostic != null && ad.Diagnostic.NameEn != null && ad.Diagnostic.NameEn.ToLower().Contains(search))) ||
                    (a.Diagnostics.Any(ad => ad.Diagnostic != null && ad.Diagnostic.NameAr != null && ad.Diagnostic.NameAr.ToLower().Contains(search)))
                );
            }

            var approvals = await query
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync(cancellationToken);

            // Batch lookup all unique UserIds
            var userIds = approvals
                .Select(a => a.CreatedBy)
                .Where(id => !string.IsNullOrEmpty(id))
                .Distinct()
                .ToList();

            var users = new Dictionary<string, string>();
            if (userIds.Any())
            {
                var userList = await _userManager.Users
                    .Where(u => userIds.Contains(u.Id))
                    .Select(u => new { u.Id, u.UserName, u.PhoneNumber })
                    .ToListAsync(cancellationToken);

                users = userList.ToDictionary(
                    u => u.Id,
                    u => u.UserName ?? u.PhoneNumber ?? u.Id
                );
            }

            var data = approvals.Select(a => MapToList(a, lang, users)).ToList().AsReadOnly();

            return ServiceResult<IReadOnlyList<ApprovalListDto>>.Ok(data);
        }

        public async Task<ServiceResult<IReadOnlyList<ApprovalListDto>>> GetAllMonthlyApprovalAsync(int page, int limit, string? searchTerm, string lang, CancellationToken cancellationToken = default)
        {
            var query = _context.Approvals
                .Include(a => a.Member)
                    .ThenInclude(m => m!.Client)
                .Include(a => a.Member)
                    .ThenInclude(m => m!.Program)
                .Include(a => a.Provider)
                .Include(a => a.ProviderLocation)
                .Include(a => a.AdditionalPool)
                .Include(a => a.Pool)
                .Include(a => a.Diagnostics)
                    .ThenInclude(ad => ad.Diagnostic)
                .Include(a => a.Services)
                    .ThenInclude(asc => asc.ServiceClass)
                .Include(a => a.Medicines)
                .Where(a => a.IsRepeated == true) // Filter for monthly/repeated approvals
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.ToLower();
                query = query.Where(a =>
                    (a.Member != null && a.Member.NationalId != null && a.Member.NationalId.ToLower().Contains(search)) ||
                    (a.Member != null && a.Member.FullName != null && a.Member.FullName.ToLower().Contains(search)) ||
                    (a.Provider != null && a.Provider.NameEn != null && a.Provider.NameEn.ToLower().Contains(search)) ||
                    (a.Provider != null && a.Provider.NameAr != null && a.Provider.NameAr.ToLower().Contains(search)) ||
                    (a.ClaimFormNumber != null && a.ClaimFormNumber.ToLower().Contains(search)) ||
                    (a.Diagnostics.Any(ad => ad.Diagnostic != null && ad.Diagnostic.NameEn != null && ad.Diagnostic.NameEn.ToLower().Contains(search))) ||
                    (a.Diagnostics.Any(ad => ad.Diagnostic != null && ad.Diagnostic.NameAr != null && ad.Diagnostic.NameAr.ToLower().Contains(search)))
                );
            }

            var approvals = await query
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync(cancellationToken);

            // Batch lookup all unique UserIds
            var userIds = approvals
                .Select(a => a.CreatedBy)
                .Where(id => !string.IsNullOrEmpty(id))
                .Distinct()
                .ToList();

            var users = new Dictionary<string, string>();
            if (userIds.Any())
            {
                var userList = await _userManager.Users
                    .Where(u => userIds.Contains(u.Id))
                    .Select(u => new { u.Id, u.UserName, u.PhoneNumber })
                    .ToListAsync(cancellationToken);

                users = userList.ToDictionary(
                    u => u.Id,
                    u => u.UserName ?? u.PhoneNumber ?? u.Id
                );
            }

            var data = approvals.Select(a => MapToList(a, lang, users)).ToList().AsReadOnly();

            return ServiceResult<IReadOnlyList<ApprovalListDto>>.Ok(data);
        }

        public async Task<ServiceResult<ApprovalReadDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default)
        {
            var approval = await _context.Approvals
                .Include(a => a.Member)
                    .ThenInclude(m => m!.Client)
                .Include(a => a.Member)
                    .ThenInclude(m => m!.Program)
                .Include(a => a.Member)
                    .ThenInclude(m => m!.MemberPolicies)
                        .ThenInclude(mp => mp.Policy)
                .Include(a => a.Member)
                    .ThenInclude(m => m!.MemberPolicies)
                        .ThenInclude(mp => mp.Program)
                            .ThenInclude(p => p.ProgramName)
                .Include(a => a.Provider)
                .Include(a => a.ProviderLocation)
                .Include(a => a.AdditionalPool)
                .Include(a => a.Pool)
                .Include(a => a.Comment)
                .Include(a => a.Diagnostics)
                    .ThenInclude(ad => ad.Diagnostic)
                .Include(a => a.Services)
                    .ThenInclude(asc => asc.ServiceClass)
                .Include(a => a.Medicines)
                    .ThenInclude(am => am.Medicine)
                .Include(a => a.Medicines)
                    .ThenInclude(am => am.Unit)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            if (approval == null)
                return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.NotFound, "ApprovalNotFound");

            return ServiceResult<ApprovalReadDto>.Ok(await MapAsync(approval, lang, cancellationToken));
        }

        public async Task<ServiceResult<ApprovalReadDto>> CreateAsync(ApprovalCreateDto dto, string createdBy, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<Approval>();

            // Validate MemberId exists if provided
            if (dto.MemberId.HasValue)
            {
                var memberExists = await _context.MemberInfos.AnyAsync(m => m.Id == dto.MemberId.Value && !m.IsDeleted, cancellationToken);
                if (!memberExists)
                    return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.Validation, "MemberNotFound");
            }

            // Validate ProviderId exists if provided
            if (dto.ProviderId.HasValue)
            {
                var providerExists = await _context.Providers.AnyAsync(p => p.Id == dto.ProviderId.Value && !p.IsDeleted, cancellationToken);
                if (!providerExists)
                    return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.Validation, "ProviderNotFound");
            }

            // Validate ProviderLocationId exists if provided
            if (dto.ProviderLocationId.HasValue)
            {
                var locationExists = await _context.ProviderLocations.AnyAsync(l => l.Id == dto.ProviderLocationId.Value && !l.IsDeleted, cancellationToken);
                if (!locationExists)
                    return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.Validation, "ProviderLocationNotFound");
            }

            // Validate AdditionalPoolId (maps to Pool entity) exists if provided
            if (dto.AdditionalPoolId.HasValue)
            {
                var poolExists = await _context.Pools.AnyAsync(p => p.Id == dto.AdditionalPoolId.Value, cancellationToken);
                if (!poolExists)
                    return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.Validation, "PoolNotFound");
            }

            // Validate CommentId exists if provided
            if (dto.CommentId.HasValue)
            {
                var commentExists = await _context.Comments.AnyAsync(c => c.Id == dto.CommentId.Value && !c.IsDeleted, cancellationToken);
                if (!commentExists)
                    return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.Validation, "CommentNotFound");
            }

            // Validate DiagnosticIds exist if provided
            if (dto.DiagnosticIds != null && dto.DiagnosticIds.Any())
            {
                var validDiagnostics = await _context.Diagnostics
                    .Where(d => dto.DiagnosticIds.Contains(d.Id) && !d.IsDeleted)
                    .Select(d => d.Id)
                    .ToListAsync(cancellationToken);

                var invalidIds = dto.DiagnosticIds.Except(validDiagnostics).ToList();
                if (invalidIds.Any())
                    return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.Validation, $"InvalidDiagnosticIds: {string.Join(", ", invalidIds)}");
            }

            // Validate ServiceIds exist if provided
            if (dto.ServiceIds != null && dto.ServiceIds.Any())
            {
                var validServices = await _context.ServiceClasses
                    .Where(s => dto.ServiceIds.Contains(s.Id) && !s.IsDeleted)
                    .Select(s => s.Id)
                    .ToListAsync(cancellationToken);

                var invalidIds = dto.ServiceIds.Except(validServices).ToList();
                if (invalidIds.Any())
                    return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.Validation, $"InvalidServiceIds: {string.Join(", ", invalidIds)}");
            }

            // Validate Medicines if provided
            if (dto.Medicines != null && dto.Medicines.Any())
            {
                var medicineIds = dto.Medicines.Select(m => m.MedicineId).Distinct().ToList();
                var validMedicines = await _context.Medicines
                    .Where(m => medicineIds.Contains(m.Id) && !m.IsDeleted)
                    .Select(m => m.Id)
                    .ToListAsync(cancellationToken);

                var invalidMedicineIds = medicineIds.Except(validMedicines).ToList();
                if (invalidMedicineIds.Any())
                    return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.Validation, $"InvalidMedicineIds: {string.Join(", ", invalidMedicineIds)}");

                var unitIds = dto.Medicines.Select(m => m.UnitId).Distinct().ToList();
                var validUnits = await _context.Units
                    .Where(u => unitIds.Contains(u.Id) && !u.IsDeleted)
                    .Select(u => u.Id)
                    .ToListAsync(cancellationToken);

                var invalidUnitIds = unitIds.Except(validUnits).ToList();
                if (invalidUnitIds.Any())
                    return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.Validation, $"InvalidUnitIds: {string.Join(", ", invalidUnitIds)}");
            }

            var entity = new Approval
            {
                MemberId = dto.MemberId,
                ProviderId = dto.ProviderId,
                ProviderLocationId = dto.ProviderLocationId,
                ReceiveTime = dto.ReceiveTime,
                ReceiveDate = dto.ReceiveDate,
                ClaimFormNumber = dto.ClaimFormNumber?.Trim(),
                PoolId = dto.AdditionalPoolId, // DTO's AdditionalPoolId maps to entity's PoolId
                ChronicForDate = dto.ChronicForDate,
                RequestEmailOrMobile = dto.RequestEmailOrMobile?.Trim(),
                CommentId = dto.CommentId,
                MaxAllowAmount = dto.MaxAllowAmount,
                InternalNote = dto.InternalNote?.Trim(),
                IsDebit = dto.IsDebit,
                IsRepeated = dto.IsRepeated,
                IsDelivery = dto.IsDelivery,
                InpatientDuration = dto.InpatientDuration,
                DurationType = dto.DurationType,
                IsApproved = false,
                IsDispensed = false,
                IsCanceled = false,
                IsFromProviderPortal = false,
                CreatedAt = DateTime.Now,
                CreatedBy = createdBy
            };

            await repo.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Add Diagnostics if provided
            if (dto.DiagnosticIds != null && dto.DiagnosticIds.Any())
            {
                foreach (var diagnosticId in dto.DiagnosticIds)
                {
                    entity.Diagnostics.Add(new ApprovalDiagnostic
                    {
                        ApprovalId = entity.Id,
                        DiagnosticId = diagnosticId
                    });
                }
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            // Add Services if provided
            if (dto.ServiceIds != null && dto.ServiceIds.Any())
            {
                foreach (var serviceId in dto.ServiceIds)
                {
                    entity.Services.Add(new ApprovalServiceClass
                    {
                        ApprovalId = entity.Id,
                        ServiceClassId = serviceId
                    });
                }
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            // Add Medicines if provided
            if (dto.Medicines != null && dto.Medicines.Any())
            {
                foreach (var medicineDto in dto.Medicines)
                {
                    entity.Medicines.Add(new ApprovalMedicine
                    {
                        ApprovalId = entity.Id,
                        MedicineId = medicineDto.MedicineId,
                        UnitId = medicineDto.UnitId,
                        Price = medicineDto.Price,
                        Qty = medicineDto.Qty,
                        StatusId = medicineDto.StatusId,
                        ReasonId = medicineDto.ReasonId
                    });
                }
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            // Reload entity with related data
            var loadedEntity = await _context.Approvals
                .Include(a => a.Member)
                .Include(a => a.Provider)
                .Include(a => a.ProviderLocation)
                .Include(a => a.AdditionalPool)
                .Include(a => a.Pool)
                .Include(a => a.Comment)
                .Include(a => a.Diagnostics)
                    .ThenInclude(ad => ad.Diagnostic)
                .Include(a => a.Services)
                    .ThenInclude(asc => asc.ServiceClass)
                .Include(a => a.Medicines)
                    .ThenInclude(am => am.Medicine)
                .Include(a => a.Medicines)
                    .ThenInclude(am => am.Unit)
                .FirstOrDefaultAsync(a => a.Id == entity.Id, cancellationToken);

            return ServiceResult<ApprovalReadDto>.Ok(await MapAsync(loadedEntity ?? entity, lang, cancellationToken));
        }

        public async Task<ServiceResult<ApprovalReadDto>> UpdateAsync(int id, ApprovalUpdateDto dto, string updatedBy, string lang, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Approvals
                .Include(a => a.Member)
                .Include(a => a.Provider)
                .Include(a => a.ProviderLocation)
                .Include(a => a.AdditionalPool)
                .Include(a => a.Diagnostics)
                .Include(a => a.Services)
                .Include(a => a.Medicines)
                .Include(a => a.Pool)
                .Include(a => a.Comment)
                .Include(a => a.Diagnostics)
                    .ThenInclude(ad => ad.Diagnostic)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            if (entity == null)
                return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.NotFound, "ApprovalNotFound");

            // Update fields if provided
            if (dto.ProviderId.HasValue)
            {
                var providerExists = await _context.Providers.AnyAsync(p => p.Id == dto.ProviderId.Value && !p.IsDeleted, cancellationToken);
                if (!providerExists)
                    return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.Validation, "ProviderNotFound");
                entity.ProviderId = dto.ProviderId.Value;
            }

            if (dto.ProviderLocationId.HasValue)
            {
                var locationExists = await _context.ProviderLocations.AnyAsync(l => l.Id == dto.ProviderLocationId.Value && !l.IsDeleted, cancellationToken);
                if (!locationExists)
                    return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.Validation, "ProviderLocationNotFound");
                entity.ProviderLocationId = dto.ProviderLocationId.Value;
            }

            if (dto.AdditionalPoolId.HasValue)
            {
                var poolExists = await _context.Pools.AnyAsync(p => p.Id == dto.AdditionalPoolId.Value, cancellationToken);
                if (!poolExists)
                    return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.Validation, "PoolNotFound");
                entity.PoolId = dto.AdditionalPoolId.Value; // DTO's AdditionalPoolId maps to entity's PoolId
            }

            if (dto.CommentId.HasValue)
            {
                var commentExists = await _context.Comments.AnyAsync(c => c.Id == dto.CommentId.Value && !c.IsDeleted, cancellationToken);
                if (!commentExists)
                    return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.Validation, "CommentNotFound");
                entity.CommentId = dto.CommentId.Value;
            }

            if (dto.MaxAllowAmount.HasValue)
                entity.MaxAllowAmount = dto.MaxAllowAmount.Value;

            if (!string.IsNullOrWhiteSpace(dto.InternalNote))
                entity.InternalNote = dto.InternalNote.Trim();

            if (dto.InpatientDuration.HasValue)
                entity.InpatientDuration = dto.InpatientDuration.Value;

            if (dto.DurationType.HasValue)
                entity.DurationType = dto.DurationType.Value;

            // Update Diagnostics if provided
            if (dto.DiagnosticIds != null && dto.DiagnosticIds.Any())
            {
                // Remove existing diagnostics
                var existingDiagnostics = entity.Diagnostics.ToList();
                foreach (var existing in existingDiagnostics)
                {
                    _context.ApprovalDiagnostics.Remove(existing);
                }

                // Validate and add new diagnostics
                var validDiagnostics = await _context.Diagnostics
                    .Where(d => dto.DiagnosticIds.Contains(d.Id) && !d.IsDeleted)
                    .Select(d => d.Id)
                    .ToListAsync(cancellationToken);

                var invalidIds = dto.DiagnosticIds.Except(validDiagnostics).ToList();
                if (invalidIds.Any())
                    return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.Validation, $"InvalidDiagnosticIds: {string.Join(", ", invalidIds)}");

                foreach (var diagnosticId in dto.DiagnosticIds)
                {
                    entity.Diagnostics.Add(new ApprovalDiagnostic
                    {
                        ApprovalId = entity.Id,
                        DiagnosticId = diagnosticId
                    });
                }
            }

            // Handle ServiceIds update
            if (dto.ServiceIds != null)
            {
                // Remove existing services
                var existingServices = entity.Services.ToList();
                foreach (var existing in existingServices)
                {
                    _context.ApprovalServiceClasses.Remove(existing);
                }

                // Validate and add new services
                if (dto.ServiceIds.Any())
                {
                    var validServices = await _context.ServiceClasses
                        .Where(s => dto.ServiceIds.Contains(s.Id) && !s.IsDeleted)
                        .Select(s => s.Id)
                        .ToListAsync(cancellationToken);

                    var invalidIds = dto.ServiceIds.Except(validServices).ToList();
                    if (invalidIds.Any())
                        return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.Validation, $"InvalidServiceIds: {string.Join(", ", invalidIds)}");

                    foreach (var serviceId in dto.ServiceIds)
                    {
                        entity.Services.Add(new ApprovalServiceClass
                        {
                            ApprovalId = entity.Id,
                            ServiceClassId = serviceId
                        });
                    }
                }
            }

            // Handle Medicines update
            if (dto.Medicines != null)
            {
                // Remove existing medicines
                var existingMedicines = entity.Medicines.ToList();
                foreach (var existing in existingMedicines)
                {
                    _context.ApprovalMedicines.Remove(existing);
                }

                // Validate and add new medicines
                if (dto.Medicines.Any())
                {
                    var medicineIds = dto.Medicines.Select(m => m.MedicineId).Distinct().ToList();
                    var validMedicines = await _context.Medicines
                        .Where(m => medicineIds.Contains(m.Id) && !m.IsDeleted)
                        .Select(m => m.Id)
                        .ToListAsync(cancellationToken);

                    var invalidMedicineIds = medicineIds.Except(validMedicines).ToList();
                    if (invalidMedicineIds.Any())
                        return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.Validation, $"InvalidMedicineIds: {string.Join(", ", invalidMedicineIds)}");

                    var unitIds = dto.Medicines.Select(m => m.UnitId).Distinct().ToList();
                    var validUnits = await _context.Units
                        .Where(u => unitIds.Contains(u.Id) && !u.IsDeleted)
                        .Select(u => u.Id)
                        .ToListAsync(cancellationToken);

                    var invalidUnitIds = unitIds.Except(validUnits).ToList();
                    if (invalidUnitIds.Any())
                        return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.Validation, $"InvalidUnitIds: {string.Join(", ", invalidUnitIds)}");

                    foreach (var medicineDto in dto.Medicines)
                    {
                        entity.Medicines.Add(new ApprovalMedicine
                        {
                            ApprovalId = entity.Id,
                            MedicineId = medicineDto.MedicineId,
                            UnitId = medicineDto.UnitId,
                            Price = medicineDto.Price,
                            Qty = medicineDto.Qty,
                            StatusId = medicineDto.StatusId,
                            ReasonId = medicineDto.ReasonId
                        });
                    }
                }
            }

            if (dto.IsApproved.HasValue)
                entity.IsApproved = dto.IsApproved.Value;

            if (dto.IsDispensed.HasValue)
                entity.IsDispensed = dto.IsDispensed.Value;

            if (dto.IsCanceled.HasValue)
                entity.IsCanceled = dto.IsCanceled.Value;

            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.Now;

            _context.Approvals.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            // Reload with related data
            var updatedEntity = await _context.Approvals
                .Include(a => a.Member)
                .Include(a => a.Provider)
                .Include(a => a.ProviderLocation)
                .Include(a => a.AdditionalPool)
                .Include(a => a.Pool)
                .Include(a => a.Diagnostics)
                    .ThenInclude(ad => ad.Diagnostic)
                .Include(a => a.Services)
                    .ThenInclude(asc => asc.ServiceClass)
                .Include(a => a.Medicines)
                    .ThenInclude(am => am.Medicine)
                .Include(a => a.Medicines)
                    .ThenInclude(am => am.Unit)
                .Include(a => a.Comment)
                .FirstOrDefaultAsync(a => a.Id == entity.Id, cancellationToken);

            return ServiceResult<ApprovalReadDto>.Ok(await MapAsync(updatedEntity ?? entity, lang, cancellationToken));
        }

        public async Task<ServiceResult> DeleteAsync(int id, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<Approval>();
            var entity = await repo.GetByIdAsync(id, cancellationToken);

            if (entity == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "ApprovalNotFound");

            repo.Delete(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult<IReadOnlyList<ServiceListDto>>> GetServicesByMemberNationalIdAsync(string nationalId, string lang, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(nationalId))
                return ServiceResult<IReadOnlyList<ServiceListDto>>.Fail(ServiceErrorType.Validation, "NationalIdRequired");

            // Step 1: Find MemberInfo by NationalId
            var member = await _context.MemberInfos
                .Include(m => m.Client)
                .FirstOrDefaultAsync(m => m.NationalId == nationalId && !m.IsDeleted, cancellationToken);

            if (member == null)
                return ServiceResult<IReadOnlyList<ServiceListDto>>.Fail(ServiceErrorType.NotFound, "MemberNotFound");

            // Step 2: Get ClientId from MemberInfo
            if (member.ClientId == 0)
                return ServiceResult<IReadOnlyList<ServiceListDto>>.Fail(ServiceErrorType.Validation, "MemberHasNoClient");

            // Step 3: Get Client (already included)
            var client = member.Client;
            if (client == null)
                return ServiceResult<IReadOnlyList<ServiceListDto>>.Fail(ServiceErrorType.NotFound, "ClientNotFound");

            // Step 4: Get ALL Policies for this Client (not just active policy)
            var policyIds = await _context.Policies
                .Where(p => p.ClientId == client.Id && !p.IsDeleted)
                .Select(p => p.Id)
                .ToListAsync(cancellationToken);

            if (!policyIds.Any())
                return ServiceResult<IReadOnlyList<ServiceListDto>>.Fail(ServiceErrorType.NotFound, "NoPoliciesFound");

            // Step 5: Get ALL ServiceClassDetails for these policies (through GeneralProgram)
            var serviceDetails = await _context.ServiceClassDetails
                .Include(scd => scd.ServiceClass)
                .Include(scd => scd.Program)
                .Where(scd => scd.Program != null && policyIds.Contains(scd.Program.PolicyId))
                .ToListAsync(cancellationToken);

            var services = serviceDetails
                .Where(scd => scd.ServiceClass != null && !scd.ServiceClass.IsDeleted)
                .Select(scd => scd.ServiceClass!)
                .GroupBy(s => s.Id)
                .Select(g => g.First())
                .ToList();

            if (!services.Any())
                return ServiceResult<IReadOnlyList<ServiceListDto>>.Fail(ServiceErrorType.NotFound, "NoServicesFound");

            // Step 6: Map to DTO
            var serviceDtos = services.Select(s => new ServiceListDto
            {
                Id = s.Id,
                NameEn = s.NameEn,
                NameAr = s.NameAr
            }).ToList();

            return ServiceResult<IReadOnlyList<ServiceListDto>>.Ok(serviceDtos);
        }

        public async Task<ServiceResult<IReadOnlyList<PoolList>>> GetPoolsByMemberNationalIdAsync(string nationalId, string lang, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(nationalId))
                return ServiceResult<IReadOnlyList<PoolList>>.Fail(ServiceErrorType.Validation, "NationalIdRequired");

            // Step 1: Find MemberInfo by NationalId
            var member = await _context.MemberInfos
                .Include(m => m.Client)
                .FirstOrDefaultAsync(m => m.NationalId == nationalId && !m.IsDeleted, cancellationToken);

            if (member == null)
                return ServiceResult<IReadOnlyList<PoolList>>.Fail(ServiceErrorType.NotFound, "MemberNotFound");

            // Step 2: Get ClientId from MemberInfo
            if (member.ClientId == 0)
                return ServiceResult<IReadOnlyList<PoolList>>.Fail(ServiceErrorType.Validation, "MemberHasNoClient");

            // Step 3: Get Client (already included)
            var client = member.Client;
            if (client == null)
                return ServiceResult<IReadOnlyList<PoolList>>.Fail(ServiceErrorType.NotFound, "ClientNotFound");

            // Step 4: Get ALL Policies for this Client (not just active policy)
            var allPolicies = await _context.Policies
                .Include(p => p.Pools)
                    .ThenInclude(pool => pool.PoolType)
                .Where(p => p.ClientId == client.Id && !p.IsDeleted)
                .ToListAsync(cancellationToken);

            if (!allPolicies.Any())
                return ServiceResult<IReadOnlyList<PoolList>>.Fail(ServiceErrorType.NotFound, "NoPoliciesFound");

            // Step 5: Get ALL Pools from ALL Policies
            var allPools = allPolicies
                .SelectMany(p => p.Pools ?? new List<Pool>())
                .ToList();

            if (!allPools.Any())
                return ServiceResult<IReadOnlyList<PoolList>>.Fail(ServiceErrorType.NotFound, "NoPoolsFound");

            // Step 6: Map to DTO
            var poolDtos = allPools.Select(pool => new PoolList
            {
                Id = pool.Id,
                PoolTypeId = pool.PoolTypeId,
                PoolTypeNameEn = pool.PoolType?.NameEn,
                PoolTypeNameAr = pool.PoolType?.NameAr
            }).ToList();

            return ServiceResult<IReadOnlyList<PoolList>>.Ok(poolDtos);
        }

        private async Task<ApprovalReadDto> MapAsync(Approval entity, string lang = "en", CancellationToken cancellationToken = default)
        {
            var isArabic = lang == "ar";
            
            // Get diagnostic names
            var diagnosticNames = entity.Diagnostics
                .Where(ad => ad.Diagnostic != null && !ad.Diagnostic.IsDeleted)
                .Select(ad => isArabic ? ad.Diagnostic!.NameAr : ad.Diagnostic!.NameEn)
                .ToList();

            // Get service names
            var serviceNames = entity.Services
                .Where(asc => asc.ServiceClass != null && !asc.ServiceClass.IsDeleted)
                .Select(asc => isArabic ? asc.ServiceClass!.NameAr : asc.ServiceClass!.NameEn)
                .ToList();

            // Map member information
            MemberInformationDto? memberInfo = null;
            if (entity.Member != null)
            {
                // Get the latest active MemberPolicyInfo
                var latestPolicyInfo = entity.Member.MemberPolicies
                    .Where(mp => !mp.IsDeleted && !mp.IsExpired)
                    .OrderByDescending(mp => mp.AddDate)
                    .FirstOrDefault();

                // Calculate total expenses from approved approvals
                decimal totalExpenses = 0;
                if (entity.MemberId.HasValue)
                {
                    totalExpenses = await _context.Approvals
                        .Where(a => a.MemberId == entity.MemberId && a.IsApproved && a.MaxAllowAmount.HasValue)
                        .SumAsync(a => a.MaxAllowAmount ?? 0, cancellationToken);
                }

                // Get coverage from policy
                decimal? coverage = latestPolicyInfo?.Policy?.TotalAmount;

                // Calculate remaining
                decimal? remaining = coverage.HasValue ? coverage.Value - totalExpenses : null;

                memberInfo = new MemberInformationDto
                {
                    MemberId = entity.Member.Id,
                    MemberName = entity.Member.FullName,
                    JobTitle = entity.Member.JobTitle,
                    MobileNumber = entity.Member.MobileNumber,
                    Gender = entity.Member.IsMale.HasValue 
                        ? (entity.Member.IsMale.Value ? "Male" : "Female") 
                        : null,
                    BirthDate = entity.Member.BirthDate,
                    CompanyName = entity.Member.Client != null
                        ? (isArabic ? entity.Member.Client.ArabicName : entity.Member.Client.EnglishName)
                        : null,
                    CompanyCode = latestPolicyInfo?.CodeAtCompany,
                    CardNumber = latestPolicyInfo?.Id.ToString(), // Using MemberPolicyInfo.Id as CardNumber
                    Note = entity.Member.Notes,
                    VipStatus = entity.Member.VipStatus,
                    ProgramName = latestPolicyInfo?.Program?.ProgramName != null
                        ? (isArabic ? latestPolicyInfo.Program.ProgramName.NameAr : latestPolicyInfo.Program.ProgramName.NameEn)
                        : (entity.Member.Program != null
                            ? (isArabic ? entity.Member.Program.NameAr : entity.Member.Program.NameEn)
                            : null),
                    AddDate = latestPolicyInfo?.AddDate ?? entity.Member.ActivatedDate,
                    MemberImage = entity.Member.MemberImage,
                    Coverage = coverage,
                    TotalApprovals = latestPolicyInfo?.TotalApprovals ?? 0,
                    TotalExpenses = totalExpenses,
                    Remaining = remaining,
                    TotalClaims = latestPolicyInfo?.TotalClaims ?? 0,
                    DebitSpent = null, // TODO: Calculate from debit approvals if needed
                    ExceedPoolSpent = null, // TODO: Calculate if needed
                    ExceedPoolLimit = null // TODO: Calculate if needed
                };
            }

            // Map Medicines (for Chronic Approvals)
            var medicines = entity.IsChronic
                ? entity.Medicines
                    .Where(m => m.Medicine != null && !m.Medicine.IsDeleted)
                    .Select(m =>
                    {
                        var total = (m.Price * m.Qty) - ((m.Price * m.Qty) * m.CP / 100);
                        return new ApprovalMedicineReadDto
                        {
                            Id = m.Id,
                            ServiceId = m.ServiceId,
                            MedicineId = m.MedicineId,
                            MedicineName = m.Medicine != null 
                                ? (isArabic ? m.Medicine.ArName : m.Medicine.EnName) 
                                : null,
                            UnitId = m.UnitId,
                            UnitName = m.Unit != null && !m.Unit.IsDeleted
                                ? (isArabic ? m.Unit.NameAr : m.Unit.NameEn) 
                                : null,
                            Days = m.Days,
                            Price = m.Price,
                            Qty = m.Qty,
                            CP = m.CP,
                            StatusId = m.StatusId,
                            StatusName = m.StatusId.ToString(),
                            ReasonId = m.ReasonId,
                            IsDebit = m.IsDebit,
                            Total = total
                        };
                    }).ToList()
                : new List<ApprovalMedicineReadDto>();

            // Map Services (for Regular Approvals)
            var services = !entity.IsChronic
                ? entity.Services
                    .Where(s => s.ServiceClass != null && !s.ServiceClass.IsDeleted)
                    .Select(s =>
                    {
                        var total = (s.Price * s.Qty) - ((s.Price * s.Qty) * s.Copayment / 100);
                        return new ApprovalServiceReadDto
                        {
                            Id = s.Id,
                            ServiceId = s.ServiceClassId,
                            ServiceName = s.ServiceClass != null
                                ? (isArabic ? s.ServiceClass.NameAr : s.ServiceClass.NameEn)
                                : null,
                            CtoNameId = s.CtoNameId,
                            Price = s.Price,
                            Qty = s.Qty,
                            Copayment = s.Copayment,
                            StatusId = s.StatusId,
                            StatusName = s.StatusId.ToString(),
                            ReasonId = s.ReasonId,
                            Total = total
                        };
                    }).ToList()
                : new List<ApprovalServiceReadDto>();

            return new ApprovalReadDto
            {
                Id = entity.Id,
                MemberIdentifier = entity.Member?.NationalId ?? entity.Member?.FullName ?? string.Empty,
                MemberPolicyInfoId = entity.MemberId,
                ProviderId = entity.ProviderId,
                ProviderName = entity.Provider != null && !entity.Provider.IsDeleted
                    ? (isArabic ? entity.Provider.NameAr : entity.Provider.NameEn) 
                    : null,
                ProviderBranch = entity.ProviderLocation != null
                    ? (isArabic ? entity.ProviderLocation.AreaNameAr : entity.ProviderLocation.AreaNameEn) 
                    : null,
                ReceiveDate = entity.ReceiveDate,
                ReceiveTime = entity.ReceiveTime,
                ClaimFormNumber = entity.ClaimFormNumber,
                AdditionalPool = entity.AdditionalPool != null
                    ? (isArabic ? entity.AdditionalPool.NameAr : entity.AdditionalPool.NameEn) 
                    : null,
                ChronicForDate = entity.ChronicForDate,
                Diagnosis = diagnosticNames.Any() ? string.Join(", ", diagnosticNames) : null,
                EmailOrPhone = entity.RequestEmailOrMobile,
                Comment = entity.Comment != null
                    ? (isArabic ? entity.Comment.TextAr : entity.Comment.TextEn) 
                    : null,
                MaxAllowedAmount = entity.MaxAllowAmount,
                InternalNote = entity.InternalNote,
                IsDebit = entity.IsDebit,
                IsRepeated = entity.IsRepeated,
                IsDelivery = entity.IsDelivery,
                IsApproved = entity.IsApproved,
                IsDispensed = entity.IsDispensed,
                IsCanceled = entity.IsCanceled,
                IsFromProviderPortal = entity.IsFromProviderPortal,
                InpatientDuration = entity.InpatientDuration,
                DurationType = entity.DurationType,
                IsChronic = entity.IsChronic,
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy,
                UpdatedAt = entity.UpdatedAt,
                UpdatedBy = entity.UpdatedBy,
                MemberInformation = memberInfo,
                Medicines = medicines,
                Services = services
            };
        }

        private ApprovalReadDto Map(Approval entity, string lang = "en")
        {
            // Synchronous version for backward compatibility - calls async version
            return MapAsync(entity, lang).GetAwaiter().GetResult();
        }

        private ApprovalListDto MapToList(Approval entity, string lang, Dictionary<string, string> users)
        {
            var isArabic = lang == "ar";
            
            // Get first service if available
            var firstService = entity.Services.FirstOrDefault();
            
            // Calculate total:
            // For medicines: (Price * Qty) - ((Price * Qty) * CP / 100)
            // For services: (Price * Qty) - ((Price * Qty) * Copayment / 100)
            var medicinesTotal = entity.Medicines
                .Sum(m => (m.Price * m.Qty) - ((m.Price * m.Qty) * m.CP / 100));
            var servicesTotal = entity.Services
                .Sum(s => (s.Price * s.Qty) - ((s.Price * s.Qty) * s.Copayment / 100));
            var total = medicinesTotal + servicesTotal;
            
            // Calculate Status based on medicines
            var status = CalculateApprovalStatus(entity.Medicines);
            
            // Calculate SLA in minutes
            int? slaInMinutes = null;
            if (entity.ReceiveDate.HasValue && entity.ReceiveTime.HasValue)
            {
                var receiveDateTime = entity.ReceiveDate.Value.Date.Add(entity.ReceiveTime.Value);
                var timeDifference = receiveDateTime - entity.CreatedAt;
                slaInMinutes = (int)timeDifference.TotalMinutes;
            }
            
            // Get UserName from dictionary
            var userName = users.GetValueOrDefault(entity.CreatedBy, entity.CreatedBy);
            
            return new ApprovalListDto
            {
                Id = entity.Id,
                MemberName = entity.Member?.FullName,
                MemberId = entity.MemberId,
                ProgramMemberName = entity.Member?.Program != null
                    ? (isArabic ? entity.Member.Program.NameAr : entity.Member.Program.NameEn)
                    : null,
                ProviderName = entity.Provider != null 
                    ? (isArabic ? entity.Provider.NameAr : entity.Provider.NameEn) 
                    : null,
                ProviderId = entity.ProviderId,
                ClientId = entity.Member?.ClientId,
                ClientName = entity.Member?.Client != null
                    ? (isArabic ? entity.Member.Client.ArabicName : entity.Member.Client.EnglishName)
                    : null,
                ServiceId = firstService?.ServiceClassId,
                ServiceName = firstService?.ServiceClass != null
                    ? (isArabic ? firstService.ServiceClass.NameAr : firstService.ServiceClass.NameEn)
                    : null,
                ReceiveDate = entity.ReceiveDate,
                Total = total,
                ShowOnPortalDate = entity.ShowOnPortalDate,
                PortalUser = entity.PortalUser,
                ApprovalSource = entity.ApprovalSource,
                Status = status,
                SLAInMinutes = slaInMinutes,
                UserName = userName,
                InternalNote = entity.InternalNote,
                CreatedAt = entity.CreatedAt
            };
        }

        private ApprovalStatus CalculateApprovalStatus(ICollection<ApprovalMedicine> medicines)
        {
            if (medicines == null || !medicines.Any())
                return ApprovalStatus.Received;
            
            var statuses = medicines.Select(m => m.StatusId).ToList();
            var allApproved = statuses.All(s => s == ApprovalStatusId.APPROVED);
            var allRejected = statuses.All(s => s == ApprovalStatusId.REJECTED);
            var hasApproved = statuses.Any(s => s == ApprovalStatusId.APPROVED);
            var hasRejected = statuses.Any(s => s == ApprovalStatusId.REJECTED);
            
            if (allRejected)
                return ApprovalStatus.Rejected;
            
            if (allApproved && !hasRejected)
                return ApprovalStatus.Approved;
            
            if (hasApproved && hasRejected)
                return ApprovalStatus.PartiallyApproved;
            
            return ApprovalStatus.Received;
        }

        public async Task<ServiceResult<IReadOnlyList<DiagnosticLookupDto>>> GetAllDiagnosticsAsync(string lang, CancellationToken cancellationToken = default)
        {
            var isArabic = lang == "ar";
            var diagnostics = await _context.Diagnostics
                .AsNoTracking()
                .Where(d => !d.IsDeleted)
                .OrderBy(d => d.Id)
                .Select(d => new DiagnosticLookupDto
                {
                    Id = d.Id,
                    Code = d.Code,
                    Name = isArabic ? d.NameAr : d.NameEn
                })
                .ToListAsync(cancellationToken);

            return ServiceResult<IReadOnlyList<DiagnosticLookupDto>>.Ok(diagnostics);
        }

        public async Task<ServiceResult<IReadOnlyList<CommentLookupDto>>> GetAllCommentsAsync(string lang, CancellationToken cancellationToken = default)
        {
            var isArabic = lang == "ar";
            var comments = await _context.Comments
                .AsNoTracking()
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Id)
                .Select(c => new CommentLookupDto
                {
                    Id = c.Id,
                    Name = isArabic ? c.TextAr : c.TextEn
                })
                .ToListAsync(cancellationToken);

            return ServiceResult<IReadOnlyList<CommentLookupDto>>.Ok(comments);
        }

        public async Task<ServiceResult<MedicineUnitsPriceDto>> GetMedicineUnitsPriceAsync(int medicineId, string lang, CancellationToken cancellationToken = default)
        {
            var medicine = await _context.Medicines
                .Include(m => m.Unit1)
                .Include(m => m.Unit2)
                .FirstOrDefaultAsync(m => m.Id == medicineId && !m.IsDeleted, cancellationToken);

            if (medicine == null)
                return ServiceResult<MedicineUnitsPriceDto>.Fail(ServiceErrorType.NotFound, "MedicineNotFound");

            if (medicine.Unit1 == null || medicine.Unit2 == null)
                return ServiceResult<MedicineUnitsPriceDto>.Fail(ServiceErrorType.Validation, "MedicineUnitsNotFound");

            var isArabic = lang == "ar";
            var (unit1Price, unit2Price) = MedicinePriceHelper.CalculateUnitPrices(
                medicine.MedicinePrice,
                medicine.Unit1.NameEn,
                medicine.Unit2.NameEn,
                medicine.Unit1Count,
                medicine.Unit2Count);

            var result = new MedicineUnitsPriceDto
            {
                Unit1Name = isArabic ? medicine.Unit1.NameAr : medicine.Unit1.NameEn,
                Unit1Price = unit1Price,
                Unit2Name = isArabic ? medicine.Unit2.NameAr : medicine.Unit2.NameEn,
                Unit2Price = unit2Price
            };

            return ServiceResult<MedicineUnitsPriceDto>.Ok(result);
        }

        public async Task<ServiceResult<IReadOnlyList<ProviderLookupDto>>> GetProvidersForApprovalAsync(bool isChronic, string lang, CancellationToken cancellationToken = default)
        {
            var isArabic = lang == "ar";
            var query = _context.Providers
                .Include(p => p.Category)
                .Where(p => !p.IsDeleted);

            if (isChronic)
            {
                // For chronic approvals: only pharmacies (CategoryId == 1)
                query = query.Where(p => p.CategoryId == 1);
            }
            else
            {
                // For regular approvals: exclude pharmacies (CategoryId != 1)
                query = query.Where(p => p.CategoryId != 1);
            }

            var providers = await query
                .Select(p => new ProviderLookupDto
                {
                    Id = p.Id,
                    Name = isArabic ? p.NameAr : p.NameEn,
                    CommercialName = p.CommercialName,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category != null ? (isArabic ? p.Category.NameAr : p.Category.NameEn) : null
                })
                .OrderBy(p => p.Name)
                .ToListAsync(cancellationToken);

            return ServiceResult<IReadOnlyList<ProviderLookupDto>>.Ok(providers.AsReadOnly());
        }

        public async Task<ServiceResult<IReadOnlyList<ProviderServiceDto>>> GetProviderServicesAsync(int providerId, string lang, CancellationToken cancellationToken = default)
        {
            // Validate provider exists and is not deleted
            var provider = await _context.Providers
                .FirstOrDefaultAsync(p => p.Id == providerId && !p.IsDeleted, cancellationToken);

            if (provider == null)
                return ServiceResult<IReadOnlyList<ProviderServiceDto>>.Fail(ServiceErrorType.NotFound, "ProviderNotFound");

            var isArabic = lang == "ar";

            // Get all services from provider's active price lists
            var services = await _context.ProviderPriceListServices
                .Include(ppls => ppls.ProviderPriceList)
                    .ThenInclude(ppl => ppl!.Provider)
                .Include(ppls => ppls.CPT)
                .Where(ppls => 
                    ppls.ProviderPriceList != null &&
                    ppls.ProviderPriceList.ProviderId == providerId &&
                    !ppls.ProviderPriceList.IsDeleted &&
                    ppls.CPT != null)
                .Select(ppls => new ProviderServiceDto
                {
                    CptId = ppls.CptId,
                    EnName = ppls.CPT!.EnName,
                    ArName = ppls.CPT.ArName,
                    Price = ppls.Price,
                    Discount = ppls.Discount,
                    ProviderPriceListServiceId = ppls.Id,
                    ProviderPriceListId = ppls.ProviderPriceListId
                })
                .Distinct() // Remove duplicates if same CPT appears in multiple price lists
                .OrderBy(s => isArabic ? s.ArName : s.EnName)
                .ToListAsync(cancellationToken);

            return ServiceResult<IReadOnlyList<ProviderServiceDto>>.Ok(services.AsReadOnly());
        }

        public async Task<ServiceResult<IReadOnlyList<MemberServiceDto>>> GetMemberServicesAsync(string nationalId, bool isChronic, string lang, CancellationToken cancellationToken = default)
        {
            // Get member by NationalId
            var member = await _context.MemberInfos
                .Include(m => m.MemberPolicies.Where(mp => !mp.IsDeleted && !mp.IsExpired))
                    .ThenInclude(mp => mp.Policy)
                        .ThenInclude(p => p!.GeneralPrograms)
                            .ThenInclude(gp => gp.ServiceClassDetails)
                                .ThenInclude(scd => scd.ServiceClass)
                .FirstOrDefaultAsync(m => m.NationalId == nationalId && !m.IsDeleted, cancellationToken);

            if (member == null)
                return ServiceResult<IReadOnlyList<MemberServiceDto>>.Fail(ServiceErrorType.NotFound, "MemberNotFound");

            // Get latest active policy
            var latestPolicy = member.MemberPolicies
                .OrderByDescending(mp => mp.AddDate)
                .FirstOrDefault();

            if (latestPolicy?.Policy == null)
                return ServiceResult<IReadOnlyList<MemberServiceDto>>.Fail(ServiceErrorType.NotFound, "MemberHasNoPolicy");

            var isArabic = lang == "ar";

            // Get all service classes from all programs in the policy
            var allServiceClasses = latestPolicy.Policy.GeneralPrograms
                .SelectMany(gp => gp.ServiceClassDetails)
                .Where(scd => scd.ServiceClass != null && !scd.ServiceClass.IsDeleted)
                .Select(scd => scd.ServiceClass!)
                .Distinct()
                .ToList();

            // Filter based on isChronic
            if (isChronic)
            {
                // For chronic approvals: only ServiceClassId = 0 and 1
                allServiceClasses = allServiceClasses
                    .Where(sc => sc.Id == 0 || sc.Id == 1)
                    .ToList();
            }
            // For regular approvals: return all services (no filter)

            var services = allServiceClasses
                .Select(sc => new MemberServiceDto
                {
                    ServiceClassId = sc.Id,
                    Name = isArabic ? sc.NameAr : sc.NameEn
                })
                .OrderBy(s => s.Name)
                .ToList();

            return ServiceResult<IReadOnlyList<MemberServiceDto>>.Ok(services.AsReadOnly());
        }

        public async Task<ServiceResult<ApprovalReadDto>> CreateRegularApprovalAsync(RegularApprovalCreateDto dto, string userId, string lang, CancellationToken cancellationToken = default)
        {
            // Validate provider is NOT a pharmacy
            if (dto.ProviderId.HasValue)
            {
                var provider = await _context.Providers
                    .FirstOrDefaultAsync(p => p.Id == dto.ProviderId.Value && !p.IsDeleted, cancellationToken);

                if (provider == null)
                    return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.NotFound, "ProviderNotFound");

                if (provider.CategoryId == 1) // Pharmacy category (CategoryId = 1 for pharmacies)
                    return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.Validation, "RegularApprovalCannotUsePharmacy");
            }

            // Validate services and get prices from PriceList
            if (dto.Services != null && dto.Services.Any())
            {
                var serviceIds = dto.Services.Select(s => s.ServiceId).Distinct().ToList();
                var validServices = await _context.ServiceClasses
                    .Where(s => serviceIds.Contains(s.Id) && !s.IsDeleted)
                    .Select(s => s.Id)
                    .ToListAsync(cancellationToken);

                var invalidServiceIds = serviceIds.Except(validServices).ToList();
                if (invalidServiceIds.Any())
                    return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.Validation, $"InvalidServiceIds: {string.Join(", ", invalidServiceIds)}");

                // Validate CtoNameId (CPT.Id) and get prices from ProviderPriceListService
                if (!dto.ProviderId.HasValue)
                    return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.Validation, "ProviderIdRequiredForServices");

                var cptIds = dto.Services
                    .Where(s => s.CtoNameId.HasValue)
                    .Select(s => s.CtoNameId!.Value)
                    .Distinct()
                    .ToList();

                if (cptIds.Any())
                {
                    var priceListServices = await _context.ProviderPriceListServices
                        .Include(ppls => ppls.ProviderPriceList)
                        .Where(ppls =>
                            ppls.ProviderPriceList != null &&
                            ppls.ProviderPriceList.ProviderId == dto.ProviderId.Value &&
                            !ppls.ProviderPriceList.IsDeleted &&
                            cptIds.Contains(ppls.CptId))
                        .ToListAsync(cancellationToken);

                    // Validate all CtoNameIds exist in provider's price list
                    var foundCptIds = priceListServices.Select(ppls => ppls.CptId).Distinct().ToList();
                    var missingCptIds = cptIds.Except(foundCptIds).ToList();
                    if (missingCptIds.Any())
                        return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.Validation, $"CPTIdsNotFoundInProviderPriceList: {string.Join(", ", missingCptIds)}");
                }
            }

            var entity = new Approval
            {
                MemberId = dto.MemberId,
                ProviderId = dto.ProviderId,
                ProviderLocationId = dto.ProviderLocationId,
                ReceiveTime = dto.ReceiveTime,
                ReceiveDate = dto.ReceiveDate,
                ClaimFormNumber = dto.ClaimFormNumber?.Trim(),
                PoolId = dto.AdditionalPoolId,
                ChronicForDate = dto.ChronicForDate,
                RequestEmailOrMobile = dto.RequestEmailOrMobile?.Trim(),
                CommentId = dto.CommentId,
                MaxAllowAmount = dto.MaxAllowAmount,
                InternalNote = dto.InternalNote?.Trim(),
                IsDebit = dto.IsDebit,
                IsRepeated = dto.IsRepeated,
                IsDelivery = dto.IsDelivery,
                IsApproved = false,
                IsDispensed = false,
                IsCanceled = false,
                IsFromProviderPortal = false,
                IsChronic = false,
                CreatedAt = DateTime.Now,
                CreatedBy = userId
            };

            var repo = _unitOfWork.Repository<Approval>();
            await repo.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Add Diagnostics if provided
            if (dto.DiagnosticIds != null && dto.DiagnosticIds.Any())
            {
                foreach (var diagnosticId in dto.DiagnosticIds)
                {
                    entity.Diagnostics.Add(new ApprovalDiagnostic
                    {
                        ApprovalId = entity.Id,
                        DiagnosticId = diagnosticId
                    });
                }
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            // Add Services with details - get price from PriceList and copayment from ServiceClassDetail
            if (dto.Services != null && dto.Services.Any() && dto.ProviderId.HasValue)
            {
                // Get all prices from ProviderPriceListService for this provider
                var cptIds = dto.Services
                    .Where(s => s.CtoNameId.HasValue)
                    .Select(s => s.CtoNameId!.Value)
                    .Distinct()
                    .ToList();

                var priceListServices = await _context.ProviderPriceListServices
                    .Include(ppls => ppls.ProviderPriceList)
                    .Where(ppls =>
                        ppls.ProviderPriceList != null &&
                        ppls.ProviderPriceList.ProviderId == dto.ProviderId.Value &&
                        !ppls.ProviderPriceList.IsDeleted &&
                        cptIds.Contains(ppls.CptId))
                    .ToDictionaryAsync(ppls => ppls.CptId, ppls => ppls.Price, cancellationToken);

                // Get copayments from ServiceClassDetail if MemberId is provided
                Dictionary<int, decimal> copayments = new Dictionary<int, decimal>();
                if (dto.MemberId.HasValue)
                {
                    // Get member's latest policy program
                    var memberPolicy = await _context.MemberPolicyInfos
                        .Include(mp => mp.Policy)
                            .ThenInclude(p => p!.GeneralPrograms)
                        .Where(mp => mp.MemberId == dto.MemberId.Value && !mp.IsDeleted)
                        .OrderByDescending(mp => mp.AddDate)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (memberPolicy != null)
                    {
                        // Get program IDs from member's policy programs
                        var programIds = new List<int>();
                        if (memberPolicy.Policy?.GeneralPrograms != null)
                        {
                            programIds = memberPolicy.Policy.GeneralPrograms.Select(gp => gp.Id).ToList();
                        }
                        // Also include direct ProgramId from MemberPolicyInfo
                        if (memberPolicy.ProgramId > 0 && !programIds.Contains(memberPolicy.ProgramId))
                        {
                            programIds.Add(memberPolicy.ProgramId);
                        }

                        if (programIds.Any())
                        {
                            var serviceIds = dto.Services.Select(s => s.ServiceId).Distinct().ToList();
                            var serviceClassDetails = await _context.ServiceClassDetails
                                .Where(scd =>
                                    programIds.Contains(scd.ProgramId) &&
                                    serviceIds.Contains(scd.ServiceClassId) &&
                                    scd.Copayment.HasValue)
                                .ToListAsync(cancellationToken);

                            // Group by ServiceClassId and take the first copayment if multiple exist
                            copayments = serviceClassDetails
                                .GroupBy(scd => scd.ServiceClassId)
                                .ToDictionary(
                                    g => g.Key,
                                    g => g.First().Copayment!.Value
                                );
                        }
                    }
                }

                foreach (var serviceDto in dto.Services)
                {
                    // Get price from PriceList using CptId (CtoNameId)
                    decimal price = 0;
                    if (serviceDto.CtoNameId.HasValue && priceListServices.TryGetValue(serviceDto.CtoNameId.Value, out var priceFromList))
                    {
                        price = priceFromList;
                    }
                    else if (!serviceDto.CtoNameId.HasValue)
                    {
                        return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.Validation, $"CtoNameIdRequiredForService: ServiceId {serviceDto.ServiceId}");
                    }

                    // Get copayment if exists
                    decimal copayment = 0;
                    if (copayments.TryGetValue(serviceDto.ServiceId, out var copaymentValue))
                    {
                        copayment = copaymentValue;
                    }

                    // Calculate total: Price - (Price * Copayment / 100)
                    // Note: Total is calculated but not stored in ApprovalServiceClass
                    // It's calculated in the DTO when mapping

                    entity.Services.Add(new ApprovalServiceClass
                    {
                        ApprovalId = entity.Id,
                        ServiceClassId = serviceDto.ServiceId,
                        CtoNameId = serviceDto.CtoNameId,
                        Price = price,
                        Qty = serviceDto.Qty,
                        Copayment = copayment,
                        StatusId = serviceDto.StatusId,
                        ReasonId = serviceDto.ReasonId
                    });
                }
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            // Reload entity with related data
            var loadedEntity = await _context.Approvals
                .Include(a => a.Member)
                .Include(a => a.Provider)
                .Include(a => a.ProviderLocation)
                .Include(a => a.Comment)
                .Include(a => a.Diagnostics)
                    .ThenInclude(ad => ad.Diagnostic)
                .Include(a => a.Services)
                    .ThenInclude(asc => asc.ServiceClass)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == entity.Id, cancellationToken);

            if (loadedEntity == null)
                return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.NotFound, "ApprovalNotFound");

            return ServiceResult<ApprovalReadDto>.Ok(await MapAsync(loadedEntity, lang, cancellationToken));
        }

        public async Task<ServiceResult<ApprovalReadDto>> CreateChronicApprovalAsync(ChronicApprovalCreateDto dto, string userId, string lang, CancellationToken cancellationToken = default)
        {
            // Validate provider IS a pharmacy
            if (dto.ProviderId.HasValue)
            {
                var provider = await _context.Providers
                    .FirstOrDefaultAsync(p => p.Id == dto.ProviderId.Value && !p.IsDeleted, cancellationToken);

                if (provider == null)
                    return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.NotFound, "ProviderNotFound");

                if (provider.CategoryId != 1) // Not a pharmacy (CategoryId = 1 for pharmacies)
                    return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.Validation, "ChronicApprovalMustUsePharmacy");
            }

            // Validate medicines
            if (dto.Medicines != null && dto.Medicines.Any())
            {
                var medicineIds = dto.Medicines.Select(m => m.MedicineId).Distinct().ToList();
                var validMedicines = await _context.Medicines
                    .Where(m => medicineIds.Contains(m.Id) && !m.IsDeleted)
                    .Select(m => m.Id)
                    .ToListAsync(cancellationToken);

                var invalidMedicineIds = medicineIds.Except(validMedicines).ToList();
                if (invalidMedicineIds.Any())
                    return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.Validation, $"InvalidMedicineIds: {string.Join(", ", invalidMedicineIds)}");

                var unitIds = dto.Medicines.Select(m => m.UnitId).Distinct().ToList();
                var validUnits = await _context.Units
                    .Where(u => unitIds.Contains(u.Id) && !u.IsDeleted)
                    .Select(u => u.Id)
                    .ToListAsync(cancellationToken);

                var invalidUnitIds = unitIds.Except(validUnits).ToList();
                if (invalidUnitIds.Any())
                    return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.Validation, $"InvalidUnitIds: {string.Join(", ", invalidUnitIds)}");

                var serviceIds = dto.Medicines.Select(m => m.ServiceId).Distinct().ToList();
                var validServices = await _context.ServiceClasses
                    .Where(s => serviceIds.Contains(s.Id) && !s.IsDeleted)
                    .Select(s => s.Id)
                    .ToListAsync(cancellationToken);

                var invalidServiceIds = serviceIds.Except(validServices).ToList();
                if (invalidServiceIds.Any())
                    return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.Validation, $"InvalidServiceIds: {string.Join(", ", invalidServiceIds)}");
            }

            var entity = new Approval
            {
                MemberId = dto.MemberId,
                ProviderId = dto.ProviderId,
                ProviderLocationId = dto.ProviderLocationId,
                ReceiveTime = dto.ReceiveTime,
                ReceiveDate = dto.ReceiveDate,
                ClaimFormNumber = dto.ClaimFormNumber?.Trim(),
                PoolId = dto.AdditionalPoolId,
                ChronicForDate = dto.ChronicForDate,
                RequestEmailOrMobile = dto.RequestEmailOrMobile?.Trim(),
                CommentId = dto.CommentId,
                MaxAllowAmount = dto.MaxAllowAmount,
                InternalNote = dto.InternalNote?.Trim(),
                IsDebit = dto.IsDebit,
                IsRepeated = dto.IsRepeated,
                IsDelivery = dto.IsDelivery,
                IsApproved = false,
                IsDispensed = false,
                IsCanceled = false,
                IsFromProviderPortal = false,
                IsChronic = true,
                CreatedAt = DateTime.Now,
                CreatedBy = userId
            };

            var repo = _unitOfWork.Repository<Approval>();
            await repo.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Add Diagnostics if provided
            if (dto.DiagnosticIds != null && dto.DiagnosticIds.Any())
            {
                foreach (var diagnosticId in dto.DiagnosticIds)
                {
                    entity.Diagnostics.Add(new ApprovalDiagnostic
                    {
                        ApprovalId = entity.Id,
                        DiagnosticId = diagnosticId
                    });
                }
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            // Add Medicines with chronic details - get CP from ServiceClassDetail.MemberPercentage
            if (dto.Medicines != null && dto.Medicines.Any())
            {
                // Get CP (Copayment Percentage) from ServiceClassDetail.MemberPercentage if MemberId is provided
                Dictionary<int, decimal> copaymentPercentages = new Dictionary<int, decimal>();
                if (dto.MemberId.HasValue)
                {
                    // Get member's latest policy program
                    var memberPolicy = await _context.MemberPolicyInfos
                        .Include(mp => mp.Policy)
                            .ThenInclude(p => p!.GeneralPrograms)
                        .Where(mp => mp.MemberId == dto.MemberId.Value && !mp.IsDeleted)
                        .OrderByDescending(mp => mp.AddDate)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (memberPolicy != null)
                    {
                        // Get program IDs from member's policy programs
                        var programIds = new List<int>();
                        if (memberPolicy.Policy?.GeneralPrograms != null)
                        {
                            programIds = memberPolicy.Policy.GeneralPrograms.Select(gp => gp.Id).ToList();
                        }
                        // Also include direct ProgramId from MemberPolicyInfo
                        if (memberPolicy.ProgramId > 0 && !programIds.Contains(memberPolicy.ProgramId))
                        {
                            programIds.Add(memberPolicy.ProgramId);
                        }

                        if (programIds.Any())
                        {
                            var serviceIds = dto.Medicines.Select(m => m.ServiceId).Distinct().ToList();
                            var serviceClassDetails = await _context.ServiceClassDetails
                                .Where(scd =>
                                    programIds.Contains(scd.ProgramId) &&
                                    serviceIds.Contains(scd.ServiceClassId) &&
                                    scd.MemberPercentage.HasValue)
                                .ToListAsync(cancellationToken);

                            // Group by ServiceClassId and take the first MemberPercentage if multiple exist
                            copaymentPercentages = serviceClassDetails
                                .GroupBy(scd => scd.ServiceClassId)
                                .ToDictionary(
                                    g => g.Key,
                                    g => g.First().MemberPercentage!.Value
                                );
                        }
                    }
                }

                foreach (var medicineDto in dto.Medicines)
                {
                    // Get CP (Copayment Percentage) from ServiceClassDetail if exists
                    decimal cp = medicineDto.CP; // Default to value from DTO
                    if (copaymentPercentages.TryGetValue(medicineDto.ServiceId, out var cpValue))
                    {
                        cp = cpValue;
                    }

                    entity.Medicines.Add(new ApprovalMedicine
                    {
                        ApprovalId = entity.Id,
                        ServiceId = medicineDto.ServiceId,
                        MedicineId = medicineDto.MedicineId,
                        UnitId = medicineDto.UnitId,
                        Days = medicineDto.Days,
                        Price = medicineDto.Price,
                        Qty = medicineDto.Qty,
                        CP = cp,
                        StatusId = medicineDto.StatusId,
                        ReasonId = medicineDto.ReasonId,
                        IsDebit = medicineDto.IsDebit
                    });
                }
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            // Reload entity with related data
            var loadedEntity = await _context.Approvals
                .Include(a => a.Member)
                .Include(a => a.Provider)
                .Include(a => a.ProviderLocation)
                .Include(a => a.Comment)
                .Include(a => a.Diagnostics)
                    .ThenInclude(ad => ad.Diagnostic)
                .Include(a => a.Medicines)
                    .ThenInclude(am => am.Medicine)
                .Include(a => a.Medicines)
                    .ThenInclude(am => am.Unit)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == entity.Id, cancellationToken);

            if (loadedEntity == null)
                return ServiceResult<ApprovalReadDto>.Fail(ServiceErrorType.NotFound, "ApprovalNotFound");

            return ServiceResult<ApprovalReadDto>.Ok(await MapAsync(loadedEntity, lang, cancellationToken));
        }
    }
}
