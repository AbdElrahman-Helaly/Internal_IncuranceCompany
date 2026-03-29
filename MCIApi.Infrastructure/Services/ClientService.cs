using MCIApi.Application.Clients.DTOs;
using MCIApi.Application.Clients.Interfaces;
using MCIApi.Application.Common;
using MCIApi.Application.Common.Interfaces;
using MCIApi.Application.MemberInfos.DTOs;
using MCIApi.Domain.Abstractions;
using MCIApi.Domain.Entities;
using MCIApi.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace MCIApi.Infrastructure.Services
{
    public class ClientService : IClientService
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageService _imageService;

        public ClientService(AppDbContext context, IUnitOfWork unitOfWork, IImageService imageService)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _imageService = imageService;
        }

        public async Task<ServiceResult<ClientPagedResultDto>> GetAllAsync(string lang, int page, int limit, string? searchColumn = null, string? searchTerm = null, int? statusId = null, CancellationToken cancellationToken = default)
        {
            if (page < 1) page = 1;
            if (limit < 1) limit = 5;

            var baseQuery = _context.Clients
                .AsNoTracking()
                .Where(c => !c.IsDeleted);

            if (statusId.HasValue)
            {
                baseQuery = baseQuery.Where(c => c.StatusId == statusId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var normalizedTerm = searchTerm.Trim().ToLowerInvariant();
                baseQuery = ApplyClientSearch(baseQuery, searchColumn, normalizedTerm);
            }

            var totalClients = await baseQuery.CountAsync(cancellationToken);

            var clients = await baseQuery
                .OrderByDescending(c => c.Id)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(c => new ClientListItemDto
                {
                    Id = c.Id,
                    ArabicName = c.ArabicName ?? string.Empty,
                    EnglishName = c.EnglishName ?? string.Empty,
                    Category = c.Category != null ? c.Category.Name : string.Empty,
                    Type = c.Type != null
                        ? (lang == "ar" ? c.Type.NameAr : c.Type.NameEn)
                        : string.Empty,
                    Status = c.Status != null
                        ? (lang == "ar" ? c.Status.NameAr : c.Status.NameEn)
                        : string.Empty,
                    Branches = c.Branches.Count(b => !b.IsDeleted),
                    Members = c.Members.Count
                })
                .ToListAsync(cancellationToken);

            var dto = new ClientPagedResultDto
            {
                TotalClients = totalClients,
                CurrentPage = page,
                Limit = limit,
                TotalPages = (int)Math.Ceiling(totalClients / (double)limit),
                Data = clients
            };

            return ServiceResult<ClientPagedResultDto>.Ok(dto);
        }

        private static IQueryable<Client> ApplyClientSearch(IQueryable<Client> query, string? searchColumn, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchColumn))
                return ApplyWildcardSearch(query, searchTerm);

            var column = searchColumn.Trim().ToLowerInvariant();
            return column switch
            {
                "id" or "clientid" => ApplyIdSearch(query, searchTerm),
                "arabicname" or "arabic_name" => query.Where(c => c.ArabicName != null && c.ArabicName.ToLower().StartsWith(searchTerm)),
                "englishname" or "english_name" => query.Where(c => c.EnglishName != null && c.EnglishName.ToLower().StartsWith(searchTerm)),
                "shortname" or "short_name" => query.Where(c => c.ShortName != null && c.ShortName.ToLower().StartsWith(searchTerm)),
                "category" => query.Where(c => c.Category != null && c.Category.Name != null && c.Category.Name.ToLower().StartsWith(searchTerm)),
                "type" => query.Where(c =>
                    c.Type != null && (
                        (c.Type.NameAr != null && c.Type.NameAr.ToLower().StartsWith(searchTerm)) ||
                        (c.Type.NameEn != null && c.Type.NameEn.ToLower().StartsWith(searchTerm))
                    )),
                "status" => query.Where(c =>
                    c.Status != null && (
                        (c.Status.NameAr != null && c.Status.NameAr.ToLower().StartsWith(searchTerm)) ||
                        (c.Status.NameEn != null && c.Status.NameEn.ToLower().StartsWith(searchTerm))
                    )),
                _ => ApplyWildcardSearch(query, searchTerm)
            };
        }

        private static IQueryable<Client> ApplyIdSearch(IQueryable<Client> query, string searchTerm)
        {
            if (int.TryParse(searchTerm, out var id))
                return query.Where(c => c.Id == id);

            return query.Where(c => c.Id.ToString().StartsWith(searchTerm));
        }

        private static IQueryable<Client> ApplyWildcardSearch(IQueryable<Client> query, string searchTerm)
        {
            return query.Where(c =>
                c.Id.ToString().StartsWith(searchTerm) ||
                (c.ArabicName != null && c.ArabicName.ToLower().StartsWith(searchTerm)) ||
                (c.EnglishName != null && c.EnglishName.ToLower().StartsWith(searchTerm)) ||
                (c.ShortName != null && c.ShortName.ToLower().StartsWith(searchTerm)) ||
                (c.Category != null && c.Category.Name != null && c.Category.Name.ToLower().StartsWith(searchTerm)) ||
                (c.Type != null && (
                    (c.Type.NameAr != null && c.Type.NameAr.ToLower().StartsWith(searchTerm)) ||
                    (c.Type.NameEn != null && c.Type.NameEn.ToLower().StartsWith(searchTerm))
                )) ||
                (c.Status != null && (
                    (c.Status.NameAr != null && c.Status.NameAr.ToLower().StartsWith(searchTerm)) ||
                    (c.Status.NameEn != null && c.Status.NameEn.ToLower().StartsWith(searchTerm))
                )));
        }

        public async Task<ServiceResult<ClientDetailDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default)
        {
            var client = await _context.Clients
                .AsNoTracking()
                .Include(c => c.Category)
                .Include(c => c.Type)
                .Include(c => c.Status)
                .Include(c => c.Level)
                .Include(c => c.ActivePolicy)
                .Include(c => c.Contacts)
                .Include(c => c.Branches)
                    .ThenInclude(b => b.BranchStatus)
                .Include(c => c.Branches)
                    .ThenInclude(b => b.MemberInfos)
                .Include(c => c.Contracts)
                    .ThenInclude(contract => contract.InsuranceCompany)
                .Where(c => c.Id == id && !c.IsDeleted)
                .Select(c => new ClientDetailDto
                {
                    Id = c.Id,
                    ArabicName = c.ArabicName ?? string.Empty,
                    EnglishName = c.EnglishName ?? string.Empty,
                    ShortName = c.ShortName ?? string.Empty,
                    CategoryId = c.CategoryId,
                    CategoryName = c.Category != null ? c.Category.Name : string.Empty,
                    TypeId = c.TypeId,
                    TypeName = c.Type != null ? (lang == "ar" ? c.Type.NameAr : c.Type.NameEn) : string.Empty,
                    StatusId = c.StatusId,
                    StatusName = c.Status != null ? (lang == "ar" ? c.Status.NameAr : c.Status.NameEn) : string.Empty,
                    LevelId = c.LevelId,
                    LevelName = c.Level != null ? (lang == "ar" ? c.Level.NameAr : c.Level.NameEn) : string.Empty,
                    RefundDueDays = c.RefundDueDays,
                    PolicyId = c.ActivePolicyId,
                    PolicyStart = c.PolicyStart.HasValue ? c.PolicyStart.Value.ToString("yyyy-MM-dd") : null,
                    PolicyExpire = c.PolicyExpire.HasValue ? c.PolicyExpire.Value.ToString("yyyy-MM-dd") : null,
                    ImageUrl = string.IsNullOrEmpty(c.ImageUrl) ? null : c.ImageUrl,
                    Contacts = c.Contacts
                        .Where(contact => !contact.IsDeleted)
                        .Select(contact => new ClientContactDto
                        {
                            Id = contact.Id,
                            Name = contact.Name,
                            JobTitle = contact.JobTitle,
                            Email = contact.Email,
                            Mobile = contact.Mobile,
                            Address = contact.Address,
                            Note = contact.Note
                        }).ToList(),
                    Branches = c.Branches
                        .Where(b => !b.IsDeleted)
                        .Select(b => new ClientBranchDto
                        {
                            Id = b.Id,
                            BranchName = lang == "ar"
                                ? (b.ArabicName ?? b.EnglishName ?? string.Empty)
                                : (b.EnglishName ?? b.ArabicName ?? string.Empty),
                            BranchStatusId = b.BranchStatusId ?? 0,
                            BranchStatusName = b.BranchStatus != null
                                ? (lang == "ar" ? b.BranchStatus.NameAr : b.BranchStatus.NameEn)
                                : string.Empty,
                            MemberCount = b.MemberInfos.Count(mi => !mi.IsDeleted)
                        }).ToList(),
                    Contracts = c.Contracts
                        .Select(contract => new ClientContractInfoResponseDto
                        {
                            Id = contract.Id,
                            StartDate = contract.StartDate.ToString("yyyy-MM-dd"),
                            ExpireDate = contract.ExpireDate.ToString("yyyy-MM-dd"),
                            TotalAmount = contract.TotalAmount,
                            TotalMembers = contract.TotalMembers,
                            InsuranceCompanyId = contract.InsuranceCompanyId,
                            InsuranceCompanyName = contract.InsuranceCompany != null
                                ? (lang == "ar" ? contract.InsuranceCompany.ArName : contract.InsuranceCompany.EnName)
                                : string.Empty
                        }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (client == null)
                return ServiceResult<ClientDetailDto>.Fail(ServiceErrorType.NotFound, "ClientNotFound");

            return ServiceResult<ClientDetailDto>.Ok(client);
        }

        public async Task<ServiceResult<ClientCreateResultDto>> CreateAsync(ClientCreateDto dto, string lang, CancellationToken cancellationToken = default)
        {
            if (await _context.Clients.AnyAsync(c => !c.IsDeleted &&
                (c.ArabicName == dto.ArabicName || c.EnglishName == dto.EnglishName), cancellationToken))
            {
                return ServiceResult<ClientCreateResultDto>.Fail(ServiceErrorType.Conflict, "ClientNameExists");
            }

            if (!await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId, cancellationToken))
                return ServiceResult<ClientCreateResultDto>.Fail(ServiceErrorType.Validation, "CategoryNotFound");

            if (!await _context.ClientTypes.AnyAsync(t => t.Id == dto.ClientTypeId, cancellationToken))
                return ServiceResult<ClientCreateResultDto>.Fail(ServiceErrorType.Validation, "ClientTypeNotFound");

            if (!await _context.Statuses.AnyAsync(s => s.Id == dto.StatusId, cancellationToken))
                return ServiceResult<ClientCreateResultDto>.Fail(ServiceErrorType.Validation, "StatusNotFound");

            if (dto.PolicyId.HasValue)
            {
                var policyExists = await _context.Policies.AnyAsync(p => p.Id == dto.PolicyId.Value, cancellationToken);
                if (!policyExists)
                    return ServiceResult<ClientCreateResultDto>.Fail(ServiceErrorType.Validation, "PolicyNotFound");
            }

            if (dto.StartDate.HasValue && dto.EndDate.HasValue && dto.EndDate < dto.StartDate)
                return ServiceResult<ClientCreateResultDto>.Fail(ServiceErrorType.Validation, "PolicyExpireBeforeStart");

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            string? imagePath = null;
            List<ClientMemberSnapshot>? memberEntities = null;
            try
            {
                if (dto.ImageFile != null)
                    imagePath = await _imageService.SaveImageAsync(dto.ImageFile, "clients", cancellationToken);

                var client = new Client
                {
                    ArabicName = dto.ArabicName,
                    EnglishName = dto.EnglishName,
                    ShortName = dto.ShortName,
                    CategoryId = dto.CategoryId,
                    TypeId = dto.ClientTypeId,
                    StatusId = dto.StatusId,
                    RefundDueDays = dto.ReimbursementPerDays,
                    ActivePolicyId = dto.PolicyId,
                    PolicyStart = dto.StartDate?.ToDateTime(TimeOnly.MinValue),
                    PolicyExpire = dto.EndDate?.ToDateTime(TimeOnly.MinValue),
                    ImageUrl = imagePath,
                    IsDeleted = false
                };

                var repo = _unitOfWork.Repository<Client>();
                await repo.AddAsync(client, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var contactEntities = BuildContactEntities(dto.ContactUs, client.Id);
                if (contactEntities.Count > 0)
                    await _context.ClientContacts.AddRangeAsync(contactEntities, cancellationToken);

                var branchEntitiesResult = await BuildBranchEntitiesAsync(dto.Branches, client.Id, lang, cancellationToken);
                if (!branchEntitiesResult.Success)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    if (!string.IsNullOrEmpty(imagePath))
                        await _imageService.DeleteImageAsync(imagePath);
                    return ServiceResult<ClientCreateResultDto>.Fail(branchEntitiesResult.ErrorType, branchEntitiesResult.ErrorCode ?? "BranchStatusNotFound");
                }

                var branchEntities = branchEntitiesResult.Data!;
                if (branchEntities.Count > 0)
                    await _context.Branches.AddRangeAsync(branchEntities, cancellationToken);

                var contractEntitiesResult = await BuildContractEntitiesAsync(dto.Contracts, client.Id, cancellationToken);
                if (!contractEntitiesResult.Success)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    if (!string.IsNullOrEmpty(imagePath))
                        await _imageService.DeleteImageAsync(imagePath);
                    return ServiceResult<ClientCreateResultDto>.Fail(contractEntitiesResult.ErrorType, contractEntitiesResult.ErrorCode ?? "ContractDataInvalid");
                }

                var contractEntities = contractEntitiesResult.Data!;
                if (contractEntities.Count > 0)
                    await _context.ClientContractInfos.AddRangeAsync(contractEntities, cancellationToken);

                if (contactEntities.Count > 0 || branchEntities.Count > 0 || contractEntities.Count > 0)
                    await _context.SaveChangesAsync(cancellationToken);

                var memberEntitiesResult = await BuildMemberEntitiesAsync(dto.Members, client, branchEntities, lang, cancellationToken);
                if (!memberEntitiesResult.Success)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    if (!string.IsNullOrEmpty(imagePath))
                        await _imageService.DeleteImageAsync(imagePath);
                    return ServiceResult<ClientCreateResultDto>.Fail(memberEntitiesResult.ErrorType, memberEntitiesResult.ErrorCode ?? "MemberDataInvalid");
                }

                memberEntities = memberEntitiesResult.Data!;
                if (memberEntities.Count > 0)
                {
                    await _context.ClientMemberSnapshots.AddRangeAsync(memberEntities, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);

                    // Also create actual MemberInfo records so they appear in "get all members"
                    // Use dto.PolicyId if available, otherwise use client.ActivePolicyId
                    var policyIdForMembers = dto.PolicyId ?? client.ActivePolicyId;
                    var memberInfoResult = await CreateActualMemberInfosAsync(dto.Members, client, branchEntities, policyIdForMembers, lang, cancellationToken);
                    if (!memberInfoResult.Success)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        if (!string.IsNullOrEmpty(imagePath))
                            await _imageService.DeleteImageAsync(imagePath);
                        return ServiceResult<ClientCreateResultDto>.Fail(memberInfoResult.ErrorType, memberInfoResult.ErrorCode ?? "MemberInfoCreationFailed");
                    }
                }

                await transaction.CommitAsync(cancellationToken);

                return ServiceResult<ClientCreateResultDto>.Ok(new ClientCreateResultDto
                {
                    Id = client.Id,
                    Name = lang == "en" ? (client.EnglishName ?? string.Empty) : (client.ArabicName ?? string.Empty),
                    ShortName = client.ShortName,
                    CategoryId = client.CategoryId,
                    TypeId = client.TypeId,
                    StatusId = client.StatusId,
                    RefundDueDays = client.RefundDueDays
                });
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                if (!string.IsNullOrEmpty(imagePath))
                    await _imageService.DeleteImageAsync(imagePath);
                if (memberEntities != null)
                    await DeleteMemberImagesAsync(memberEntities, cancellationToken);
                throw;
            }
        }

        public async Task<ServiceResult<ClientUpdateResultDto>> UpdateAsync(int id, ClientUpdateDto dto, string lang, CancellationToken cancellationToken = default)
        {
            // Load client with tracking enabled
            var client = await _context.Clients
                .Include(c => c.ActivePolicy)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);

            if (client == null)
                return ServiceResult<ClientUpdateResultDto>.Fail(ServiceErrorType.NotFound, "ClientNotFound");

            // Validate client name uniqueness if names are being updated
            if ((!string.IsNullOrEmpty(dto.ArabicName) || !string.IsNullOrEmpty(dto.EnglishName)))
            {
                var nameExists = await _context.Clients
                    .AnyAsync(c => !c.IsDeleted && c.Id != id &&
                        (c.ArabicName == dto.ArabicName || c.EnglishName == dto.EnglishName), cancellationToken);
                
                if (nameExists)
                    return ServiceResult<ClientUpdateResultDto>.Fail(ServiceErrorType.Conflict, "ClientNameExists");
            }

            // Validate and update CategoryId
            if (dto.CategoryId.HasValue)
            {
                var categoryExists = await _context.Categories
                    .AnyAsync(c => c.Id == dto.CategoryId.Value, cancellationToken);
                if (!categoryExists)
                    return ServiceResult<ClientUpdateResultDto>.Fail(ServiceErrorType.Validation, "CategoryNotFound");
                client.CategoryId = dto.CategoryId.Value;
            }

            // Validate and update ClientTypeId
            if (dto.ClientTypeId.HasValue)
            {
                var typeExists = await _context.ClientTypes
                    .AnyAsync(t => t.Id == dto.ClientTypeId.Value, cancellationToken);
                if (!typeExists)
                    return ServiceResult<ClientUpdateResultDto>.Fail(ServiceErrorType.Validation, "ClientTypeNotFound");
                client.TypeId = dto.ClientTypeId.Value;
            }

            // Validate and update StatusId
            if (dto.StatusId.HasValue)
            {
                var statusExists = await _context.Statuses
                    .AnyAsync(s => s.Id == dto.StatusId.Value, cancellationToken);
                if (!statusExists)
                    return ServiceResult<ClientUpdateResultDto>.Fail(ServiceErrorType.Validation, "StatusNotFound");
                client.StatusId = dto.StatusId.Value;
            }

            // Validate and update LevelId
            if (dto.LevelId.HasValue)
            {
                var levelExists = await _context.MemberLevels
                    .AnyAsync(l => l.Id == dto.LevelId.Value && !l.IsDeleted, cancellationToken);
                if (!levelExists)
                    return ServiceResult<ClientUpdateResultDto>.Fail(ServiceErrorType.Validation, "LevelNotFound");
                client.LevelId = dto.LevelId.Value;
            }

            // Update basic client properties
            if (!string.IsNullOrEmpty(dto.ArabicName))
                client.ArabicName = dto.ArabicName;

            if (!string.IsNullOrEmpty(dto.EnglishName))
                client.EnglishName = dto.EnglishName;

            if (!string.IsNullOrEmpty(dto.ShortName))
                client.ShortName = dto.ShortName;

            if (dto.ReimbursementPerDays.HasValue)
                client.RefundDueDays = dto.ReimbursementPerDays.Value;

            // Validate and update PolicyId
            if (dto.PolicyId.HasValue)
            {
                var policyExists = await _context.Policies
                    .AnyAsync(p => p.Id == dto.PolicyId.Value, cancellationToken);
                if (!policyExists)
                    return ServiceResult<ClientUpdateResultDto>.Fail(ServiceErrorType.Validation, "PolicyNotFound");
                client.ActivePolicyId = dto.PolicyId.Value;
            }

            // Update policy dates
            if (dto.StartDate.HasValue || dto.EndDate.HasValue)
            {
                var start = dto.StartDate.HasValue 
                    ? dto.StartDate.Value.ToDateTime(TimeOnly.MinValue) 
                    : client.PolicyStart;
                var end = dto.EndDate.HasValue 
                    ? dto.EndDate.Value.ToDateTime(TimeOnly.MinValue) 
                    : client.PolicyExpire;

                if (start.HasValue && end.HasValue && end < start)
                    return ServiceResult<ClientUpdateResultDto>.Fail(ServiceErrorType.Validation, "PolicyExpireBeforeStart");

                client.PolicyStart = start;
                client.PolicyExpire = end;
            }

            // Handle image deletion
            if (dto.DeleteImage && !string.IsNullOrEmpty(client.ImageUrl))
            {
                await _imageService.DeleteImageAsync(client.ImageUrl);
                client.ImageUrl = null;
            }

            // Handle image upload
            if (dto.ImageFile != null)
            {
                if (!string.IsNullOrEmpty(client.ImageUrl))
                    await _imageService.DeleteImageAsync(client.ImageUrl);

                client.ImageUrl = await _imageService.SaveImageAsync(dto.ImageFile, "clients", cancellationToken);
            }

            // Handle branch updates
            if (dto.Branches != null && dto.Branches.Count > 0)
            {
                var branchUpdateResult = await UpdateBranchesAsync(id, dto.Branches, cancellationToken);
                if (!branchUpdateResult.Success)
                    return ServiceResult<ClientUpdateResultDto>.Fail(branchUpdateResult.ErrorType, branchUpdateResult.ErrorCode ?? "BranchUpdateFailed");
            }

            // Handle contact updates
            if (dto.ContactUs != null && dto.ContactUs.Count > 0)
            {
                var contactUpdateResult = await UpdateContactsAsync(id, dto.ContactUs, cancellationToken);
                if (!contactUpdateResult.Success)
                    return ServiceResult<ClientUpdateResultDto>.Fail(contactUpdateResult.ErrorType, contactUpdateResult.ErrorCode ?? "ContactUpdateFailed");
            }

            // Handle contract updates
            if (dto.Contracts != null && dto.Contracts.Count > 0)
            {
                var contractUpdateResult = await UpdateContractsAsync(id, dto.Contracts, cancellationToken);
                if (!contractUpdateResult.Success)
                    return ServiceResult<ClientUpdateResultDto>.Fail(contractUpdateResult.ErrorType, contractUpdateResult.ErrorCode ?? "ContractUpdateFailed");
            }

            // Ensure client entity is tracked and marked as modified
            // Client is already tracked from the initial query, but ensure it's marked as modified
            var clientEntry = _context.Entry(client);
            if (clientEntry.State == Microsoft.EntityFrameworkCore.EntityState.Unchanged)
            {
                clientEntry.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            
            // Save all changes (this will save client, branches, contacts, and contracts)
            var savedCount = await _context.SaveChangesAsync(cancellationToken);

            return ServiceResult<ClientUpdateResultDto>.Ok(new ClientUpdateResultDto
            {
                Message = "Client updated",
                Id = client.Id
            });
        }

        private async Task<ServiceResult> UpdateBranchesAsync(int clientId, List<ClientBranchDto> branchDtos, CancellationToken cancellationToken)
        {
            // Separate branches to update and create
            var branchesToUpdate = branchDtos.Where(b => b.Id.HasValue).ToList();
            var branchesToCreate = branchDtos.Where(b => !b.Id.HasValue).ToList();

            // Validate required fields only for NEW branches (not updates)
            foreach (var branchDto in branchesToCreate)
            {
                if (string.IsNullOrWhiteSpace(branchDto.BranchName))
                    return ServiceResult.Fail(ServiceErrorType.Validation, $"BranchNameRequired: BranchName is required for new branches");
                
                if (branchDto.BranchStatusId <= 0)
                    return ServiceResult.Fail(ServiceErrorType.Validation, $"BranchStatusIdRequired: BranchStatusId is required and must be greater than 0 for new branches");
            }

            // Validate BranchStatusId for branches that are being updated (only if provided)
            var statusIdsToValidate = branchDtos
                .Where(b => b.BranchStatusId > 0)
                .Select(b => b.BranchStatusId)
                .Distinct()
                .ToList();
            
            if (statusIdsToValidate.Any())
            {
                var validStatusIdsForUpdate = await _context.Statuses
                    .Where(s => statusIdsToValidate.Contains(s.Id))
                    .Select(s => s.Id)
                    .ToListAsync(cancellationToken);

                var invalidStatusIds = statusIdsToValidate.Except(validStatusIdsForUpdate).ToList();
                if (invalidStatusIds.Any())
                    return ServiceResult.Fail(ServiceErrorType.Validation, $"BranchStatusNotFound: Invalid status IDs: {string.Join(", ", invalidStatusIds)}");
            }

            // Update existing branches - load all at once for better performance
            // Note: Entities loaded without AsNoTracking() are automatically tracked
            if (branchesToUpdate.Any())
            {
                var branchIds = branchesToUpdate.Select(b => b.Id!.Value).ToList();
                var existingBranches = await _context.Branches
                    .Where(b => branchIds.Contains(b.Id) && b.ClientId == clientId && !b.IsDeleted)
                    .ToListAsync(cancellationToken);

                foreach (var branchDto in branchesToUpdate)
                {
                    var branch = existingBranches.FirstOrDefault(b => b.Id == branchDto.Id!.Value);

                    if (branch == null)
                        return ServiceResult.Fail(ServiceErrorType.Validation, $"BranchNotFound: Branch with ID {branchDto.Id!.Value} not found for this client");

                    // Update branch properties ONLY if provided (not null/empty)
                    if (!string.IsNullOrWhiteSpace(branchDto.BranchName))
                    {
                        branch.ArabicName = branchDto.BranchName;
                        branch.EnglishName = branchDto.BranchName;
                    }
                    
                    if (branchDto.BranchStatusId > 0)
                    {
                        branch.BranchStatusId = branchDto.BranchStatusId;
                    }
                    
                    // Note: MemberCount is not stored in Branch entity - it's calculated from MemberInfos collection
                    // The MemberCount from DTO is accepted for reference/validation purposes only
                    // If you need to validate member count, you can check: branch.MemberInfos.Count(m => !m.IsDeleted)
                    
                    // Only mark as modified if something actually changed
                    if (!string.IsNullOrWhiteSpace(branchDto.BranchName) || branchDto.BranchStatusId > 0)
                    {
                        _context.Entry(branch).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    }
                }
            }

            // Create new branches
            foreach (var branchDto in branchesToCreate)
            {
                var newBranch = new Branch
                {
                    ClientId = clientId,
                    ArabicName = branchDto.BranchName,
                    EnglishName = branchDto.BranchName,
                    BranchStatusId = branchDto.BranchStatusId,
                    Status = "Active",
                    IsDeleted = false
                    // Note: MemberCount is not stored in Branch entity - it's calculated from MemberInfos collection
                    // The MemberCount from DTO is accepted for reference/validation purposes only
                };
                await _context.Branches.AddAsync(newBranch, cancellationToken);
            }

            return ServiceResult.Ok();
        }

        private async Task<ServiceResult> UpdateContactsAsync(int clientId, List<ClientContactDto> contactDtos, CancellationToken cancellationToken)
        {
            // Separate contacts to update and create
            var contactsToUpdate = contactDtos.Where(c => c.Id.HasValue).ToList();
            var contactsToCreate = contactDtos.Where(c => !c.Id.HasValue).ToList();

            // Update existing contacts - load all at once for better performance
            // Note: Entities loaded without AsNoTracking() are automatically tracked
            var contactIds = contactsToUpdate.Select(c => c.Id!.Value).ToList();
            var existingContacts = await _context.ClientContacts
                .Where(c => contactIds.Contains(c.Id) && c.ClientId == clientId && !c.IsDeleted)
                .ToListAsync(cancellationToken);

            foreach (var contactDto in contactsToUpdate)
            {
                var contact = existingContacts.FirstOrDefault(c => c.Id == contactDto.Id!.Value);

                if (contact == null)
                    return ServiceResult.Fail(ServiceErrorType.Validation, $"ContactNotFound: Contact with ID {contactDto.Id!.Value} not found for this client");

                // Update contact properties ONLY if provided (not null/empty)
                bool hasChanges = false;
                
                if (!string.IsNullOrWhiteSpace(contactDto.Name))
                {
                    contact.Name = contactDto.Name;
                    hasChanges = true;
                }
                
                if (contactDto.JobTitle != null)
                {
                    contact.JobTitle = contactDto.JobTitle;
                    hasChanges = true;
                }
                
                if (contactDto.Email != null)
                {
                    contact.Email = contactDto.Email;
                    hasChanges = true;
                }
                
                if (contactDto.Mobile != null)
                {
                    contact.Mobile = contactDto.Mobile;
                    hasChanges = true;
                }
                
                if (contactDto.Address != null)
                {
                    contact.Address = contactDto.Address;
                    hasChanges = true;
                }
                
                if (contactDto.Note != null)
                {
                    contact.Note = contactDto.Note;
                    hasChanges = true;
                }

                // Only update timestamp and mark as modified if something changed
                if (hasChanges)
                {
                    contact.UpdatedAt = DateTime.Now;
                    contact.UpdatedBy = "System";
                    _context.Entry(contact).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
            }

            // Create new contacts
            foreach (var contactDto in contactsToCreate)
            {
                var newContact = new ClientContact
                {
                    ClientId = clientId,
                    Name = contactDto.Name,
                    JobTitle = contactDto.JobTitle,
                    Email = contactDto.Email,
                    Mobile = contactDto.Mobile,
                    Address = contactDto.Address,
                    Note = contactDto.Note,
                    CreatedBy = "System",
                    CreatedAt = DateTime.Now,
                    IsDeleted = false
                };
                await _context.ClientContacts.AddAsync(newContact, cancellationToken);
            }

            return ServiceResult.Ok();
        }

        private async Task<ServiceResult> UpdateContractsAsync(int clientId, List<ClientContractInfoDto> contractDtos, CancellationToken cancellationToken)
        {
            // Separate contracts to update and create
            var contractsToUpdate = contractDtos.Where(c => c.Id.HasValue).ToList();
            var contractsToCreate = contractDtos.Where(c => !c.Id.HasValue).ToList();

            // Validate required fields only for NEW contracts (not updates)
            foreach (var contractDto in contractsToCreate)
            {
                if (contractDto.InsuranceCompanyId <= 0)
                    return ServiceResult.Fail(ServiceErrorType.Validation, $"InsuranceCompanyIdRequired: InsuranceCompanyId is required for new contracts");
                
                if (contractDto.ExpireDate < contractDto.StartDate)
                    return ServiceResult.Fail(ServiceErrorType.Validation, $"ContractExpireBeforeStart: Contract expire date must be after start date");
            }

            // Validate InsuranceCompanyId for contracts that are being updated (only if provided)
            var insuranceCompanyIdsToValidate = contractDtos
                .Where(c => c.InsuranceCompanyId > 0)
                .Select(c => c.InsuranceCompanyId)
                .Distinct()
                .ToList();
            
            if (insuranceCompanyIdsToValidate.Any())
            {
                var validInsuranceCompanyIds = await _context.InsuranceCompanies
                    .Where(ic => insuranceCompanyIdsToValidate.Contains(ic.Id) && !ic.IsDeleted)
                    .Select(ic => ic.Id)
                    .ToListAsync(cancellationToken);

                var invalidInsuranceCompanyIds = insuranceCompanyIdsToValidate.Except(validInsuranceCompanyIds).ToList();
                if (invalidInsuranceCompanyIds.Any())
                    return ServiceResult.Fail(ServiceErrorType.Validation, $"InsuranceCompanyNotFound: Invalid insurance company IDs: {string.Join(", ", invalidInsuranceCompanyIds)}");
            }

            // Validate date ranges only if both dates are provided
            foreach (var contractDto in contractDtos)
            {
                if (contractDto.StartDate != default && contractDto.ExpireDate != default && contractDto.ExpireDate < contractDto.StartDate)
                    return ServiceResult.Fail(ServiceErrorType.Validation, $"ContractExpireBeforeStart: Contract expire date must be after start date");
            }

            // Update existing contracts - load all at once for better performance
            // Note: Entities loaded without AsNoTracking() are automatically tracked
            var contractIds = contractsToUpdate.Select(c => c.Id!.Value).ToList();
            var existingContracts = await _context.ClientContractInfos
                .Where(c => contractIds.Contains(c.Id) && c.ClientId == clientId)
                .ToListAsync(cancellationToken);

            foreach (var contractDto in contractsToUpdate)
            {
                var contract = existingContracts.FirstOrDefault(c => c.Id == contractDto.Id!.Value);

                if (contract == null)
                    return ServiceResult.Fail(ServiceErrorType.Validation, $"ContractNotFound: Contract with ID {contractDto.Id!.Value} not found for this client");

                // Update contract properties ONLY if provided (not default values)
                bool hasChanges = false;
                
                if (contractDto.StartDate != default)
                {
                    contract.StartDate = contractDto.StartDate.ToDateTime(TimeOnly.MinValue);
                    hasChanges = true;
                }
                
                if (contractDto.ExpireDate != default)
                {
                    contract.ExpireDate = contractDto.ExpireDate.ToDateTime(TimeOnly.MinValue);
                    hasChanges = true;
                }
                
                if (contractDto.TotalAmount > 0)
                {
                    contract.TotalAmount = contractDto.TotalAmount;
                    hasChanges = true;
                }
                
                if (contractDto.TotalMembers > 0)
                {
                    contract.TotalMembers = contractDto.TotalMembers;
                    hasChanges = true;
                }
                
                if (contractDto.InsuranceCompanyId > 0)
                {
                    contract.InsuranceCompanyId = contractDto.InsuranceCompanyId;
                    hasChanges = true;
                }

                // Only update timestamp and mark as modified if something changed
                if (hasChanges)
                {
                    contract.UpdatedAt = DateTime.Now;
                    _context.Entry(contract).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
            }

            // Create new contracts
            foreach (var contractDto in contractsToCreate)
            {
                var newContract = new ClientContractInfo
                {
                    ClientId = clientId,
                    StartDate = contractDto.StartDate.ToDateTime(TimeOnly.MinValue),
                    ExpireDate = contractDto.ExpireDate.ToDateTime(TimeOnly.MinValue),
                    TotalAmount = contractDto.TotalAmount,
                    TotalMembers = contractDto.TotalMembers,
                    InsuranceCompanyId = contractDto.InsuranceCompanyId,
                    CreatedAt = DateTime.Now
                };
                await _context.ClientContractInfos.AddAsync(newContract, cancellationToken);
            }

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<Client>();
            var client = await repo.GetByIdAsync(id, cancellationToken);
            if (client == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "ClientNotFound");

            client.IsDeleted = true;
            repo.Update(client);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeleteContactAsync(int clientId, int contactId, CancellationToken cancellationToken = default)
        {
            // Verify client exists
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.Id == clientId && !c.IsDeleted, cancellationToken);
            
            if (client == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "ClientNotFound");

            // Find the contact and verify it belongs to the client
            var contact = await _context.ClientContacts
                .FirstOrDefaultAsync(c => c.Id == contactId && c.ClientId == clientId && !c.IsDeleted, cancellationToken);

            if (contact == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "ContactNotFound");

            // Soft delete the contact
            contact.IsDeleted = true;
            contact.UpdatedAt = DateTime.Now;
            contact.UpdatedBy = "System";
            
            _context.ClientContacts.Update(contact);
            await _context.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeleteContractAsync(int clientId, int contractId, CancellationToken cancellationToken = default)
        {
            // Verify client exists
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.Id == clientId && !c.IsDeleted, cancellationToken);
            
            if (client == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "ClientNotFound");

            // Find the contract and verify it belongs to the client
            var contract = await _context.ClientContractInfos
                .FirstOrDefaultAsync(c => c.Id == contractId && c.ClientId == clientId, cancellationToken);

            if (contract == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "ContractNotFound");

            // Hard delete the contract (no IsDeleted field)
            _context.ClientContractInfos.Remove(contract);
            await _context.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> UpdateClientStatusAsync(int clientId, int statusId, CancellationToken cancellationToken = default)
        {
            var client = await _context.Clients
                .Include(c => c.Branches)
                .Include(c => c.Members)
                .FirstOrDefaultAsync(c => c.Id == clientId && !c.IsDeleted, cancellationToken);

            if (client == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "ClientNotFound");

            var statusExists = await _context.Statuses.AnyAsync(s => s.Id == statusId, cancellationToken);
            if (!statusExists)
                return ServiceResult.Fail(ServiceErrorType.Validation, "StatusNotFound");

            client.StatusId = statusId;
            _context.Clients.Update(client);

            if (statusId != 1)
            {
                foreach (var branch in client.Branches.Where(b => !b.IsDeleted))
                {
                    branch.BranchStatusId = statusId;
                    _context.Branches.Update(branch);
                }

                foreach (var member in client.Members)
                {
                    member.StatusId = statusId;
                    _context.ClientMemberSnapshots.Update(member);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok();
        }

        private async Task DeleteMemberImagesAsync(IEnumerable<ClientMemberSnapshot>? members, CancellationToken cancellationToken)
        {
            if (members == null)
                return;

            foreach (var member in members)
            {
                if (!string.IsNullOrEmpty(member.ImageUrl))
                    await _imageService.DeleteImageAsync(member.ImageUrl);
            }
        }

        public async Task<ServiceResult<IReadOnlyCollection<ClientLookupDto>>> GetAllStatusesAsync(string lang, CancellationToken cancellationToken = default)
        {
            var statuses = await _context.Statuses
                .AsNoTracking()
                .OrderBy(s => s.Id)
                .Select(s => new ClientLookupDto
                {
                    Id = s.Id,
                    Name = lang == "ar" ? s.NameAr : s.NameEn
                })
                .ToListAsync(cancellationToken);

            return ServiceResult<IReadOnlyCollection<ClientLookupDto>>.Ok(statuses);
        }

        public async Task<ServiceResult<IReadOnlyCollection<ClientLookupDto>>> GetAllTypesAsync(string lang, CancellationToken cancellationToken = default)
        {
            var types = await _context.ClientTypes
                .AsNoTracking()
                .OrderBy(t => t.Id)
                .Select(t => new ClientLookupDto
                {
                    Id = t.Id,
                    Name = lang == "ar" ? t.NameAr : t.NameEn
                })
                .ToListAsync(cancellationToken);

            return ServiceResult<IReadOnlyCollection<ClientLookupDto>>.Ok(types);
        }

        public async Task<ServiceResult<IReadOnlyCollection<ClientLookupDto>>> GetAllCategoriesAsync(string lang, CancellationToken cancellationToken = default)
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .Select(c => new ClientLookupDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync(cancellationToken);

            return ServiceResult<IReadOnlyCollection<ClientLookupDto>>.Ok(categories);
        }

        public async Task<ServiceResult<IReadOnlyCollection<ClientLookupDto>>> GetAllInsuranceCompaniesAsync(string lang, CancellationToken cancellationToken = default)
        {
            var companies = await _context.InsuranceCompanies
                .AsNoTracking()
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Id)
                .Select(c => new ClientLookupDto
                {
                    Id = c.Id,
                    Name = lang == "ar" ? c.ArName : c.EnName
                })
                .ToListAsync(cancellationToken);

            return ServiceResult<IReadOnlyCollection<ClientLookupDto>>.Ok(companies);
        }

        public async Task<ServiceResult<IReadOnlyCollection<ClientLookupDto>>> GetAllProgramsAsync(string lang, CancellationToken cancellationToken = default)
        {
            var programs = await _context.Programs
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .Select(p => new ClientLookupDto
                {
                    Id = p.Id,
                    Name = lang == "ar" ? p.NameAr : p.NameEn
                })
                .ToListAsync(cancellationToken);

            return ServiceResult<IReadOnlyCollection<ClientLookupDto>>.Ok(programs);
        }

        public async Task<ServiceResult<IReadOnlyCollection<ClientLookupDto>>> GetProgramsByClientIdAsync(int clientId, string lang, CancellationToken cancellationToken = default)
        {
            // Verify client exists
            var clientExists = await _context.Clients.AnyAsync(c => c.Id == clientId && !c.IsDeleted, cancellationToken);
            if (!clientExists)
            {
                return ServiceResult<IReadOnlyCollection<ClientLookupDto>>.Fail(ServiceErrorType.NotFound, "ClientNotFound");
            }

            // Get all unique programs from policies for this client
            var programs = await _context.Policies
                .AsNoTracking()
                .Where(p => p.ClientId == clientId && !p.IsDeleted)
                .SelectMany(p => p.GeneralPrograms)
                .Where(gp => gp.ProgramName != null && !gp.ProgramName.IsDeleted)
                .Select(gp => gp.ProgramName!)
                .Distinct()
                .OrderBy(p => p.Id)
                .Select(p => new ClientLookupDto
                {
                    Id = p.Id,
                    Name = lang == "ar" ? p.NameAr : p.NameEn
                })
                .ToListAsync(cancellationToken);

            return ServiceResult<IReadOnlyCollection<ClientLookupDto>>.Ok(programs);
        }

        public async Task<ServiceResult<IReadOnlyCollection<ClientLookupDto>>> GetAllMemberLevelsAsync(string lang, CancellationToken cancellationToken = default)
        {
            var levels = await _context.MemberLevels
                .AsNoTracking()
                .Where(level => !level.IsDeleted)
                .OrderBy(level => level.Id)
                .Select(level => new ClientLookupDto
                {
                    Id = level.Id,
                    Name = lang == "ar" ? level.NameAr : level.NameEn
                })
                .ToListAsync(cancellationToken);

            return ServiceResult<IReadOnlyCollection<ClientLookupDto>>.Ok(levels);
        }

        public async Task<ServiceResult<IReadOnlyCollection<ClientLookupDto>>> GetAllVipStatusesAsync(string lang, CancellationToken cancellationToken = default)
        {
            var statuses = await _context.VipStatuses
                .AsNoTracking()
                .Where(status => !status.IsDeleted)
                .OrderBy(status => status.Id)
                .Select(status => new ClientLookupDto
                {
                    Id = status.Id,
                    Name = lang == "ar" ? status.NameAr : status.NameEn
                })
                .ToListAsync(cancellationToken);

            return ServiceResult<IReadOnlyCollection<ClientLookupDto>>.Ok(statuses);
        }

        public async Task<ServiceResult<MemberInfoPagedResultDto>> GetMembersByClientIdAsync(int clientId, int page, int limit, string? searchColumn = null, string? search = null, int? statusId = null, string lang = "en", CancellationToken cancellationToken = default)
        {
            // Verify client exists
            var clientExists = await _context.Clients.AnyAsync(c => c.Id == clientId && !c.IsDeleted, cancellationToken);
            if (!clientExists)
            {
                return ServiceResult<MemberInfoPagedResultDto>.Fail(ServiceErrorType.NotFound, "ClientNotFound");
            }

            if (page < 1) page = 1;
            if (limit < 1) limit = 10;

            // Step 1: Get all branch IDs for this client
            var branchIds = await _context.Branches
                .Where(b => b.ClientId == clientId)
                .Select(b => b.Id)
                .ToListAsync(cancellationToken);

            // Step 2: If no branches exist, return empty result
            if (branchIds.Count == 0)
            {
                return ServiceResult<MemberInfoPagedResultDto>.Ok(new MemberInfoPagedResultDto
                {
                    TotalMembers = 0,
                    CurrentPage = page,
                    Limit = limit,
                    TotalPages = 0,
                    Data = Array.Empty<MemberInfoListItemDto>()
                });
            }

            // Step 3: Query all members in those branches
            var query = _context.MemberInfos
                .AsNoTracking()
                .Include(m => m.Status)
                .Include(m => m.Branch)
                    .ThenInclude(b => b!.Client)
                .Include(m => m.MemberPolicies.Where(p => !p.IsDeleted))
                    .ThenInclude(p => p.Policy)!.ThenInclude(p => p!.Client)
                .Include(m => m.MemberPolicies.Where(p => !p.IsDeleted))
                    .ThenInclude(p => p.Branch)
                .Include(m => m.MemberPolicies.Where(p => !p.IsDeleted))
                    .ThenInclude(p => p.Program)!
                        .ThenInclude(gp => gp.ProgramName)
                .Where(m => !m.IsDeleted && m.BranchId.HasValue && branchIds.Contains(m.BranchId.Value));

            // Filter by StatusId if provided
            if (statusId.HasValue)
            {
                query = query.Where(m => m.StatusId == statusId.Value);
            }

            // Apply search if provided
            if (!string.IsNullOrWhiteSpace(search))
            {
                var normalizedTerm = search.Trim().ToLowerInvariant();
                query = ApplyMemberSearchForClient(query, searchColumn, normalizedTerm, lang);
            }

            // Get total count
            var total = await query.CountAsync(cancellationToken);

            // Get paginated members
            var members = await query
                .OrderByDescending(m => m.Id)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync(cancellationToken);

            // Map to DTOs
            var data = members
                .Select(m => MapMemberListItem(m, lang))
                .ToList();

            var totalPages = (int)Math.Ceiling(total / (double)limit);

            return ServiceResult<MemberInfoPagedResultDto>.Ok(new MemberInfoPagedResultDto
            {
                TotalMembers = total,
                CurrentPage = page,
                Limit = limit,
                TotalPages = totalPages,
                Data = data
            });
        }

        private IQueryable<MemberInfo> ApplyMemberSearchForClient(IQueryable<MemberInfo> query, string? searchColumn, string normalizedTerm, string lang)
        {
            if (string.IsNullOrWhiteSpace(searchColumn))
            {
                // Search across all relevant columns
                var isArabic = lang == "ar";
                return query.Where(m =>
                    m.FullName.ToLower().StartsWith(normalizedTerm) ||
                    m.MobileNumber.ToLower().StartsWith(normalizedTerm) ||
                    (m.NationalId != null && m.NationalId.ToLower().StartsWith(normalizedTerm)) ||
                    (m.Branch != null && ((isArabic ? m.Branch.ArabicName : m.Branch.EnglishName) != null && (isArabic ? m.Branch.ArabicName! : m.Branch.EnglishName!).ToLower().StartsWith(normalizedTerm))) ||
                    (m.Status != null && ((isArabic ? m.Status.NameAr : m.Status.NameEn) != null && (isArabic ? m.Status.NameAr! : m.Status.NameEn!).ToLower().StartsWith(normalizedTerm)))
                );
            }

            var isArabicSearch = lang == "ar";
            return searchColumn.ToLowerInvariant() switch
            {
                "id" => query.Where(m => m.Id.ToString().StartsWith(normalizedTerm)),
                "name" or "membername" or "fullname" => query.Where(m => m.FullName.ToLower().StartsWith(normalizedTerm)),
                "mobile" or "mobilenumber" => query.Where(m => m.MobileNumber.ToLower().StartsWith(normalizedTerm)),
                "nationalid" => query.Where(m => m.NationalId != null && m.NationalId.ToLower().StartsWith(normalizedTerm)),
                "clientname" => query.Where(m => m.Branch != null && m.Branch.Client != null && 
                    ((isArabicSearch ? m.Branch.Client.ArabicName : m.Branch.Client.EnglishName) != null && 
                     (isArabicSearch ? m.Branch.Client.ArabicName! : m.Branch.Client.EnglishName!).ToLower().StartsWith(normalizedTerm))),
                "branchname" => query.Where(m => m.Branch != null && 
                    ((isArabicSearch ? m.Branch.ArabicName : m.Branch.EnglishName) != null && 
                     (isArabicSearch ? m.Branch.ArabicName! : m.Branch.EnglishName!).ToLower().StartsWith(normalizedTerm))),
                "statusname" or "status" => query.Where(m => m.Status != null && 
                    ((isArabicSearch ? m.Status.NameAr : m.Status.NameEn) != null && 
                     (isArabicSearch ? m.Status.NameAr! : m.Status.NameEn!).ToLower().StartsWith(normalizedTerm))),
                _ => query
            };
        }

        private static MemberInfoListItemDto MapMemberListItem(MemberInfo member, string lang)
        {
            var latestPolicy = member.MemberPolicies
                .Where(p => !p.IsDeleted)
                .OrderByDescending(p => p.AddDate)
                .FirstOrDefault();

            // Get ClientName: prefer from policy, fallback to branch's client
            var clientName = latestPolicy?.Policy?.Client != null
                ? (lang == "ar" ? latestPolicy.Policy.Client.ArabicName : latestPolicy.Policy.Client.EnglishName)
                : (member.Branch?.Client != null
                    ? (lang == "ar" ? member.Branch.Client.ArabicName : member.Branch.Client.EnglishName)
                    : null);

            // Get BranchName: prefer from policy, fallback to member's branch
            var branchName = latestPolicy?.Branch != null
                ? (lang == "ar" ? latestPolicy.Branch.ArabicName : latestPolicy.Branch.EnglishName)
                : (member.Branch != null
                    ? (lang == "ar" ? member.Branch.ArabicName : member.Branch.EnglishName)
                    : null);

            // Get ProgramName: only from policy (no direct relationship on MemberInfo)
            var programName = latestPolicy?.Program != null && latestPolicy.Program.ProgramName != null
                ? (lang == "ar" ? latestPolicy.Program.ProgramName.NameAr : latestPolicy.Program.ProgramName.NameEn)
                : null;

            return new MemberInfoListItemDto
            {
                Id = member.Id,
                MemberName = member.FullName,
                BirthDate = member.BirthDate?.ToString("yyyy-MM-dd"),
                Age = CalculateAge(member.BirthDate),
                ClientName = clientName,
                BranchName = branchName,
                ProgramName = programName,
                StatusName = member.Status != null ? (lang == "ar" ? member.Status.NameAr : member.Status.NameEn) : "Active",
                Mobile = member.MobileNumber
            };
        }

        public async Task<ServiceResult<IReadOnlyCollection<ClientLookupDto>>> GetAllClientsAsync(string lang, CancellationToken cancellationToken = default)
        {
            var clients = await _context.Clients
                .AsNoTracking()
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Id)
                .Select(c => new ClientLookupDto
                {
                    Id = c.Id,
                    Name = lang == "ar" ? (c.ArabicName ?? c.EnglishName ?? string.Empty) : (c.EnglishName ?? c.ArabicName ?? string.Empty)
                })
                .ToListAsync(cancellationToken);

            return ServiceResult<IReadOnlyCollection<ClientLookupDto>>.Ok(clients);
        }

        public async Task<ServiceResult<byte[]>> ExportToExcelAsync(string lang, string? searchColumn = null, string? searchTerm = null, int? statusId = null, CancellationToken cancellationToken = default)
        {
            var baseQuery = _context.Clients
                .AsNoTracking()
                .Include(c => c.Category)
                .Include(c => c.Type)
                .Include(c => c.Status)
                .Include(c => c.Branches)
                .Include(c => c.Members)
                .Where(c => !c.IsDeleted);

            if (statusId.HasValue)
            {
                baseQuery = baseQuery.Where(c => c.StatusId == statusId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var normalizedTerm = searchTerm.Trim().ToLowerInvariant();
                baseQuery = ApplyClientSearch(baseQuery, searchColumn, normalizedTerm);
            }

            var clients = await baseQuery
                .OrderByDescending(c => c.Id)
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
            var worksheet = package.Workbook.Worksheets.Add("Clients");

            // Set headers based on language
            var headers = lang == "ar"
                ? new[] { "الاسم العربي", "الاسم الإنجليزي", "الاسم المختصر", "الفئة", "النوع", "الحالة", "عدد الفروع", "عدد الأعضاء" }
                : new[] { "Arabic Name", "English Name", "Short Name", "Category", "Type", "Status", "Branches Count", "Members Count" };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // Add data rows
            for (int i = 0; i < clients.Count; i++)
            {
                var client = clients[i];
                var row = i + 2;

                worksheet.Cells[row, 1].Value = client.ArabicName ?? string.Empty;
                worksheet.Cells[row, 2].Value = client.EnglishName ?? string.Empty;
                worksheet.Cells[row, 3].Value = client.ShortName ?? string.Empty;
                worksheet.Cells[row, 4].Value = client.Category?.Name ?? string.Empty;
                worksheet.Cells[row, 5].Value = client.Type != null
                    ? (lang == "ar" ? client.Type.NameAr : client.Type.NameEn)
                    : string.Empty;
                worksheet.Cells[row, 6].Value = client.Status != null
                    ? (lang == "ar" ? client.Status.NameAr : client.Status.NameEn)
                    : string.Empty;
                worksheet.Cells[row, 7].Value = client.Branches.Count(b => !b.IsDeleted);
                worksheet.Cells[row, 8].Value = client.Members.Count;
            }

            // Auto-fit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var excelBytes = package.GetAsByteArray();
            return ServiceResult<byte[]>.Ok(excelBytes);
        }

        private static List<ClientContact> BuildContactEntities(IReadOnlyCollection<CreateClientContactDto> contacts, int clientId)
        {
            if (contacts == null || contacts.Count == 0)
                return new List<ClientContact>();

            return contacts
                .Where(contact => !string.IsNullOrWhiteSpace(contact.Name))
                .Select(contact => new ClientContact
                {
                    ClientId = clientId,
                    Name = contact.Name,
                    JobTitle = contact.JobTitle,
                    Email = contact.Email,
                    Mobile = contact.Mobile,
                    Address = contact.Address,
                    Note = contact.Note,
                    CreatedBy = "system"
                }).ToList();
        }

        private async Task<ServiceResult<List<Branch>>> BuildBranchEntitiesAsync(IReadOnlyCollection<CreateClientBranchDto> branches, int clientId, string lang, CancellationToken cancellationToken)
        {
            if (branches == null || branches.Count == 0)
                return ServiceResult<List<Branch>>.Ok(new List<Branch>());

            var sanitizedBranches = branches
                .Where(b => !string.IsNullOrWhiteSpace(b.BranchName))
                .ToList();

            if (sanitizedBranches.Count == 0)
                return ServiceResult<List<Branch>>.Ok(new List<Branch>());

            var statusIds = sanitizedBranches.Select(b => b.BranchStatusId).Distinct().ToList();
            var statusLookup = await _context.Statuses
                .Where(status => statusIds.Contains(status.Id))
                .ToDictionaryAsync(status => status.Id, status => status, cancellationToken);

            if (statusLookup.Count != statusIds.Count)
                return ServiceResult<List<Branch>>.Fail(ServiceErrorType.Validation, "BranchStatusNotFound");

            var branchEntities = sanitizedBranches.Select(dto =>
            {
                var normalizedName = dto.BranchName.Trim();
                var status = statusLookup[dto.BranchStatusId];
                return new Branch
                {
                    ClientId = clientId,
                    ArabicName = normalizedName,
                    EnglishName = normalizedName,
                    Status = lang == "ar" ? status.NameAr : status.NameEn,
                    BranchStatusId = status.Id
                    // Note: MemberCount is not stored in Branch entity - it's calculated from MemberInfos collection
                    // The MemberCount from DTO is accepted for reference/validation purposes only
                };
            }).ToList();

            return ServiceResult<List<Branch>>.Ok(branchEntities);
        }

        private async Task<ServiceResult<List<ClientContractInfo>>> BuildContractEntitiesAsync(IReadOnlyCollection<CreateClientContractInfoDto> contracts, int clientId, CancellationToken cancellationToken)
        {
            if (contracts == null || contracts.Count == 0)
                return ServiceResult<List<ClientContractInfo>>.Ok(new List<ClientContractInfo>());

            var sanitizedContracts = contracts.ToList();
            var insuranceIds = sanitizedContracts.Select(contract => contract.InsuranceCompanyId).Distinct().ToList();

            var insuranceLookup = await _context.InsuranceCompanies
                .Where(company => insuranceIds.Contains(company.Id) && !company.IsDeleted)
                .ToDictionaryAsync(company => company.Id, company => company, cancellationToken);

            if (insuranceLookup.Count != insuranceIds.Count)
                return ServiceResult<List<ClientContractInfo>>.Fail(ServiceErrorType.Validation, "InsuranceCompanyNotFound");

            foreach (var contract in sanitizedContracts)
            {
                if (contract.ExpireDate < contract.StartDate)
                    return ServiceResult<List<ClientContractInfo>>.Fail(ServiceErrorType.Validation, "ContractDatesInvalid");
            }

            var entities = sanitizedContracts.Select(contract => new ClientContractInfo
            {
                ClientId = clientId,
                StartDate = contract.StartDate.ToDateTime(TimeOnly.MinValue),
                ExpireDate = contract.ExpireDate.ToDateTime(TimeOnly.MinValue),
                TotalAmount = contract.TotalAmount,
                TotalMembers = contract.TotalMembers,
                InsuranceCompanyId = contract.InsuranceCompanyId
            }).ToList();

            return ServiceResult<List<ClientContractInfo>>.Ok(entities);
        }

        private async Task<ServiceResult<List<ClientMemberSnapshot>>> BuildMemberEntitiesAsync(
            IReadOnlyCollection<CreateClientMemberDto> members,
            Client client,
            IReadOnlyCollection<Branch> branches,
            string lang,
            CancellationToken cancellationToken)
        {
            if (members == null || members.Count == 0)
                return ServiceResult<List<ClientMemberSnapshot>>.Ok(new List<ClientMemberSnapshot>());

            var sanitizedMembers = members
                .Where(member => !string.IsNullOrWhiteSpace(member.Name))
                .ToList();

            if (sanitizedMembers.Count == 0)
                return ServiceResult<List<ClientMemberSnapshot>>.Ok(new List<ClientMemberSnapshot>());

            var memberStatusIds = sanitizedMembers
                .Select(m => m.StatusId)
                .Distinct()
                .ToList();

            var statusLookup = await _context.Statuses
                .Where(status => memberStatusIds.Contains(status.Id))
                .ToDictionaryAsync(status => status.Id, status => status, cancellationToken);

            if (statusLookup.Count != memberStatusIds.Count)
                return ServiceResult<List<ClientMemberSnapshot>>.Fail(ServiceErrorType.Validation, "MemberStatusNotFound");

            var levelIds = sanitizedMembers
                .Select(m => m.LevelId)
                .Distinct()
                .ToList();

            var levelLookup = await _context.MemberLevels
                .Where(level => levelIds.Contains(level.Id) && !level.IsDeleted)
                .ToDictionaryAsync(level => level.Id, level => level, cancellationToken);

            if (levelLookup.Count != levelIds.Count)
                return ServiceResult<List<ClientMemberSnapshot>>.Fail(ServiceErrorType.Validation, "MemberLevelNotFound");

            var vipIds = sanitizedMembers
                .Select(m => m.VipStatusId)
                .Distinct()
                .ToList();

            var vipLookup = await _context.VipStatuses
                .Where(vip => vipIds.Contains(vip.Id) && !vip.IsDeleted)
                .ToDictionaryAsync(vip => vip.Id, vip => vip, cancellationToken);

            if (vipLookup.Count != vipIds.Count)
                return ServiceResult<List<ClientMemberSnapshot>>.Fail(ServiceErrorType.Validation, "VipStatusNotFound");

            var clientNameFallback = client.EnglishName ?? client.ArabicName ?? string.Empty;
            var snapshotEntities = new List<ClientMemberSnapshot>();

            foreach (var member in sanitizedMembers)
            {
                var status = statusLookup[member.StatusId];
                var level = levelLookup[member.LevelId];
                var vipStatus = vipLookup[member.VipStatusId];

                snapshotEntities.Add(new ClientMemberSnapshot
                {
                    ClientId = client.Id,
                    BranchId = null,
                    BranchName = null,
                    Name = member.Name,
                    Birthday = member.Birthday?.ToDateTime(TimeOnly.MinValue),
                    Age = CalculateAge(member.Birthday?.ToDateTime(TimeOnly.MinValue)),
                    ClientName = clientNameFallback,
                    StatusName = lang == "ar" ? status.NameAr : status.NameEn,
                    StatusId = member.StatusId,
                    LevelId = member.LevelId,
                    LevelName = lang == "ar" ? level.NameAr : level.NameEn,
                    VipStatusId = member.VipStatusId,
                    VipStatusName = lang == "ar" ? vipStatus.NameAr : vipStatus.NameEn,
                    Mobile = member.Mobile,
                    IsMale = member.IsMale,
                    JobTitle = member.JobTitle,
                    NationalId = member.NationalId,
                    CompanyCode = member.CompanyCode,
                    HofCode = member.HofCode
                });
            }

            return ServiceResult<List<ClientMemberSnapshot>>.Ok(snapshotEntities);
        }

        private async Task<ServiceResult<bool>> CreateActualMemberInfosAsync(
            IReadOnlyCollection<CreateClientMemberDto> members,
            Client client,
            IReadOnlyCollection<Branch> branches,
            int? policyId,
            string lang,
            CancellationToken cancellationToken)
        {
            if (members == null || members.Count == 0)
                return ServiceResult<bool>.Ok(true);

            var sanitizedMembers = members
                .Where(member => !string.IsNullOrWhiteSpace(member.Name))
                .ToList();

            if (sanitizedMembers.Count == 0)
                return ServiceResult<bool>.Ok(true);

            // If no policy provided, try to find an active policy for this client
            if (!policyId.HasValue)
            {
                var activePolicy = await _context.Policies
                    .FirstOrDefaultAsync(p => p.ClientId == client.Id && 
                                              p.Status == Domain.Entities.PolicyStatus.Active && 
                                              !p.IsDeleted, cancellationToken);
                if (activePolicy != null)
                    policyId = activePolicy.Id;
            }

            // Get default branch (use first branch if available)
            // Note: BranchId is optional, but ClientId is always required
            var defaultBranchId = branches?.FirstOrDefault()?.Id;

            // Create a list of branches for index-based access
            var branchList = branches?.ToList() ?? new List<Branch>();

            // Get default relation (only needed if we're creating MemberPolicyInfo)
            Relation? defaultRelation = null;
            GeneralProgram? defaultProgram = null;
            
            if (policyId.HasValue)
            {
                defaultRelation = await _context.Relations.FirstOrDefaultAsync(cancellationToken);
                if (defaultRelation == null)
                    return ServiceResult<bool>.Fail(ServiceErrorType.Validation, "NoRelationFound");

                // Get default program (try to get first program from policy, or first available)
                defaultProgram = await _context.GeneralPrograms
                    .Where(p => p.PolicyId == policyId.Value)
                    .FirstOrDefaultAsync(cancellationToken) 
                    ?? await _context.GeneralPrograms.FirstOrDefaultAsync(cancellationToken);
                
                if (defaultProgram == null)
                    return ServiceResult<bool>.Fail(ServiceErrorType.Validation, "NoProgramFound");
            }

            // Get VIP status lookup
            var vipIds = sanitizedMembers.Select(m => m.VipStatusId).Distinct().ToList();
            var vipLookup = await _context.VipStatuses
                .Where(vip => vipIds.Contains(vip.Id) && !vip.IsDeleted)
                .ToDictionaryAsync(vip => vip.Id, vip => vip, cancellationToken);

            // Check for duplicate mobile numbers in the input list itself
            var mobileNumbers = sanitizedMembers
                .Where(m => !string.IsNullOrWhiteSpace(m.Mobile))
                .Select(m => m.Mobile!.Trim())
                .ToList();

            var duplicatesInInput = mobileNumbers
                .GroupBy(m => m)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicatesInInput.Count > 0)
            {
                var duplicateMobiles = string.Join(", ", duplicatesInInput);
                return ServiceResult<bool>.Fail(ServiceErrorType.Validation, $"DuplicateMobileNumberInInput: The following mobile numbers appear multiple times in the request: {duplicateMobiles}");
            }

            // Check for duplicate mobile numbers in the database (normalize comparison)
            // Get all existing mobile numbers and normalize them for comparison
            var allExistingMobiles = await _context.MemberInfos
                .Where(m => !m.IsDeleted)
                .Select(m => m.MobileNumber)
                .ToListAsync(cancellationToken);

            var normalizedExistingMobiles = allExistingMobiles
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .Select(m => m.Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var duplicateMobilesInDb = mobileNumbers
                .Where(m => normalizedExistingMobiles.Contains(m))
                .ToList();

            if (duplicateMobilesInDb.Count > 0)
            {
                var duplicateMobiles = string.Join(", ", duplicateMobilesInDb);
                return ServiceResult<bool>.Fail(ServiceErrorType.Conflict, $"DuplicateMobileNumber: The following mobile numbers already exist in the database: {duplicateMobiles}");
            }

            // Check for duplicate NationalIds in the input list itself
            var nationalIds = sanitizedMembers
                .Where(m => !string.IsNullOrWhiteSpace(m.NationalId))
                .Select(m => m.NationalId!.Trim())
                .ToList();

            var duplicatesNationalIdInInput = nationalIds
                .GroupBy(n => n)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicatesNationalIdInInput.Count > 0)
            {
                var duplicateNationalIds = string.Join(", ", duplicatesNationalIdInInput);
                return ServiceResult<bool>.Fail(ServiceErrorType.Validation, $"DuplicateNationalIdInInput: The following national IDs appear multiple times in the request: {duplicateNationalIds}");
            }

            // Check for duplicate NationalIds in the database (normalize comparison)
            var allExistingNationalIds = await _context.MemberInfos
                .Where(m => !m.IsDeleted)
                .Select(m => m.NationalId)
                .ToListAsync(cancellationToken);

            var normalizedExistingNationalIds = allExistingNationalIds
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n => n!.Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var duplicateNationalIdsInDb = nationalIds
                .Where(n => normalizedExistingNationalIds.Contains(n))
                .ToList();

            if (duplicateNationalIdsInDb.Count > 0)
            {
                var duplicateNationalIds = string.Join(", ", duplicateNationalIdsInDb);
                return ServiceResult<bool>.Fail(ServiceErrorType.Conflict, $"DuplicateNationalId: The following national IDs already exist in the database: {duplicateNationalIds}");
            }

            var memberInfos = new List<MemberInfo>();
            var memberPolicies = new List<MemberPolicyInfo>();

            foreach (var memberDto in sanitizedMembers)
            {
                // Determine which branch to assign this member to
                int memberBranchId;
                if (!string.IsNullOrWhiteSpace(memberDto.BranchName))
                {
                    // Find branch by name (match either ArabicName or EnglishName)
                    var branch = branchList.FirstOrDefault(b => 
                        b.ArabicName?.Trim().Equals(memberDto.BranchName.Trim(), StringComparison.OrdinalIgnoreCase) == true ||
                        b.EnglishName?.Trim().Equals(memberDto.BranchName.Trim(), StringComparison.OrdinalIgnoreCase) == true);
                    
                    if (branch == null)
                        return ServiceResult<bool>.Fail(ServiceErrorType.Validation, $"BranchName '{memberDto.BranchName}' not found. Available branches: {string.Join(", ", branchList.Select(b => b.EnglishName ?? b.ArabicName ?? "Unknown"))}");
                    
                    // Ensure branch belongs to the client
                    if (branch.ClientId != client.Id)
                        return ServiceResult<bool>.Fail(ServiceErrorType.Validation, $"Branch '{memberDto.BranchName}' does not belong to client {client.Id}");
                    
                    memberBranchId = branch.Id;
                }
                else
                {
                    // Use default branch (first branch) if available, otherwise null
                    memberBranchId = defaultBranchId ?? 0; // Will be set to null if no branch
                }
                
                // Validation: If BranchId is provided, it must belong to the client
                if (memberBranchId > 0)
                {
                    var branch = branchList.FirstOrDefault(b => b.Id == memberBranchId);
                    if (branch == null || branch.ClientId != client.Id)
                        return ServiceResult<bool>.Fail(ServiceErrorType.Validation, $"BranchId {memberBranchId} does not belong to client {client.Id}");
                }

                // Infer birthdate and gender from NationalId if provided
                DateTime? birthDate = memberDto.Birthday?.ToDateTime(TimeOnly.MinValue);
                bool? isMale = memberDto.IsMale;
                if (TryInferFromNationalId(memberDto.NationalId, out var inferredBirthDate, out var inferredIsMale))
                {
                    if (!birthDate.HasValue)
                        birthDate = inferredBirthDate;
                    if (!isMale.HasValue)
                        isMale = inferredIsMale;
                }

                // Get VIP status name
                var vipStatusName = "No";
                if (vipLookup.TryGetValue(memberDto.VipStatusId, out var vipStatus))
                {
                    vipStatusName = lang == "ar" ? vipStatus.NameAr : vipStatus.NameEn;
                }

                // Split name into first and last
                var nameParts = memberDto.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var firstName = nameParts.Length > 0 ? nameParts[0] : memberDto.Name;
                var lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : string.Empty;

                // Create MemberInfo
                // Rules:
                // - ClientId is always required
                // - BranchId is optional (nullable), but if provided, must belong to the client
                var memberInfo = new MemberInfo
                {
                    ClientId = client.Id, // ClientId is always required
                    BranchId = memberBranchId > 0 ? memberBranchId : null, // BranchId is optional (nullable), but if provided, must belong to the client
                    FullName = memberDto.Name,
                    FirstName = firstName,
                    LastName = lastName,
                    MiddleName = null,
                    MobileNumber = memberDto.Mobile?.Trim() ?? string.Empty,
                    StatusId = memberDto.StatusId,
                    JobTitle = memberDto.JobTitle,
                    BirthDate = birthDate,
                    IsMale = isMale,
                    NationalId = memberDto.NationalId,
                    Notes = null,
                    PrivateNotes = null,
                    VipStatus = vipStatusName,
                    ActivatedDate = memberDto.StatusId == 1 ? DateTime.Now : null,
                    CreatedBy = "system",
                    CreatedAt = DateTime.Now,
                    UpdatedBy = "system",
                    UpdatedAt = DateTime.Now,
                    IsDeleted = false
                };

                memberInfos.Add(memberInfo);
            }

            // Save MemberInfos first to get their IDs
            try
            {
                await _context.MemberInfos.AddRangeAsync(memberInfos, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
            {
                // Handle duplicate key constraint violation
                var errorMessage = sqlEx.Message;
                if (errorMessage.Contains("IX_MemberInfos_MobileNumber"))
                {
                    // Extract the duplicate mobile number from the error message if possible
                    var match = System.Text.RegularExpressions.Regex.Match(errorMessage, @"\(([^)]+)\)");
                    var duplicateMobile = match.Success ? match.Groups[1].Value : "unknown";
                    return ServiceResult<bool>.Fail(ServiceErrorType.Conflict, $"DuplicateMobileNumber: Mobile number '{duplicateMobile}' already exists in the database. Please use a different mobile number.");
                }
                if (errorMessage.Contains("IX_MemberInfos_NationalId"))
                {
                    // Extract the duplicate national ID from the error message if possible
                    var match = System.Text.RegularExpressions.Regex.Match(errorMessage, @"\(([^)]+)\)");
                    var duplicateNationalId = match.Success ? match.Groups[1].Value : "unknown";
                    return ServiceResult<bool>.Fail(ServiceErrorType.Conflict, $"DuplicateNationalId: National ID '{duplicateNationalId}' already exists in the database. Please use a different national ID.");
                }
                throw; // Re-throw if it's a different constraint violation
            }

            // Create MemberPolicyInfo records only if we have a policy
            if (policyId.HasValue && defaultRelation != null && defaultProgram != null)
            {
                for (int i = 0; i < memberInfos.Count; i++)
                {
                    var memberInfo = memberInfos[i];
                    var memberDto = sanitizedMembers[i];

                    var memberPolicy = new MemberPolicyInfo
                    {
                        MemberId = memberInfo.Id,
                        PolicyId = policyId.Value,
                        ProgramId = defaultProgram.Id,
                        BranchId = memberInfo.BranchId,
                        RelationId = defaultRelation.Id,
                        HeadOfFamilyId = !string.IsNullOrWhiteSpace(memberDto.HofCode) && int.TryParse(memberDto.HofCode, out var hofId) ? hofId : null,
                        CodeAtCompany = memberDto.CompanyCode,
                        JobTitle = memberDto.JobTitle,
                        Notes = null,
                        IsVip = memberDto.VipStatusId > 1, // Assuming 1 is "No"
                        CreatedBy = "system",
                        CreatedAt = DateTime.Now,
                        UpdatedBy = "system",
                        UpdatedAt = DateTime.Now,
                        IsDeleted = false
                    };

                    memberPolicies.Add(memberPolicy);
                }

                if (memberPolicies.Count > 0)
                {
                    await _context.MemberPolicyInfos.AddRangeAsync(memberPolicies, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
            // Note: If no policy exists, members are still created in MemberInfos table
            // but without MemberPolicyInfo. They can be linked to a policy later.

            return ServiceResult<bool>.Ok(true);
        }

        private static bool TryInferFromNationalId(string? nationalId, out DateTime? birthDate, out bool? isMale)
        {
            birthDate = null;
            isMale = null;

            if (string.IsNullOrWhiteSpace(nationalId) || nationalId.Length != 14)
                return false;

            try
            {
                // Egyptian National ID format: YYMMDDGNNNNNC
                // YY: Year (00-99, add 1900 or 2000)
                // MM: Month (01-12)
                // DD: Day (01-31)
                // G: Gender (1-9, odd = male, even = female)
                // NNNNN: Sequential number
                // C: Check digit

                var year = int.Parse(nationalId.Substring(1, 2));
                var month = int.Parse(nationalId.Substring(3, 2));
                var day = int.Parse(nationalId.Substring(5, 2));
                var genderDigit = int.Parse(nationalId.Substring(12, 1));

                // Determine century (rough heuristic: if year > 50, assume 1900s, else 2000s)
                var fullYear = year > 50 ? 1900 + year : 2000 + year;

                birthDate = new DateTime(fullYear, month, day);
                isMale = genderDigit % 2 == 1;

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static int? CalculateAge(DateTime? birthDate)
        {
            if (!birthDate.HasValue)
                return null;

            var today = DateTime.Now;
            var age = today.Year - birthDate.Value.Year;

            if (birthDate.Value.Date > today.AddYears(-age))
                age--;

            return age < 0 ? 0 : age;
        }
    }
}

