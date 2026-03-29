using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MCIApi.Application.Common;
using MCIApi.Application.Policies.DTOs;
using MCIApi.Application.Policies.Interfaces;
using MCIApi.Domain.Entities;
using MCIApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace MCIApi.Infrastructure.Services
{
    public class PolicyService : IPolicyService
    {
        private readonly AppDbContext _context;

        public PolicyService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<PolicyPagedResultDto>> GetAllAsync(PolicyFilterDto filter, string lang, CancellationToken cancellationToken = default)
        {
            var page = filter.Page < 1 ? 1 : filter.Page;
            var limit = filter.Limit <= 0 ? 10 : filter.Limit;

            var query = _context.Policies
                .AsNoTracking()
                .Include(p => p.PolicyType)
                .Include(p => p.CarrierCompany)
                .Include(p => p.Client)
                .Where(p => !p.IsDeleted);

            // Filter by ClientId if provided
            if (filter.ClientId.HasValue)
            {
                query = query.Where(p => p.ClientId == filter.ClientId.Value);
            }

            // Filter by PolicyTypeId if provided
            if (filter.PolicyTypeId.HasValue)
            {
                query = query.Where(p => p.PolicyTypeId == filter.PolicyTypeId.Value);
            }

            // Filter by CarrierCompanyId if provided
            if (filter.CarrierCompanyId.HasValue)
            {
                query = query.Where(p => p.CarrierCompanyId == filter.CarrierCompanyId.Value);
            }

            // Apply search if provided
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var normalizedTerm = filter.Search.Trim().ToLowerInvariant();
                query = ApplyPolicySearch(query, filter.SearchColumn, normalizedTerm, lang);
            }

            var total = await query.CountAsync(cancellationToken);
            var policies = await query
                .OrderByDescending(p => p.Id)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync(cancellationToken);

            var data = policies.Select(p => MapDto(p, lang)).ToList();
            var totalPages = (int)Math.Ceiling(total / (double)limit);

            var dto = new PolicyPagedResultDto
            {
                TotalPolicies = total,
                CurrentPage = page,
                Limit = limit,
                TotalPages = totalPages,
                Data = data
            };

            return ServiceResult<PolicyPagedResultDto>.Ok(dto);
        }

        public async Task<ServiceResult<byte[]>> ExportToExcelAsync(string lang, string? searchColumn = null, string? searchTerm = null, int? clientId = null, int? policyTypeId = null, int? carrierCompanyId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Policies
                .AsNoTracking()
                .Include(p => p.PolicyType)
                .Include(p => p.CarrierCompany)
                .Include(p => p.Client)
                .Where(p => !p.IsDeleted);

            if (clientId.HasValue)
            {
                query = query.Where(p => p.ClientId == clientId.Value);
            }

            if (policyTypeId.HasValue)
            {
                query = query.Where(p => p.PolicyTypeId == policyTypeId.Value);
            }

            if (carrierCompanyId.HasValue)
            {
                query = query.Where(p => p.CarrierCompanyId == carrierCompanyId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var normalizedTerm = searchTerm.Trim().ToLowerInvariant();
                query = ApplyPolicySearch(query, searchColumn, normalizedTerm, lang);
            }

            var policies = await query
                .OrderByDescending(p => p.Id)
                .ToListAsync(cancellationToken);

            // Set EPPlus license for non-commercial use (same pattern as MemberInfoService/ClientService)
            try
            {
                ExcelPackage.License.SetNonCommercialPersonal("MCI API");
            }
            catch (InvalidOperationException)
            {
                // License already set, ignore
            }
            catch (Exception)
            {
                // Ignore other license errors; ExcelPackage will still throw later if critical
            }

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Policies");

            var isArabic = lang == "ar";

            // Header
            worksheet.Cells[1, 1].Value = "PolicyId";
            worksheet.Cells[1, 2].Value = isArabic ? "اسم العميل" : "Client Name";
            worksheet.Cells[1, 3].Value = isArabic ? "تاريخ البداية" : "Start Date";
            worksheet.Cells[1, 4].Value = isArabic ? "تاريخ النهاية" : "End Date";
            worksheet.Cells[1, 5].Value = isArabic ? "نوع الوثيقة" : "Policy Type";
            worksheet.Cells[1, 6].Value = isArabic ? "شركة التأمين" : "Carrier Company";
            worksheet.Cells[1, 7].Value = isArabic ? "إجمالي المبلغ" : "Total Amount";

            var row = 2;
            foreach (var policy in policies)
            {
                var dto = MapDto(policy, lang);

                worksheet.Cells[row, 1].Value = dto.Id;
                worksheet.Cells[row, 2].Value = dto.ClientName;
                worksheet.Cells[row, 3].Value = dto.StartDate.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 4].Value = dto.EndDate.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 5].Value = dto.PolicyTypeName;
                worksheet.Cells[row, 6].Value = dto.CarrierCompanyName;
                worksheet.Cells[row, 7].Value = dto.TotalAmount;

                row++;
            }

            worksheet.Cells[1, 1, row - 1, 7].AutoFitColumns();

            var bytes = package.GetAsByteArray();
            return ServiceResult<byte[]>.Ok(bytes);
        }

        private static IQueryable<Policy> ApplyPolicySearch(IQueryable<Policy> query, string? searchColumn, string searchTerm, string lang)
        {
            if (string.IsNullOrWhiteSpace(searchColumn))
                return ApplyWildcardPolicySearch(query, searchTerm, lang);

            var column = searchColumn.Trim().ToLowerInvariant();
            var isArabic = lang == "ar";

            return column switch
            {
                "id" or "policyid" => ApplyPolicyIdSearch(query, searchTerm),
                "clientname" or "client_name" => isArabic
                    ? query.Where(p => p.Client != null && p.Client.ArabicName != null && p.Client.ArabicName.ToLower().StartsWith(searchTerm))
                    : query.Where(p => p.Client != null && p.Client.EnglishName != null && p.Client.EnglishName.ToLower().StartsWith(searchTerm)),
                "policytypename" or "policytype_name" => isArabic
                    ? query.Where(p => p.PolicyType != null && p.PolicyType.NameAr != null && p.PolicyType.NameAr.ToLower().StartsWith(searchTerm))
                    : query.Where(p => p.PolicyType != null && p.PolicyType.NameEn != null && p.PolicyType.NameEn.ToLower().StartsWith(searchTerm)),
                "carriercompanyname" or "carriercompany_name" => isArabic
                    ? query.Where(p => p.CarrierCompany != null && p.CarrierCompany.NameAr != null && p.CarrierCompany.NameAr.ToLower().StartsWith(searchTerm))
                    : query.Where(p => p.CarrierCompany != null && p.CarrierCompany.NameEn != null && p.CarrierCompany.NameEn.ToLower().StartsWith(searchTerm)),
                _ => ApplyWildcardPolicySearch(query, searchTerm, lang)
            };
        }

        private static IQueryable<Policy> ApplyPolicyIdSearch(IQueryable<Policy> query, string searchTerm)
        {
            if (int.TryParse(searchTerm, out var id))
                return query.Where(p => p.Id == id);

            return query.Where(p => p.Id.ToString().StartsWith(searchTerm));
        }

        private static IQueryable<Policy> ApplyWildcardPolicySearch(IQueryable<Policy> query, string searchTerm, string lang)
        {
            var isArabic = lang == "ar";

            if (isArabic)
            {
                return query.Where(p =>
                    p.Id.ToString().StartsWith(searchTerm) ||
                    (p.Client != null && p.Client.ArabicName != null && p.Client.ArabicName.ToLower().StartsWith(searchTerm)) ||
                    (p.PolicyType != null && p.PolicyType.NameAr != null && p.PolicyType.NameAr.ToLower().StartsWith(searchTerm)) ||
                    (p.CarrierCompany != null && p.CarrierCompany.NameAr != null && p.CarrierCompany.NameAr.ToLower().StartsWith(searchTerm)));
            }
            else
            {
                return query.Where(p =>
                    p.Id.ToString().StartsWith(searchTerm) ||
                    (p.Client != null && p.Client.EnglishName != null && p.Client.EnglishName.ToLower().StartsWith(searchTerm)) ||
                    (p.PolicyType != null && p.PolicyType.NameEn != null && p.PolicyType.NameEn.ToLower().StartsWith(searchTerm)) ||
                    (p.CarrierCompany != null && p.CarrierCompany.NameEn != null && p.CarrierCompany.NameEn.ToLower().StartsWith(searchTerm)));
            }
        }

        public async Task<ServiceResult<PolicyUpdateDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default)
        {
            var policy = await _context.Policies
                .AsNoTracking()
                .Include(p => p.PolicyType)
                .Include(p => p.CarrierCompany)
                .Include(p => p.Client)
                .Include(p => p.GeneralPrograms)
                    .ThenInclude(gp => gp.ProgramName)
                .Include(p => p.GeneralPrograms)
                    .ThenInclude(gp => gp.RoomType)
                .Include(p => p.GeneralPrograms)
                    .ThenInclude(gp => gp.ServiceClassDetails)
                        .ThenInclude(scd => scd.ServiceClass)
                .Include(p => p.GeneralPrograms)
                    .ThenInclude(gp => gp.ServiceClassDetails)
                        .ThenInclude(scd => scd.Pool)
                            .ThenInclude(pool => pool.PoolType)
                .Include(p => p.Pools)
                    .ThenInclude(pool => pool.PoolType)
                .Include(p => p.RefundRules)
                    .ThenInclude(rr => rr.ReimbursementType)
                .Include(p => p.RefundRules)
                    .ThenInclude(rr => rr.Program)
                .Include(p => p.RefundRules)
                    .ThenInclude(rr => rr.Pricelist)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);

            if (policy == null)
                return ServiceResult<PolicyUpdateDto>.Fail(ServiceErrorType.NotFound, "PolicyNotFound");

            return ServiceResult<PolicyUpdateDto>.Ok(MapToUpdateDto(policy, lang));
        }

        public async Task<ServiceResult<PolicyDto>> CreateAsync(PolicyCreateDto dto, string lang, string currentUser, CancellationToken cancellationToken = default)
        {
            // Validate PolicyType
            var policyTypeExists = await _context.PolicyTypes.AnyAsync(pt => pt.Id == dto.PolicyTypeId && !pt.IsDeleted, cancellationToken);
            if (!policyTypeExists)
                return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "PolicyTypeNotFound");

            // Validate CarrierCompany
            var carrierCompanyExists = await _context.CarrierCompanies.AnyAsync(cc => cc.Id == dto.CarrierCompanyId && !cc.IsDeleted, cancellationToken);
            if (!carrierCompanyExists)
                return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "CarrierCompanyNotFound");

            // Validate Client if provided
            if (dto.ClientId.HasValue)
            {
                var clientExists = await _context.Clients.AnyAsync(c => c.Id == dto.ClientId.Value && !c.IsDeleted, cancellationToken);
                if (!clientExists)
                    return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "ClientNotFound");
            }

            var policy = new Policy
            {
                PolicyTypeId = dto.PolicyTypeId,
                CarrierCompanyId = dto.CarrierCompanyId,
                ClientId = dto.ClientId,
                StartDate = dto.StartDate.Date,
                EndDate = dto.EndDate.Date,
                Status = PolicyStatus.Pending, // Default to Pending
                IsCalculateUpperPeday = dto.IsCalculateUpperPeday,
                TotalAmount = dto.TotalAmount,
                WarningOnPercentage = dto.WarningOnPercentage,
                CreatedBy = currentUser,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            // Create Pools FIRST and store them in a list for reference by index
            var createdPools = new List<Pool>();
            if (dto.ListOfPool != null && dto.ListOfPool.Any())
            {
                foreach (var poolDto in dto.ListOfPool)
                {
                    // Validate that at least one PoolTypeId is provided
                    if (poolDto.PoolTypeIds == null || !poolDto.PoolTypeIds.Any())
                        return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "PoolTypeIdsRequired");

                    // Validate all PoolTypes exist
                    var poolTypeIds = poolDto.PoolTypeIds.Distinct().ToList();
                    var existingPoolTypes = await _context.PoolTypes
                        .Where(pt => poolTypeIds.Contains(pt.Id) && !pt.IsDeleted)
                        .Select(pt => pt.Id)
                        .ToListAsync(cancellationToken);

                    var missingPoolTypeIds = poolTypeIds.Except(existingPoolTypes).ToList();
                    if (missingPoolTypeIds.Any())
                        return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, $"PoolTypeNotFound: {string.Join(", ", missingPoolTypeIds)}");

                    // Create one Pool record for each PoolTypeId with the same configuration
                    foreach (var poolTypeId in poolTypeIds)
                    {
                        var pool = new Pool
                        {
                            PoolTypeId = poolTypeId,
                            ApplyOn = poolDto.ApplyOn,
                            ApplyTo = poolDto.ApplyTo,
                            PoolLimit = poolDto.PoolLimit,
                            MemberCount = poolDto.MemberCount,
                            PercentageOfMember = poolDto.PercentageOfMember,
                            IsLimitExceed = poolDto.IsLimitExceed,
                            Notes = poolDto.Notes
                        };
                        policy.Pools.Add(pool);
                        createdPools.Add(pool); // Store for index-based reference
                    }
                }
            }

            // Add Programs (after pools are created so we can reference them by index)
            if (dto.ListOfProgram != null && dto.ListOfProgram.Any())
            {
                foreach (var programDto in dto.ListOfProgram)
                {
                    // Validate ProgramName exists
                    var programNameExists = await _context.Programs.AnyAsync(p => p.Id == programDto.ProgramNameId && !p.IsDeleted, cancellationToken);
                    if (!programNameExists)
                        return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "ProgramNameNotFound");

                    // Validate RoomType if provided
                    if (programDto.RoomTypeId.HasValue)
                    {
                        var roomTypeExists = await _context.RoomTypes.AnyAsync(rt => rt.Id == programDto.RoomTypeId.Value && !rt.IsDeleted, cancellationToken);
                        if (!roomTypeExists)
                            return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "RoomTypeNotFound");
                    }

                    var program = new GeneralProgram
                    {
                        ProgramNameId = programDto.ProgramNameId,
                        Limit = programDto.Limit,
                        RoomTypeId = programDto.RoomTypeId,
                        Note = programDto.Note
                    };

                    // Add Service Class Details to the program
                    if (programDto.ListOfServiceClasses != null && programDto.ListOfServiceClasses.Any())
                    {
                        foreach (var serviceClassDto in programDto.ListOfServiceClasses)
                        {
                            // Validate ServiceClass exists
                            var serviceClassExists = await _context.ServiceClasses.AnyAsync(sc => sc.Id == serviceClassDto.ServiceClassId && !sc.IsDeleted, cancellationToken);
                            if (!serviceClassExists)
                                return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, $"ServiceClassNotFound: {serviceClassDto.ServiceClassId}");

                            // Validate PoolIndex if provided and get the Pool entity reference
                            Pool? referencedPool = null;
                            if (serviceClassDto.PoolIndex.HasValue)
                            {
                                var poolIndex = serviceClassDto.PoolIndex.Value;
                                if (poolIndex < 0 || poolIndex >= createdPools.Count)
                                    return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, $"PoolIndexOutOfRange: {poolIndex}. Valid range is 0 to {createdPools.Count - 1}");
                                
                                // Get the pool from the created pools list by index
                                referencedPool = createdPools[poolIndex];
                            }

                            program.ServiceClassDetails.Add(new ServiceClassDetail
                            {
                                ServiceClassId = serviceClassDto.ServiceClassId,
                                ServiceLimitType = serviceClassDto.ServiceLimitType,
                                Pool = referencedPool, // Reference the Pool entity directly - EF will set PoolId after SaveChanges
                                ServiceLimit = serviceClassDto.ServiceLimit,
                                MemberCount = serviceClassDto.MemberCount,
                                MemberPercentage = serviceClassDto.MemberPercentage,
                                ApplyTo = serviceClassDto.ApplyTo,
                                Copayment = serviceClassDto.Copayment,
                                Notes = serviceClassDto.Notes,
                                OnlyRefund = serviceClassDto.OnlyRefund
                            });
                        }
                    }

                    policy.GeneralPrograms.Add(program);
                }
            }

            // Add Reimbursements
            if (dto.ListOfReimbursement != null && dto.ListOfReimbursement.Any())
            {
                foreach (var reimbursementDto in dto.ListOfReimbursement)
                {
                    // Validate ReimbursementType if provided
                    if (reimbursementDto.ReimbursementTypeId.HasValue)
                    {
                        var reimbursementTypeExists = await _context.ReimbursementTypes.AnyAsync(rt => rt.Id == reimbursementDto.ReimbursementTypeId.Value && !rt.IsDeleted, cancellationToken);
                    if (!reimbursementTypeExists)
                        return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "ReimbursementTypeNotFound");
                    }

                    // Validate Program if provided
                    if (reimbursementDto.ProgramId.HasValue)
                    {
                        var programExists = await _context.Programs.AnyAsync(p => p.Id == reimbursementDto.ProgramId.Value && !p.IsDeleted, cancellationToken);
                        if (!programExists)
                            return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "ProgramNotFound");
                    }

                    // Validate Pricelist if provided
                    if (reimbursementDto.PricelistId.HasValue)
                    {
                        var pricelistExists = await _context.ProviderPriceLists.AnyAsync(pl => pl.Id == reimbursementDto.PricelistId.Value, cancellationToken);
                        if (!pricelistExists)
                            return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "PricelistNotFound");
                    }

                    policy.RefundRules.Add(new RefundRule
                    {
                        ReimbursementTypeId = reimbursementDto.ReimbursementTypeId,
                        ApplyOn = reimbursementDto.ApplyOn,
                        ProgramId = reimbursementDto.ProgramId,
                        PricelistId = reimbursementDto.PricelistId,
                        ApplyBy = reimbursementDto.ApplyBy,
                        MaxValue = reimbursementDto.MaxValue,
                        ReimbursementPercentage = reimbursementDto.ReimbursementPercentage,
                        Notes = reimbursementDto.Notes
                    });
                }
            }

            _context.Policies.Add(policy);
            await _context.SaveChangesAsync(cancellationToken);

            // Load navigation properties needed for DTO mapping
            await _context.Entry(policy).Reference(p => p.PolicyType).LoadAsync(cancellationToken);
            await _context.Entry(policy).Reference(p => p.CarrierCompany).LoadAsync(cancellationToken);
            if (policy.ClientId.HasValue)
            {
                await _context.Entry(policy).Reference(p => p.Client).LoadAsync(cancellationToken);
            }

            await SyncClientPolicyDatesAsync(policy, cancellationToken);

            return ServiceResult<PolicyDto>.Ok(MapDto(policy, lang));
        }

        public async Task<ServiceResult<PolicyDto>> UpdateAsync(int id, PolicyUpdateRequestDto dto, string lang, string currentUser, CancellationToken cancellationToken = default)
        {
            var policy = await _context.Policies
                .Include(p => p.PolicyType)
                .Include(p => p.CarrierCompany)
                .Include(p => p.Client)
                .Include(p => p.GeneralPrograms)
                    .ThenInclude(gp => gp.ServiceClassDetails)
                .Include(p => p.Pools)
                .Include(p => p.RefundRules)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);

            if (policy == null)
                return ServiceResult<PolicyDto>.Fail(ServiceErrorType.NotFound, "PolicyNotFound");

            // Validate PolicyType if provided
            if (dto.PolicyTypeId.HasValue)
            {
                var policyTypeExists = await _context.PolicyTypes.AnyAsync(pt => pt.Id == dto.PolicyTypeId.Value && !pt.IsDeleted, cancellationToken);
                if (!policyTypeExists)
                    return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "PolicyTypeNotFound");
            }

            // Validate CarrierCompany if provided
            if (dto.CarrierCompanyId.HasValue)
            {
                var carrierCompanyExists = await _context.CarrierCompanies.AnyAsync(cc => cc.Id == dto.CarrierCompanyId.Value && !cc.IsDeleted, cancellationToken);
                if (!carrierCompanyExists)
                    return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "CarrierCompanyNotFound");
            }

            // Validate Client if provided
            if (dto.ClientId.HasValue)
            {
                var clientExists = await _context.Clients.AnyAsync(c => c.Id == dto.ClientId.Value && !c.IsDeleted, cancellationToken);
                if (!clientExists)
                    return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "ClientNotFound");
            }

            // Update policy basic info
            // Note: In ASP.NET Core, we cannot distinguish between "field not sent" and "field sent as null"
            // So the behavior is:
            // - If you send a value (non-null): it will be updated
            // - If you send null: it will be set to null (for nullable fields)
            // - If you don't send the field: it will keep the existing value
            
            // PolicyTypeId is non-nullable - only update if value is provided
            if (dto.PolicyTypeId.HasValue)
                policy.PolicyTypeId = dto.PolicyTypeId.Value;
            
            // CarrierCompanyId is non-nullable - only update if value is provided
            if (dto.CarrierCompanyId.HasValue)
                policy.CarrierCompanyId = dto.CarrierCompanyId.Value;
            
            // ClientId is nullable - if sent as null, it will be null in dto.ClientId
            // But HasValue will be false, so we need to check differently
            // Actually, if ClientId is sent as null in JSON, dto.ClientId will be null and HasValue will be false
            // So we can't distinguish between "not sent" and "sent as null"
            // For nullable int?, if you want to set it to null, you must send it explicitly: "ClientId": null
            // But since we can't detect this, we'll use a workaround:
            // - If you want to clear ClientId, send 0 or -1 (special marker values)
            // - Or we accept that null means "don't update" for nullable fields
            // For now, we'll only update if a non-null value is sent
            if (dto.ClientId.HasValue)
                policy.ClientId = dto.ClientId;
            // If you want to set ClientId to null, you need to send a special marker value like 0 or -1
            // Then we can check: if (dto.ClientId == 0) policy.ClientId = null;
            
            // StartDate and EndDate - only update if value is provided
            if (dto.StartDate.HasValue)
                policy.StartDate = dto.StartDate.Value.ToDateTime(TimeOnly.MinValue);
            if (dto.EndDate.HasValue)
                policy.EndDate = dto.EndDate.Value.ToDateTime(TimeOnly.MinValue);
            
            // IsCalculateUpperPeday is bool? - only update if value is provided
            if (dto.IsCalculateUpperPeday.HasValue)
                policy.IsCalculateUpperPeday = dto.IsCalculateUpperPeday.Value;
            
            // TotalAmount is decimal? - only update if value is provided
            if (dto.TotalAmount.HasValue)
                policy.TotalAmount = dto.TotalAmount.Value;
            
            // WarningOnPercentage is nullable - only update if value is provided
            // If you want to set it to null, send it explicitly: "WarningOnPercentage": null
            // But since we can't detect this, we'll only update if a non-null value is sent
            if (dto.WarningOnPercentage.HasValue)
                policy.WarningOnPercentage = dto.WarningOnPercentage;
            policy.UpdatedBy = currentUser;
            policy.UpdatedAt = DateTime.Now;

            // Get existing IDs to track what exists (for update/create logic only, no deletion)
            var existingProgramIds = policy.GeneralPrograms.Select(gp => gp.Id).ToHashSet();
            var existingRefundRuleIds = policy.RefundRules.Select(rr => rr.Id).ToHashSet();

            // STEP 1: Handle Pools FIRST (before programs, since ServiceClassDetails reference pools)
            var existingPoolIds = policy.Pools.Select(p => p.Id).ToHashSet();

            if (dto.ListOfPool != null && dto.ListOfPool.Any())
            {
                foreach (var poolDto in dto.ListOfPool)
                {
                    if (poolDto.Id.HasValue && existingPoolIds.Contains(poolDto.Id.Value))
                    {
                        // Update existing pool - only update non-null properties
                        var pool = policy.Pools.First(p => p.Id == poolDto.Id.Value);
                        
                        // Update PoolTypeIds if provided
                        if (poolDto.PoolTypeIds != null && poolDto.PoolTypeIds.Any())
                        {
                            var poolTypeIds = poolDto.PoolTypeIds.Distinct().ToList();
                            var existingPoolTypes = await _context.PoolTypes
                                .Where(pt => poolTypeIds.Contains(pt.Id) && !pt.IsDeleted)
                                .Select(pt => pt.Id)
                                .ToListAsync(cancellationToken);

                            var missingPoolTypeIds = poolTypeIds.Except(existingPoolTypes).ToList();
                            if (missingPoolTypeIds.Any())
                                return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, $"PoolTypeNotFound: {string.Join(", ", missingPoolTypeIds)}");

                            pool.PoolTypeId = poolTypeIds.First();
                            
                            // If there are additional PoolTypeIds, create new pools for them
                            foreach (var poolTypeId in poolTypeIds.Skip(1))
                            {
                                var newPool = new Pool
                                {
                                    PoolTypeId = poolTypeId,
                                    ApplyOn = pool.ApplyOn,
                                    ApplyTo = pool.ApplyTo,
                                    PoolLimit = pool.PoolLimit,
                                    MemberCount = pool.MemberCount,
                                    PercentageOfMember = pool.PercentageOfMember,
                                    IsLimitExceed = pool.IsLimitExceed,
                                    Notes = pool.Notes
                                };
                                if (poolDto.ApplyOn.HasValue) newPool.ApplyOn = poolDto.ApplyOn.Value;
                                if (poolDto.ApplyTo.HasValue) newPool.ApplyTo = poolDto.ApplyTo;
                                if (poolDto.PoolLimit.HasValue) newPool.PoolLimit = poolDto.PoolLimit;
                                if (poolDto.MemberCount.HasValue) newPool.MemberCount = poolDto.MemberCount;
                                if (poolDto.PercentageOfMember.HasValue) newPool.PercentageOfMember = poolDto.PercentageOfMember;
                                if (poolDto.IsLimitExceed.HasValue) newPool.IsLimitExceed = poolDto.IsLimitExceed.Value;
                                if (poolDto.Notes != null) newPool.Notes = poolDto.Notes;
                                policy.Pools.Add(newPool);
                            }
                        }
                        
                        if (poolDto.ApplyOn.HasValue)
                            pool.ApplyOn = poolDto.ApplyOn.Value;
                        if (poolDto.ApplyTo.HasValue)
                            pool.ApplyTo = poolDto.ApplyTo;
                        if (poolDto.PoolLimit.HasValue)
                            pool.PoolLimit = poolDto.PoolLimit;
                        if (poolDto.MemberCount.HasValue)
                            pool.MemberCount = poolDto.MemberCount;
                        if (poolDto.PercentageOfMember.HasValue)
                            pool.PercentageOfMember = poolDto.PercentageOfMember;
                        if (poolDto.IsLimitExceed.HasValue)
                            pool.IsLimitExceed = poolDto.IsLimitExceed.Value;
                        if (poolDto.Notes != null)
                            pool.Notes = poolDto.Notes;
                    }
                    else
                    {
                        // Create new pool - requires PoolTypeIds and ApplyOn
                        if (poolDto.PoolTypeIds == null || !poolDto.PoolTypeIds.Any())
                            return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "PoolTypeIdsRequired");
                        if (!poolDto.ApplyOn.HasValue)
                            return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "ApplyOnRequired");

                        var poolTypeIds = poolDto.PoolTypeIds.Distinct().ToList();
                        var existingPoolTypes = await _context.PoolTypes
                            .Where(pt => poolTypeIds.Contains(pt.Id) && !pt.IsDeleted)
                            .Select(pt => pt.Id)
                            .ToListAsync(cancellationToken);

                        var missingPoolTypeIds = poolTypeIds.Except(existingPoolTypes).ToList();
                        if (missingPoolTypeIds.Any())
                            return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, $"PoolTypeNotFound: {string.Join(", ", missingPoolTypeIds)}");

                        // Create new pools - one for each PoolTypeId
                        foreach (var poolTypeId in poolTypeIds)
                        {
                            policy.Pools.Add(new Pool
                            {
                                PoolTypeId = poolTypeId,
                                ApplyOn = poolDto.ApplyOn.Value,
                                ApplyTo = poolDto.ApplyTo,
                                PoolLimit = poolDto.PoolLimit,
                                MemberCount = poolDto.MemberCount,
                                PercentageOfMember = poolDto.PercentageOfMember,
                                IsLimitExceed = poolDto.IsLimitExceed ?? false,
                                Notes = poolDto.Notes
                            });
                        }
                    }
                }
            }

            // Save pools first to get their IDs before processing ServiceClassDetails
            await _context.SaveChangesAsync(cancellationToken);

            // STEP 2: Update or create Programs (after pools are saved)
            if (dto.ListOfProgram != null && dto.ListOfProgram.Any())
            {
                foreach (var programDto in dto.ListOfProgram)
                {
                    GeneralProgram program;
                    if (programDto.Id.HasValue && existingProgramIds.Contains(programDto.Id.Value))
                    {
                        // Update existing program - only update non-null properties
                        program = policy.GeneralPrograms.First(gp => gp.Id == programDto.Id.Value);
                        
                        if (programDto.ProgramNameId.HasValue)
                        {
                            var programNameExists = await _context.Programs.AnyAsync(p => p.Id == programDto.ProgramNameId.Value && !p.IsDeleted, cancellationToken);
                            if (!programNameExists)
                                return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "ProgramNameNotFound");
                            program.ProgramNameId = programDto.ProgramNameId.Value;
                        }
                        
                        if (programDto.Limit.HasValue)
                            program.Limit = programDto.Limit;
                        if (programDto.RoomTypeId.HasValue)
                        {
                            var roomTypeExists = await _context.RoomTypes.AnyAsync(rt => rt.Id == programDto.RoomTypeId.Value && !rt.IsDeleted, cancellationToken);
                            if (!roomTypeExists)
                                return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "RoomTypeNotFound");
                            program.RoomTypeId = programDto.RoomTypeId;
                        }
                        if (programDto.Note != null)
                            program.Note = programDto.Note;

                        // Handle ServiceClassDetails
                        var existingScdIds = program.ServiceClassDetails.Select(scd => scd.Id).ToHashSet();

                        // Update or create ServiceClassDetails (no deletion)
                        if (programDto.ListOfServiceClasses != null && programDto.ListOfServiceClasses.Any())
                        {
                            foreach (var scdDto in programDto.ListOfServiceClasses)
                            {
                                ServiceClassDetail scd;
                                if (scdDto.Id.HasValue && existingScdIds.Contains(scdDto.Id.Value))
                                {
                                    // Update existing - only update non-null properties
                                    scd = program.ServiceClassDetails.First(s => s.Id == scdDto.Id.Value);
                                    
                                    if (scdDto.ServiceClassId.HasValue)
                                    {
                                        var serviceClassExists = await _context.ServiceClasses.AnyAsync(sc => sc.Id == scdDto.ServiceClassId.Value && !sc.IsDeleted, cancellationToken);
                                        if (!serviceClassExists)
                                            return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, $"ServiceClassNotFound: {scdDto.ServiceClassId.Value}");
                                        scd.ServiceClassId = scdDto.ServiceClassId.Value;
                                    }
                                    
                                    if (scdDto.ServiceLimitType.HasValue)
                                        scd.ServiceLimitType = scdDto.ServiceLimitType.Value;
                                    
                                    if (scdDto.PoolId.HasValue)
                                    {
                                        var poolExists = await _context.Pools.AnyAsync(p => p.Id == scdDto.PoolId.Value && p.PolicyId == id, cancellationToken);
                                        if (!poolExists)
                                            return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, $"PoolNotFound: {scdDto.PoolId.Value}");
                                        scd.PoolId = scdDto.PoolId;
                                    }
                                    
                                    // PoolIndex is not a property of ServiceClassDetail - removed
                                    if (scdDto.ServiceLimit.HasValue)
                                        scd.ServiceLimit = scdDto.ServiceLimit;
                                    if (scdDto.MemberCount.HasValue)
                                        scd.MemberCount = scdDto.MemberCount;
                                    if (scdDto.MemberPercentage.HasValue)
                                        scd.MemberPercentage = scdDto.MemberPercentage;
                                    if (scdDto.ApplyTo.HasValue)
                                        scd.ApplyTo = scdDto.ApplyTo;
                                    if (scdDto.Copayment.HasValue)
                                        scd.Copayment = scdDto.Copayment;
                                    if (scdDto.Notes != null)
                                        scd.Notes = scdDto.Notes;
                                    if (scdDto.OnlyRefund.HasValue)
                                        scd.OnlyRefund = scdDto.OnlyRefund.Value;
                                }
                                else
                                {
                                    // Create new - requires ServiceClassId and ServiceLimitType
                                    if (!scdDto.ServiceClassId.HasValue)
                                        return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "ServiceClassIdRequired");
                                    if (!scdDto.ServiceLimitType.HasValue)
                                        return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "ServiceLimitTypeRequired");
                                    
                                    var serviceClassExists = await _context.ServiceClasses.AnyAsync(sc => sc.Id == scdDto.ServiceClassId.Value && !sc.IsDeleted, cancellationToken);
                                    if (!serviceClassExists)
                                        return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, $"ServiceClassNotFound: {scdDto.ServiceClassId.Value}");

                                    if (scdDto.PoolId.HasValue)
                                    {
                                        var poolExists = await _context.Pools.AnyAsync(p => p.Id == scdDto.PoolId.Value && p.PolicyId == id, cancellationToken);
                                        if (!poolExists)
                                            return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, $"PoolNotFound: {scdDto.PoolId.Value}");
                                    }

                                    scd = new ServiceClassDetail
                                    {
                                        ServiceClassId = scdDto.ServiceClassId.Value,
                                        ServiceLimitType = scdDto.ServiceLimitType.Value,
                                        PoolId = scdDto.PoolId,
                                        ServiceLimit = scdDto.ServiceLimit,
                                        MemberCount = scdDto.MemberCount,
                                        MemberPercentage = scdDto.MemberPercentage,
                                        ApplyTo = scdDto.ApplyTo,
                                        Copayment = scdDto.Copayment,
                                        Notes = scdDto.Notes,
                                        OnlyRefund = scdDto.OnlyRefund ?? false
                                    };
                                    program.ServiceClassDetails.Add(scd);
                                }
                            }
                        }
                    }
                    else
                    {
                        // Create new program - requires ProgramNameId
                        if (!programDto.ProgramNameId.HasValue)
                            return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "ProgramNameIdRequired");
                        
                        var programNameExists = await _context.Programs.AnyAsync(p => p.Id == programDto.ProgramNameId.Value && !p.IsDeleted, cancellationToken);
                        if (!programNameExists)
                            return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "ProgramNameNotFound");

                        if (programDto.RoomTypeId.HasValue)
                        {
                            var roomTypeExists = await _context.RoomTypes.AnyAsync(rt => rt.Id == programDto.RoomTypeId.Value && !rt.IsDeleted, cancellationToken);
                            if (!roomTypeExists)
                                return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "RoomTypeNotFound");
                        }

                        program = new GeneralProgram
                        {
                            ProgramNameId = programDto.ProgramNameId.Value,
                            Limit = programDto.Limit,
                            RoomTypeId = programDto.RoomTypeId,
                            Note = programDto.Note
                        };

                        // Add ServiceClassDetails
                        if (programDto.ListOfServiceClasses != null && programDto.ListOfServiceClasses.Any())
                        {
                            foreach (var scdDto in programDto.ListOfServiceClasses)
                            {
                                if (!scdDto.ServiceClassId.HasValue)
                                    return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "ServiceClassIdRequired");
                                if (!scdDto.ServiceLimitType.HasValue)
                                    return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "ServiceLimitTypeRequired");
                                
                                var serviceClassExists = await _context.ServiceClasses.AnyAsync(sc => sc.Id == scdDto.ServiceClassId.Value && !sc.IsDeleted, cancellationToken);
                                if (!serviceClassExists)
                                    return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, $"ServiceClassNotFound: {scdDto.ServiceClassId.Value}");

                                if (scdDto.PoolId.HasValue)
                                {
                                    var poolExists = await _context.Pools.AnyAsync(p => p.Id == scdDto.PoolId.Value && p.PolicyId == id, cancellationToken);
                                    if (!poolExists)
                                        return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, $"PoolNotFound: {scdDto.PoolId.Value}");
                                }

                                program.ServiceClassDetails.Add(new ServiceClassDetail
                                {
                                    ServiceClassId = scdDto.ServiceClassId.Value,
                                    ServiceLimitType = scdDto.ServiceLimitType.Value,
                                    PoolId = scdDto.PoolId,
                                    ServiceLimit = scdDto.ServiceLimit,
                                    MemberCount = scdDto.MemberCount,
                                    MemberPercentage = scdDto.MemberPercentage,
                                    ApplyTo = scdDto.ApplyTo,
                                    Copayment = scdDto.Copayment,
                                    Notes = scdDto.Notes,
                                    OnlyRefund = scdDto.OnlyRefund ?? false
                                });
                            }
                        }

                        policy.GeneralPrograms.Add(program);
                    }
                }

                // No deletion - only update/create
            }

            // Update or create Reimbursements (RefundRules)
            if (dto.ListOfReimbursement != null && dto.ListOfReimbursement.Any())
            {
                foreach (var reimbursementDto in dto.ListOfReimbursement)
                {
                    RefundRule refundRule;
                    if (reimbursementDto.Id.HasValue && existingRefundRuleIds.Contains(reimbursementDto.Id.Value))
                    {
                        // Update existing - only update non-null properties
                        refundRule = policy.RefundRules.First(rr => rr.Id == reimbursementDto.Id.Value);
                        
                        if (reimbursementDto.ReimbursementTypeId.HasValue)
                        {
                            var reimbursementTypeExists = await _context.ReimbursementTypes.AnyAsync(rt => rt.Id == reimbursementDto.ReimbursementTypeId.Value && !rt.IsDeleted, cancellationToken);
                            if (!reimbursementTypeExists)
                                return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "ReimbursementTypeNotFound");
                            refundRule.ReimbursementTypeId = reimbursementDto.ReimbursementTypeId;
                        }
                        
                        if (reimbursementDto.ApplyOn.HasValue)
                            refundRule.ApplyOn = reimbursementDto.ApplyOn.Value;
                        
                        if (reimbursementDto.ProgramId.HasValue)
                        {
                            var programExists = await _context.Programs.AnyAsync(p => p.Id == reimbursementDto.ProgramId.Value && !p.IsDeleted, cancellationToken);
                            if (!programExists)
                                return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "ProgramNotFound");
                            refundRule.ProgramId = reimbursementDto.ProgramId;
                        }
                        
                        if (reimbursementDto.PricelistId.HasValue)
                        {
                            var pricelistExists = await _context.ProviderPriceLists.AnyAsync(pl => pl.Id == reimbursementDto.PricelistId.Value, cancellationToken);
                            if (!pricelistExists)
                                return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "PricelistNotFound");
                            refundRule.PricelistId = reimbursementDto.PricelistId;
                        }
                        
                        if (reimbursementDto.ApplyBy.HasValue)
                            refundRule.ApplyBy = reimbursementDto.ApplyBy.Value;
                        
                        if (reimbursementDto.MaxValue.HasValue)
                            refundRule.MaxValue = reimbursementDto.MaxValue;
                        if (reimbursementDto.ReimbursementPercentage.HasValue)
                            refundRule.ReimbursementPercentage = reimbursementDto.ReimbursementPercentage;
                        if (reimbursementDto.Notes != null)
                            refundRule.Notes = reimbursementDto.Notes;
                    }
                    else
                    {
                        // Create new - requires ApplyOn and ApplyBy
                        if (!reimbursementDto.ApplyOn.HasValue)
                            return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "ApplyOnRequired");
                        if (!reimbursementDto.ApplyBy.HasValue)
                            return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "ApplyByRequired");
                        
                        if (reimbursementDto.ReimbursementTypeId.HasValue)
                        {
                            var reimbursementTypeExists = await _context.ReimbursementTypes.AnyAsync(rt => rt.Id == reimbursementDto.ReimbursementTypeId.Value && !rt.IsDeleted, cancellationToken);
                            if (!reimbursementTypeExists)
                                return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "ReimbursementTypeNotFound");
                        }

                        if (reimbursementDto.ProgramId.HasValue)
                        {
                            var programExists = await _context.Programs.AnyAsync(p => p.Id == reimbursementDto.ProgramId.Value && !p.IsDeleted, cancellationToken);
                            if (!programExists)
                                return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "ProgramNotFound");
                        }

                        if (reimbursementDto.PricelistId.HasValue)
                        {
                            var pricelistExists = await _context.ProviderPriceLists.AnyAsync(pl => pl.Id == reimbursementDto.PricelistId.Value, cancellationToken);
                            if (!pricelistExists)
                                return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "PricelistNotFound");
                        }
                        // Create new - requires ApplyOn and ApplyBy
                        if (!reimbursementDto.ApplyOn.HasValue)
                            return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "ApplyOnRequired");
                        if (!reimbursementDto.ApplyBy.HasValue)
                            return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "ApplyByRequired");
                        
                        if (reimbursementDto.ReimbursementTypeId.HasValue)
                        {
                            var reimbursementTypeExists = await _context.ReimbursementTypes.AnyAsync(rt => rt.Id == reimbursementDto.ReimbursementTypeId.Value && !rt.IsDeleted, cancellationToken);
                            if (!reimbursementTypeExists)
                                return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "ReimbursementTypeNotFound");
                        }

                        if (reimbursementDto.ProgramId.HasValue)
                        {
                            var programExists = await _context.Programs.AnyAsync(p => p.Id == reimbursementDto.ProgramId.Value && !p.IsDeleted, cancellationToken);
                            if (!programExists)
                                return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "ProgramNotFound");
                        }

                        if (reimbursementDto.PricelistId.HasValue)
                        {
                            var pricelistExists = await _context.ProviderPriceLists.AnyAsync(pl => pl.Id == reimbursementDto.PricelistId.Value, cancellationToken);
                            if (!pricelistExists)
                                return ServiceResult<PolicyDto>.Fail(ServiceErrorType.Validation, "PricelistNotFound");
                        }
                        // Create new
                        refundRule = new RefundRule
                        {
                            ReimbursementTypeId = reimbursementDto.ReimbursementTypeId,
                            ApplyOn = reimbursementDto.ApplyOn.Value,
                            ProgramId = reimbursementDto.ProgramId,
                            PricelistId = reimbursementDto.PricelistId,
                            ApplyBy = reimbursementDto.ApplyBy.Value,
                            MaxValue = reimbursementDto.MaxValue,
                            ReimbursementPercentage = reimbursementDto.ReimbursementPercentage,
                            Notes = reimbursementDto.Notes
                        };
                        policy.RefundRules.Add(refundRule);
                    }
                }

                // No deletion - only update/create
            }

            await _context.SaveChangesAsync(cancellationToken);
            await SyncClientPolicyDatesAsync(policy, cancellationToken);

            return ServiceResult<PolicyDto>.Ok(MapDto(policy, lang));
        }

        public async Task<ServiceResult> DeleteAsync(int id, string lang, string currentUser, CancellationToken cancellationToken = default)
        {
            var policy = await _context.Policies.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);
            if (policy == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "PolicyNotFound");

            policy.IsDeleted = true;
            policy.UpdatedBy = currentUser;
            policy.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok();
        }

        private static PolicyDto MapDto(Policy policy, string lang)
        {
            var isArabic = lang == "ar";

            return new PolicyDto
            {
                Id = policy.Id,
                ClientId = policy.ClientId,
                ClientName = isArabic
                    ? policy.Client?.ArabicName ?? string.Empty
                    : policy.Client?.EnglishName ?? string.Empty,
                StartDate = DateOnly.FromDateTime(policy.StartDate),
                EndDate = DateOnly.FromDateTime(policy.EndDate),
                PolicyTypeId = policy.PolicyTypeId,
                PolicyTypeName = isArabic
                    ? policy.PolicyType?.NameAr ?? string.Empty
                    : policy.PolicyType?.NameEn ?? string.Empty,
                CarrierCompanyId = policy.CarrierCompanyId,
                CarrierCompanyName = isArabic
                    ? policy.CarrierCompany?.NameAr ?? string.Empty
                    : policy.CarrierCompany?.NameEn ?? string.Empty,
                TotalAmount = policy.TotalAmount
            };
        }

        private static PolicyUpdateDto MapToUpdateDto(Policy policy, string lang)
        {
            var isArabic = lang == "ar";
            
            var dto = new PolicyUpdateDto
            {
                PolicyTypeId = policy.PolicyTypeId,
                PolicyTypeName = isArabic
                    ? policy.PolicyType?.NameAr ?? string.Empty
                    : policy.PolicyType?.NameEn ?? string.Empty,
                CarrierCompanyId = policy.CarrierCompanyId,
                CarrierCompanyName = isArabic
                    ? policy.CarrierCompany?.NameAr ?? string.Empty
                    : policy.CarrierCompany?.NameEn ?? string.Empty,
                ClientId = policy.ClientId,
                ClientName = policy.Client != null
                    ? (isArabic ? policy.Client.ArabicName : policy.Client.EnglishName)
                    : string.Empty,
                StartDate = DateOnly.FromDateTime(policy.StartDate),
                EndDate = DateOnly.FromDateTime(policy.EndDate),
                IsCalculateUpperPeday = policy.IsCalculateUpperPeday,
                TotalAmount = policy.TotalAmount,
                WarningOnPercentage = policy.WarningOnPercentage,
                ListOfProgram = new List<ProgramDto>(),
                ListOfPool = new List<PoolDto>(),
                ListOfReimbursement = new List<ReimbursementDto>()
            };

            // Map Programs
            foreach (var program in policy.GeneralPrograms)
            {
                var programDto = new ProgramDto
                {
                    Id = program.Id,
                    ProgramNameId = program.ProgramNameId,
                    ProgramName = program.ProgramName != null
                        ? (isArabic ? program.ProgramName.NameAr : program.ProgramName.NameEn)
                        : string.Empty,
                    Limit = program.Limit,
                    RoomTypeId = program.RoomTypeId,
                    RoomTypeName = program.RoomType != null
                        ? (isArabic ? program.RoomType.NameAr : program.RoomType.NameEn)
                        : null,
                    Note = program.Note,
                    ListOfServiceClasses = new List<ServiceClassDetailDto>()
                };

                // Map ServiceClassDetails
                foreach (var scd in program.ServiceClassDetails)
                {
                    programDto.ListOfServiceClasses.Add(new ServiceClassDetailDto
                    {
                        Id = scd.Id,
                        ServiceClassId = scd.ServiceClassId,
                        ServiceClassName = scd.ServiceClass != null
                            ? (isArabic ? scd.ServiceClass.NameAr : scd.ServiceClass.NameEn)
                            : string.Empty,
                        ServiceLimitType = scd.ServiceLimitType,
                        PoolId = scd.PoolId,
                        PoolName = scd.Pool?.PoolType != null
                            ? (isArabic ? scd.Pool.PoolType.NameAr : scd.Pool.PoolType.NameEn)
                            : null,
                        ServiceLimit = scd.ServiceLimit,
                        MemberCount = scd.MemberCount,
                        MemberPercentage = scd.MemberPercentage,
                        ApplyTo = scd.ApplyTo,
                        Copayment = scd.Copayment,
                        Notes = scd.Notes,
                        OnlyRefund = scd.OnlyRefund
                    });
                }

                dto.ListOfProgram.Add(programDto);
            }

            // Map Pools - Group by configuration (same config but different PoolTypeIds)
            var poolGroups = policy.Pools
                .GroupBy(p => new
                {
                    p.ApplyOn,
                    p.ApplyTo,
                    p.PoolLimit,
                    p.MemberCount,
                    p.PercentageOfMember,
                    p.IsLimitExceed,
                    p.Notes
                })
                .ToList();

            foreach (var group in poolGroups)
            {
                var poolDto = new PoolDto
                {
                    PoolTypeIds = group.Select(p => p.PoolTypeId).ToList(),
                    PoolTypeNames = group
                        .Where(p => p.PoolType != null)
                        .Select(p => isArabic ? p.PoolType!.NameAr : p.PoolType!.NameEn)
                        .Distinct()
                        .ToList(),
                    ApplyOn = group.Key.ApplyOn,
                    ApplyTo = group.Key.ApplyTo,
                    PoolLimit = group.Key.PoolLimit,
                    MemberCount = group.Key.MemberCount,
                    PercentageOfMember = group.Key.PercentageOfMember,
                    IsLimitExceed = group.Key.IsLimitExceed,
                    Notes = group.Key.Notes
                };

                // If all pools in group have same ID (shouldn't happen, but handle it)
                if (group.Count() == 1)
                {
                    poolDto.Id = group.First().Id;
                }

                dto.ListOfPool.Add(poolDto);
            }

            // Map Reimbursements (RefundRules)
            foreach (var refundRule in policy.RefundRules)
            {
                dto.ListOfReimbursement.Add(new ReimbursementDto
                {
                    Id = refundRule.Id,
                    ReimbursementTypeId = refundRule.ReimbursementTypeId,
                    ReimbursementTypeName = refundRule.ReimbursementType != null
                        ? (isArabic ? refundRule.ReimbursementType.NameAr : refundRule.ReimbursementType.NameEn)
                        : null,
                    ApplyOn = refundRule.ApplyOn,
                    ProgramId = refundRule.ProgramId,
                    ProgramName = refundRule.Program != null
                        ? (isArabic ? refundRule.Program.NameAr : refundRule.Program.NameEn)
                        : null,
                    PricelistId = refundRule.PricelistId,
                    PricelistName = refundRule.Pricelist?.Name,
                    ApplyBy = refundRule.ApplyBy,
                    MaxValue = refundRule.MaxValue,
                    ReimbursementPercentage = refundRule.ReimbursementPercentage,
                    Notes = refundRule.Notes
                });
            }

            return dto;
        }

        private static PolicyStatus MapStatus(string code) => code switch
        {
            "a" => PolicyStatus.Active,
            "p" => PolicyStatus.Pending,
            "e" => PolicyStatus.Expired,
            _ => PolicyStatus.Pending
        };

        private static string MapStatus(PolicyStatus status) => status switch
        {
            PolicyStatus.Active => "a",
            PolicyStatus.Pending => "p",
            PolicyStatus.Expired => "e",
            _ => "p"
        };


        private async Task SyncClientPolicyDatesAsync(Policy policy, CancellationToken cancellationToken)
        {
            if (!policy.ClientId.HasValue)
                return;

            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == policy.ClientId.Value && !c.IsDeleted, cancellationToken);
            if (client == null)
                return;

            client.PolicyStart = policy.StartDate;
            client.PolicyExpire = policy.EndDate;
            client.ActivePolicyId = policy.Id;

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<ServiceResult<List<PolicyPaymentDto>>> GetPaymentsAsync(int policyId, CancellationToken cancellationToken = default)
        {
            var policyExists = await _context.Policies.AnyAsync(p => p.Id == policyId && !p.IsDeleted, cancellationToken);
            if (!policyExists)
                return ServiceResult<List<PolicyPaymentDto>>.Fail(ServiceErrorType.NotFound, "PolicyNotFound");

            var payments = await _context.PolicyPayments
                .AsNoTracking()
                .Where(pp => pp.PolicyId == policyId && !pp.IsDeleted)
                .OrderBy(pp => pp.PaymentDate)
                .Select(pp => new PolicyPaymentDto
                {
                    Id = pp.Id,
                    PolicyId = pp.PolicyId,
                    PaymentDate = DateOnly.FromDateTime(pp.PaymentDate),
                    PaymentValue = pp.PaymentValue,
                    ActualPaidValue = pp.ActualPaidValue,
                    ActualPaymentDate = pp.ActualPaymentDate.HasValue ? DateOnly.FromDateTime(pp.ActualPaymentDate.Value) : null,
                    Notes = pp.Notes
                })
                .ToListAsync(cancellationToken);

            return ServiceResult<List<PolicyPaymentDto>>.Ok(payments);
        }

        public async Task<ServiceResult<List<PolicyPaymentDto>>> SavePaymentsAsync(int policyId, PolicyPaymentCreateOrUpdateDto dto, string currentUser, CancellationToken cancellationToken = default)
        {
            var policy = await _context.Policies.FirstOrDefaultAsync(p => p.Id == policyId && !p.IsDeleted, cancellationToken);
            if (policy == null)
                return ServiceResult<List<PolicyPaymentDto>>.Fail(ServiceErrorType.NotFound, "PolicyNotFound");

            // Delete all existing payments for this policy
            var existingPayments = await _context.PolicyPayments
                .Where(pp => pp.PolicyId == policyId && !pp.IsDeleted)
                .ToListAsync(cancellationToken);

            _context.PolicyPayments.RemoveRange(existingPayments);

            // Create new payments from frontend data
            var newPayments = dto.Payments.Select(paymentDto => new PolicyPayment
            {
                PolicyId = policyId,
                PaymentDate = paymentDto.PaymentDate.ToDateTime(TimeOnly.MinValue),
                PaymentValue = paymentDto.PaymentValue,
                ActualPaidValue = paymentDto.ActualPaidValue,
                ActualPaymentDate = paymentDto.ActualPaymentDate?.ToDateTime(TimeOnly.MinValue),
                Notes = paymentDto.Notes,
                CreatedBy = currentUser,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            }).ToList();

            _context.PolicyPayments.AddRange(newPayments);
            await _context.SaveChangesAsync(cancellationToken);

            // Return saved payments
            var result = newPayments.Select(p => new PolicyPaymentDto
            {
                Id = p.Id,
                PolicyId = p.PolicyId,
                PaymentDate = DateOnly.FromDateTime(p.PaymentDate),
                PaymentValue = p.PaymentValue,
                ActualPaidValue = p.ActualPaidValue,
                ActualPaymentDate = p.ActualPaymentDate.HasValue ? DateOnly.FromDateTime(p.ActualPaymentDate.Value) : null,
                Notes = p.Notes
            }).ToList();

            return ServiceResult<List<PolicyPaymentDto>>.Ok(result);
        }

        public async Task<ServiceResult<List<PolicyPaymentDto>>> GeneratePaymentsAsync(int policyId, PolicyPaymentGenerateDto dto, string currentUser, CancellationToken cancellationToken = default)
        {
            var policy = await _context.Policies.FirstOrDefaultAsync(p => p.Id == policyId && !p.IsDeleted, cancellationToken);
            if (policy == null)
                return ServiceResult<List<PolicyPaymentDto>>.Fail(ServiceErrorType.NotFound, "PolicyNotFound");

            // Delete existing payments for this policy
            var existingPayments = await _context.PolicyPayments
                .Where(pp => pp.PolicyId == policyId && !pp.IsDeleted)
                .ToListAsync(cancellationToken);

            _context.PolicyPayments.RemoveRange(existingPayments);

            // Calculate payment amount per installment
            var paymentAmount = dto.TotalAmount / dto.NumberOfPayments;
            var paymentDate = dto.StartDate;

            // Generate payments
            var payments = new List<PolicyPayment>();
            for (int i = 0; i < dto.NumberOfPayments; i++)
            {
                var payment = new PolicyPayment
                {
                    PolicyId = policyId,
                    PaymentDate = paymentDate.ToDateTime(TimeOnly.MinValue),
                    PaymentValue = paymentAmount,
                    ActualPaidValue = 0,
                    ActualPaymentDate = null,
                    Notes = null,
                    CreatedBy = currentUser,
                    CreatedAt = DateTime.Now,
                    IsDeleted = false
                };
                payments.Add(payment);

                // Move to next payment date (monthly intervals)
                paymentDate = paymentDate.AddMonths(1);
            }

            _context.PolicyPayments.AddRange(payments);
            await _context.SaveChangesAsync(cancellationToken);

            var result = payments.Select(p => new PolicyPaymentDto
            {
                Id = p.Id,
                PolicyId = p.PolicyId,
                PaymentDate = DateOnly.FromDateTime(p.PaymentDate),
                PaymentValue = p.PaymentValue,
                ActualPaidValue = p.ActualPaidValue,
                ActualPaymentDate = p.ActualPaymentDate.HasValue ? DateOnly.FromDateTime(p.ActualPaymentDate.Value) : null,
                Notes = p.Notes
            }).ToList();

            return ServiceResult<List<PolicyPaymentDto>>.Ok(result);
        }

        public async Task<ServiceResult> DeletePaymentAsync(int paymentId, string currentUser, CancellationToken cancellationToken = default)
        {
            var payment = await _context.PolicyPayments
                .Include(pp => pp.Policy)
                .FirstOrDefaultAsync(pp => pp.Id == paymentId && !pp.IsDeleted, cancellationToken);

            if (payment == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "PaymentNotFound");

            if (payment.Policy?.IsDeleted == true)
                return ServiceResult.Fail(ServiceErrorType.Validation, "CannotDeletePaymentForDeletedPolicy");

            payment.IsDeleted = true;
            payment.UpdatedAt = DateTime.Now;
            payment.UpdatedBy = currentUser;

            await _context.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeletePoolAsync(int policyId, int poolId, string currentUser, CancellationToken cancellationToken = default)
        {
            var policy = await _context.Policies
                .Include(p => p.Pools)
                .FirstOrDefaultAsync(p => p.Id == policyId && !p.IsDeleted, cancellationToken);

            if (policy == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "PolicyNotFound");

            var pool = policy.Pools.FirstOrDefault(p => p.Id == poolId);
            if (pool == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "PoolNotFound");

            // Remove references from ServiceClassDetails before deleting the pool
            var serviceClassDetails = await _context.ServiceClassDetails
                .Where(scd => scd.PoolId == poolId)
                .ToListAsync(cancellationToken);

            foreach (var scd in serviceClassDetails)
            {
                scd.PoolId = null; // Remove the reference
            }

            _context.Pools.Remove(pool);
            await _context.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeleteProgramAsync(int policyId, int programId, string currentUser, CancellationToken cancellationToken = default)
        {
            var policy = await _context.Policies
                .Include(p => p.GeneralPrograms)
                .FirstOrDefaultAsync(p => p.Id == policyId && !p.IsDeleted, cancellationToken);

            if (policy == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "PolicyNotFound");

            var program = policy.GeneralPrograms.FirstOrDefault(gp => gp.Id == programId);
            if (program == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "ProgramNotFound");

            // Remove references from RefundRules before deleting the program
            var refundRules = await _context.RefundRules
                .Where(rr => rr.ProgramId == programId)
                .ToListAsync(cancellationToken);

            foreach (var refundRule in refundRules)
            {
                refundRule.ProgramId = null; // Remove the reference
            }

            // Also delete all ServiceClassDetails associated with this program
            var serviceClassDetails = await _context.ServiceClassDetails
                .Where(scd => scd.ProgramId == programId)
                .ToListAsync(cancellationToken);

            _context.ServiceClassDetails.RemoveRange(serviceClassDetails);

            _context.GeneralPrograms.Remove(program);
            await _context.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeleteServiceClassDetailAsync(int policyId, int serviceClassDetailId, string currentUser, CancellationToken cancellationToken = default)
        {
            var policy = await _context.Policies
                .Include(p => p.GeneralPrograms)
                    .ThenInclude(gp => gp.ServiceClassDetails)
                .FirstOrDefaultAsync(p => p.Id == policyId && !p.IsDeleted, cancellationToken);

            if (policy == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "PolicyNotFound");

            var serviceClassDetail = policy.GeneralPrograms
                .SelectMany(gp => gp.ServiceClassDetails)
                .FirstOrDefault(scd => scd.Id == serviceClassDetailId);

            if (serviceClassDetail == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "ServiceClassDetailNotFound");

            _context.ServiceClassDetails.Remove(serviceClassDetail);
            await _context.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeleteReimbursementAsync(int policyId, int reimbursementId, string currentUser, CancellationToken cancellationToken = default)
        {
            var policy = await _context.Policies
                .Include(p => p.RefundRules)
                .FirstOrDefaultAsync(p => p.Id == policyId && !p.IsDeleted, cancellationToken);

            if (policy == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "PolicyNotFound");

            var refundRule = policy.RefundRules.FirstOrDefault(rr => rr.Id == reimbursementId);
            if (refundRule == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "ReimbursementNotFound");

            _context.RefundRules.Remove(refundRule);
            await _context.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}

