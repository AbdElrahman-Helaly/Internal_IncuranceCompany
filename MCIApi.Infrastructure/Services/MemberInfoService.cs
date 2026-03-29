using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MCIApi.Application.Common;
using MCIApi.Application.MemberInfos.DTOs;
using MCIApi.Application.MemberInfos.Interfaces;
using MCIApi.Application.Common.Interfaces;
using MCIApi.Application.Clients.DTOs;
using MCIApi.Domain.Entities;
using MCIApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace MCIApi.Infrastructure.Services
{
    public class MemberInfoService : IMemberInfoService
    {
        private readonly AppDbContext _context;
        private readonly IImageService _imageService;

        public MemberInfoService(AppDbContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public async Task<ServiceResult<MemberInfoPagedResultDto>> GetAllAsync(MemberInfoFilterDto filter, string lang, CancellationToken cancellationToken = default)
        {
            var page = filter.Page <= 0 ? 1 : filter.Page;
            var limit = filter.Limit <= 0 ? 10 : filter.Limit;

            var query = _context.MemberInfos
                .AsNoTracking()
                .Include(m => m.Status)
                .Include(m => m.Client)
                .Include(m => m.Branch)
                    .ThenInclude(b => b!.Client)
                .Include(m => m.Program)
                .Include(m => m.MemberPolicies.Where(p => !p.IsDeleted))
                    .ThenInclude(p => p.Policy)!.ThenInclude(p => p!.Client)
                .Include(m => m.MemberPolicies.Where(p => !p.IsDeleted))
                    .ThenInclude(p => p.Branch)
                .Include(m => m.MemberPolicies.Where(p => !p.IsDeleted))
                    .ThenInclude(p => p.Program)
                        .ThenInclude(gp => gp!.ProgramName)
                .Include(m => m.MemberPolicies.Where(p => !p.IsDeleted))
                    .ThenInclude(p => p.Relation)
                .Where(m => !m.IsDeleted);

            // Filter by StatusId if provided
            if (filter.StatusId.HasValue)
            {
                query = query.Where(m => m.StatusId == filter.StatusId.Value);
            }

            // Apply search if provided
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var normalizedTerm = filter.Search.Trim().ToLowerInvariant();
                query = ApplyMemberSearch(query, filter.SearchColumn, normalizedTerm, lang);
            }

            var total = await query.CountAsync(cancellationToken);
            var members = await query
                .OrderByDescending(m => m.Id)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync(cancellationToken);

            var data = members
                .Select(m => 
                {
                    try
                    {
                        return MapListItem(m, lang);
                    }
                    catch (Exception ex)
                    {
                        // Log and return a safe default if mapping fails
                        return new MemberInfoListItemDto
                        {
                            Id = m.Id,
                            MemberName = m.FullName,
                            BirthDate = m.BirthDate?.ToString("yyyy-MM-dd"),
                            Age = CalculateAge(m.BirthDate),
                            Mobile = m.MobileNumber
                        };
                    }
                })
                .ToList()
                .AsReadOnly();

            var dto = new MemberInfoPagedResultDto
            {
                TotalMembers = total,
                CurrentPage = page,
                Limit = limit,
                TotalPages = (int)Math.Ceiling(total / (double)limit),
                Data = data
            };

            return ServiceResult<MemberInfoPagedResultDto>.Ok(dto);
        }

        private static IQueryable<MemberInfo> ApplyMemberSearch(IQueryable<MemberInfo> query, string? searchColumn, string searchTerm, string lang)
        {
            if (string.IsNullOrWhiteSpace(searchColumn))
                return ApplyWildcardMemberSearch(query, searchTerm, lang);

            var column = searchColumn.Trim().ToLowerInvariant();
            var isArabic = lang == "ar";
            
            return column switch
            {
                "id" or "memberid" => ApplyMemberIdSearch(query, searchTerm),
                "name" or "fullname" => query.Where(m => m.FullName != null && m.FullName.ToLower().StartsWith(searchTerm)),
                "mobile" or "mobilenumber" => query.Where(m => m.MobileNumber != null && m.MobileNumber.StartsWith(searchTerm)),
                "clientname" or "client_name" => isArabic 
                    ? query.Where(m => m.MemberPolicies.Any(p => !p.IsDeleted && p.Policy != null && p.Policy.Client != null && p.Policy.Client.ArabicName != null && p.Policy.Client.ArabicName.ToLower().StartsWith(searchTerm)))
                    : query.Where(m => m.MemberPolicies.Any(p => !p.IsDeleted && p.Policy != null && p.Policy.Client != null && p.Policy.Client.EnglishName != null && p.Policy.Client.EnglishName.ToLower().StartsWith(searchTerm))),
                "branchname" or "branch_name" => isArabic
                    ? query.Where(m => m.MemberPolicies.Any(p => !p.IsDeleted && p.Branch != null && p.Branch.ArabicName != null && p.Branch.ArabicName.ToLower().StartsWith(searchTerm)))
                    : query.Where(m => m.MemberPolicies.Any(p => !p.IsDeleted && p.Branch != null && p.Branch.EnglishName != null && p.Branch.EnglishName.ToLower().StartsWith(searchTerm))),
                "programname" or "program_name" => isArabic
                    ? query.Where(m => m.MemberPolicies.Any(p => !p.IsDeleted && p.Program != null && p.Program.ProgramName != null && p.Program.ProgramName.NameAr != null && p.Program.ProgramName.NameAr.ToLower().StartsWith(searchTerm)))
                    : query.Where(m => m.MemberPolicies.Any(p => !p.IsDeleted && p.Program != null && p.Program.ProgramName != null && p.Program.ProgramName.NameEn != null && p.Program.ProgramName.NameEn.ToLower().StartsWith(searchTerm))),
                "statusname" or "status_name" => isArabic
                    ? query.Where(m => m.Status != null && m.Status.NameAr != null && m.Status.NameAr.ToLower().StartsWith(searchTerm))
                    : query.Where(m => m.Status != null && m.Status.NameEn != null && m.Status.NameEn.ToLower().StartsWith(searchTerm)),
                _ => ApplyWildcardMemberSearch(query, searchTerm, lang)
            };
        }

        private static IQueryable<MemberInfo> ApplyMemberIdSearch(IQueryable<MemberInfo> query, string searchTerm)
        {
            if (int.TryParse(searchTerm, out var id))
                return query.Where(m => m.Id == id);

            return query.Where(m => m.Id.ToString().StartsWith(searchTerm));
        }

        private static IQueryable<MemberInfo> ApplyWildcardMemberSearch(IQueryable<MemberInfo> query, string searchTerm, string lang)
        {
            var isArabic = lang == "ar";
            
            if (isArabic)
            {
                return query.Where(m =>
                    m.Id.ToString().StartsWith(searchTerm) ||
                    (m.FullName != null && m.FullName.ToLower().StartsWith(searchTerm)) ||
                    (m.MobileNumber != null && m.MobileNumber.StartsWith(searchTerm)) ||
                    (m.MemberPolicies.Any(p => !p.IsDeleted && p.Policy != null && p.Policy.Client != null && p.Policy.Client.ArabicName != null && p.Policy.Client.ArabicName.ToLower().StartsWith(searchTerm))) ||
                    (m.MemberPolicies.Any(p => !p.IsDeleted && p.Branch != null && p.Branch.ArabicName != null && p.Branch.ArabicName.ToLower().StartsWith(searchTerm))) ||
                    (m.MemberPolicies.Any(p => !p.IsDeleted && p.Program != null && p.Program.ProgramName != null && p.Program.ProgramName.NameAr != null && p.Program.ProgramName.NameAr.ToLower().StartsWith(searchTerm))) ||
                    (m.Status != null && m.Status.NameAr != null && m.Status.NameAr.ToLower().StartsWith(searchTerm)));
            }
            else
            {
                return query.Where(m =>
                    m.Id.ToString().StartsWith(searchTerm) ||
                    (m.FullName != null && m.FullName.ToLower().StartsWith(searchTerm)) ||
                    (m.MobileNumber != null && m.MobileNumber.StartsWith(searchTerm)) ||
                    (m.MemberPolicies.Any(p => !p.IsDeleted && p.Policy != null && p.Policy.Client != null && p.Policy.Client.EnglishName != null && p.Policy.Client.EnglishName.ToLower().StartsWith(searchTerm))) ||
                    (m.MemberPolicies.Any(p => !p.IsDeleted && p.Branch != null && p.Branch.EnglishName != null && p.Branch.EnglishName.ToLower().StartsWith(searchTerm))) ||
                    (m.MemberPolicies.Any(p => !p.IsDeleted && p.Program != null && p.Program.ProgramName != null && p.Program.ProgramName.NameAr != null && p.Program.ProgramName.NameAr.ToLower().StartsWith(searchTerm))) ||
                    (m.Status != null && m.Status.NameEn != null && m.Status.NameEn.ToLower().StartsWith(searchTerm)));
            }
        }

        public async Task<ServiceResult<MemberInfoDetailDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default)
        {
            var member = await _context.MemberInfos
                .AsNoTracking()
                .Include(m => m.Status)
                .Include(m => m.Client)
                .Include(m => m.Branch)
                .Include(m => m.Program)
                .Include(m => m.MemberPolicies.Where(p => !p.IsDeleted))
                    .ThenInclude(p => p.Policy)
                        .ThenInclude(p => p!.Client)
                .Include(m => m.MemberPolicies.Where(p => !p.IsDeleted))
                    .ThenInclude(p => p.Branch)
                .Include(m => m.MemberPolicies.Where(p => !p.IsDeleted))
                    .ThenInclude(p => p.Program)!
                        .ThenInclude(gp => gp.ProgramName)
                .Include(m => m.MemberPolicies.Where(p => !p.IsDeleted))
                    .ThenInclude(p => p.Relation)
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted, cancellationToken);

            if (member == null)
                return ServiceResult<MemberInfoDetailDto>.Fail(ServiceErrorType.NotFound, "MemberNotFound");

            var dto = MapDetail(member, lang);
            return ServiceResult<MemberInfoDetailDto>.Ok(dto);
        }

        public async Task<ServiceResult<MemberInfoDetailDto>> CreateAsync(MemberInfoCreateDto dto, string lang, string currentUser, CancellationToken cancellationToken = default)
        {
            // Validate ClientId
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.Id == dto.ClientId && !c.IsDeleted, cancellationToken);
            if (client == null)
                return ServiceResult<MemberInfoDetailDto>.Fail(ServiceErrorType.Validation, "ClientNotFound");

            // Validate BranchId (if provided, must belong to the client)
            if (dto.BranchId.HasValue)
            {
                var branch = await _context.Branches
                    .FirstOrDefaultAsync(b => b.Id == dto.BranchId.Value && !b.IsDeleted, cancellationToken);
                if (branch == null)
                    return ServiceResult<MemberInfoDetailDto>.Fail(ServiceErrorType.Validation, "BranchNotFound");
                if (branch.ClientId != dto.ClientId)
                    return ServiceResult<MemberInfoDetailDto>.Fail(ServiceErrorType.Validation, "BranchDoesNotBelongToClient");
            }

            // Validate VipStatusId
            var vipStatusExists = await _context.VipStatuses.AnyAsync(v => v.Id == dto.VipStatusId && !v.IsDeleted, cancellationToken);
            if (!vipStatusExists)
                return ServiceResult<MemberInfoDetailDto>.Fail(ServiceErrorType.Validation, "VipStatusNotFound");

            // Validate LevelId
            var levelExists = await _context.MemberLevels.AnyAsync(l => l.Id == dto.LevelId && !l.IsDeleted, cancellationToken);
            if (!levelExists)
                return ServiceResult<MemberInfoDetailDto>.Fail(ServiceErrorType.Validation, "LevelNotFound");

            // Validate ProgramId
            var programExists = await _context.Programs.AnyAsync(p => p.Id == dto.ProgramId && !p.IsDeleted, cancellationToken);
            if (!programExists)
                return ServiceResult<MemberInfoDetailDto>.Fail(ServiceErrorType.Validation, "ProgramNotFound");

            // Check for duplicate NationalId
            if (!string.IsNullOrWhiteSpace(dto.NationalId))
            {
                var exists = await _context.MemberInfos.AnyAsync(m => m.NationalId == dto.NationalId && !m.IsDeleted, cancellationToken);
                if (exists)
                    return ServiceResult<MemberInfoDetailDto>.Fail(ServiceErrorType.Conflict, "NationalIdExists");
            }

            // Check for duplicate Mobile
            var mobileExists = await _context.MemberInfos.AnyAsync(m => m.MobileNumber == dto.Mobile && !m.IsDeleted, cancellationToken);
            if (mobileExists)
                return ServiceResult<MemberInfoDetailDto>.Fail(ServiceErrorType.Conflict, "MobileExists");

            // Infer birthdate and gender from NationalId if provided
            if (TryInferFromNationalId(dto.NationalId, out var birthDate, out var isMale))
            {
                if (!dto.Birthday.HasValue && birthDate.HasValue)
                    dto.Birthday = DateOnly.FromDateTime(birthDate.Value);
                if (!dto.IsMale.HasValue)
                    dto.IsMale = isMale;
            }

            // Get VipStatus name for MemberInfo
            var vipStatus = await _context.VipStatuses.FirstOrDefaultAsync(v => v.Id == dto.VipStatusId, cancellationToken);
            var vipStatusName = vipStatus != null ? (lang == "ar" ? vipStatus.NameAr : vipStatus.NameEn) : "No";

            // Create MemberInfo
            // Rules:
            // - ClientId is always required
            // - BranchId is optional (nullable), but if provided, must belong to the client
            var member = new MemberInfo
            {
                ClientId = dto.ClientId, // ClientId is always required
                BranchId = dto.BranchId, // BranchId is optional (nullable), but if provided, must belong to the client
                ProgramId = dto.ProgramId, // ProgramId references Programs table
                FullName = dto.MemberName,
                FirstName = dto.MemberName.Split(' ').FirstOrDefault() ?? dto.MemberName,
                LastName = dto.MemberName.Split(' ').LastOrDefault() ?? string.Empty,
                MobileNumber = dto.Mobile,
                StatusId = 1, // 1 = Activated
                JobTitle = dto.JobTitle,
                BirthDate = dto.Birthday?.ToDateTime(TimeOnly.MinValue),
                IsMale = dto.IsMale,
                NationalId = dto.NationalId,
                Notes = dto.Notes,
                PrivateNotes = dto.PrivateNotes,
                VipStatus = vipStatusName,
                ActivatedDate = dto.ActivatedDate ?? DateTime.Now,
                CreatedBy = currentUser,
                CreatedAt = DateTime.Now
            };

            // Save image if provided
            if (dto.ImageFile is not null)
                member.MemberImage = await _imageService.SaveImageAsync(dto.ImageFile, "members", cancellationToken);

            _context.MemberInfos.Add(member);
            await _context.SaveChangesAsync(cancellationToken);

            var created = await _context.MemberInfos
                .Include(m => m.Status)
                .Include(m => m.Client)
                .Include(m => m.Branch)
                .Include(m => m.Program)
                .FirstOrDefaultAsync(m => m.Id == member.Id, cancellationToken);

            return ServiceResult<MemberInfoDetailDto>.Ok(MapDetail(created!, lang));
        }

        public async Task<ServiceResult<MemberInfoDetailDto>> UpdateAsync(int id, MemberInfoUpdateDto dto, string lang, string currentUser, CancellationToken cancellationToken = default)
        {
            var member = await _context.MemberInfos
                .Include(m => m.Status)
                .Include(m => m.MemberPolicies.Where(p => !p.IsDeleted))
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted, cancellationToken);

            if (member == null)
                return ServiceResult<MemberInfoDetailDto>.Fail(ServiceErrorType.NotFound, "MemberNotFound");

            if (!string.IsNullOrWhiteSpace(dto.NationalId) && dto.NationalId != member.NationalId)
            {
                var exists = await _context.MemberInfos.AnyAsync(m => m.NationalId == dto.NationalId && !m.IsDeleted, cancellationToken);
                if (exists)
                    return ServiceResult<MemberInfoDetailDto>.Fail(ServiceErrorType.Conflict, "NationalIdExists");
            }

            if (!string.IsNullOrWhiteSpace(dto.MobileNumber) && dto.MobileNumber != member.MobileNumber)
            {
                var mobileExists = await _context.MemberInfos.AnyAsync(m => m.MobileNumber == dto.MobileNumber && !m.IsDeleted, cancellationToken);
                if (mobileExists)
                    return ServiceResult<MemberInfoDetailDto>.Fail(ServiceErrorType.Conflict, "MobileExists");
            }

            if (dto.BranchId.HasValue && dto.BranchId.Value != member.BranchId)
            {
                var branchExists = await _context.Branches.AnyAsync(b => b.Id == dto.BranchId.Value && !b.IsDeleted, cancellationToken);
                if (!branchExists)
                    return ServiceResult<MemberInfoDetailDto>.Fail(ServiceErrorType.Validation, "BranchNotFound");
                member.BranchId = dto.BranchId.Value;
            }

            if (TryInferFromNationalId(dto.NationalId, out var birthDate, out var isMale))
            {
                if (birthDate.HasValue)
                    member.BirthDate = birthDate;
                member.IsMale = isMale;
            }

            member.FirstName = dto.FirstName ?? member.FirstName;
            member.MiddleName = dto.MiddleName ?? member.MiddleName;
            member.LastName = dto.LastName ?? member.LastName;
            member.FullName = dto.FullName ?? member.FullName;
            
            if (dto.StatusId.HasValue)
            {
                var statusExists = await _context.Statuses.AnyAsync(s => s.Id == dto.StatusId.Value, cancellationToken);
                if (!statusExists)
                    return ServiceResult<MemberInfoDetailDto>.Fail(ServiceErrorType.Validation, "StatusNotFound");
                member.StatusId = dto.StatusId.Value;
            }
            
            member.JobTitle = dto.JobTitle ?? member.JobTitle;
            member.MobileNumber = dto.MobileNumber ?? member.MobileNumber;
            member.NationalId = dto.NationalId ?? member.NationalId;
            member.Notes = dto.Notes ?? member.Notes;
            member.PrivateNotes = dto.PrivateNotes ?? member.PrivateNotes;
            member.VipStatus = dto.VipStatus ?? member.VipStatus;
            member.BirthDate = dto.BirthDate?.ToDateTime(TimeOnly.MinValue) ?? member.BirthDate;
            member.IsMale = dto.IsMale ?? member.IsMale;

            if (member.StatusId == 1 && member.ActivatedDate is null)
                member.ActivatedDate = DateTime.Now;

            if (dto.DeleteImage && !string.IsNullOrEmpty(member.MemberImage))
            {
                await _imageService.DeleteImageAsync(member.MemberImage);
                member.MemberImage = null;
            }

            if (dto.MemberImage is not null)
            {
                if (!string.IsNullOrEmpty(member.MemberImage))
                    await _imageService.DeleteImageAsync(member.MemberImage);
                member.MemberImage = await _imageService.SaveImageAsync(dto.MemberImage, "members", cancellationToken);
            }

            member.UpdatedBy = currentUser;
            member.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync(cancellationToken);

            var updated = await _context.MemberInfos
                .Include(m => m.MemberPolicies.Where(p => !p.IsDeleted))
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

            return ServiceResult<MemberInfoDetailDto>.Ok(MapDetail(updated!, lang));
        }

        public async Task<ServiceResult> DeleteAsync(int id, string lang, string currentUser, CancellationToken cancellationToken = default)
        {
            var member = await _context.MemberInfos.FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted, cancellationToken);
            if (member == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "MemberNotFound");

            member.IsDeleted = true;
            member.UpdatedBy = currentUser;
            member.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> UpdateMemberStatusAsync(int memberId, int statusId, CancellationToken cancellationToken = default)
        {
            var member = await _context.MemberInfos.FirstOrDefaultAsync(m => m.Id == memberId && !m.IsDeleted, cancellationToken);
            if (member == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "MemberNotFound");

            var statusExists = await _context.Statuses.AnyAsync(s => s.Id == statusId, cancellationToken);
            if (!statusExists)
                return ServiceResult.Fail(ServiceErrorType.Validation, "StatusNotFound");

            member.StatusId = statusId;
            if (statusId == 1 && member.ActivatedDate == null)
                member.ActivatedDate = DateTime.Now;

            _context.MemberInfos.Update(member);
            await _context.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult<IReadOnlyCollection<ClientLookupDto>>> GetBranchesByClientIdAsync(int clientId, string lang, CancellationToken cancellationToken = default)
        {
            var branches = await _context.Branches
                .AsNoTracking()
                .Where(b => b.ClientId == clientId && !b.IsDeleted)
                .OrderBy(b => b.Id)
                .Select(b => new ClientLookupDto
                {
                    Id = b.Id,
                    Name = lang == "ar" ? (b.ArabicName ?? b.EnglishName ?? string.Empty) : (b.EnglishName ?? b.ArabicName ?? string.Empty)
                })
                .ToListAsync(cancellationToken);

            return ServiceResult<IReadOnlyCollection<ClientLookupDto>>.Ok(branches);
        }


        public async Task<ServiceResult<BulkOperationResultDto>> BulkActivateMembersAsync(List<int> memberIds, string currentUser, CancellationToken cancellationToken = default)
        {
            var result = new BulkOperationResultDto();
            const int activatedStatusId = 1; // Assuming 1 is Activated

            var members = await _context.MemberInfos
                .Where(m => memberIds.Contains(m.Id) && !m.IsDeleted)
                .ToListAsync(cancellationToken);

            foreach (var member in members)
            {
                try
                {
                    member.StatusId = activatedStatusId;
                    if (member.ActivatedDate == null)
                        member.ActivatedDate = DateTime.Now;
                    member.UpdatedBy = currentUser;
                    member.UpdatedAt = DateTime.Now;
                    result.SuccessIds.Add(member.Id);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.Errors.Add(new BulkOperationErrorDto
                    {
                        MemberId = member.Id,
                        ErrorMessage = ex.Message
                    });
                    result.FailureCount++;
                }
            }

            // Add errors for members not found
            var foundIds = members.Select(m => m.Id).ToList();
            var notFoundIds = memberIds.Except(foundIds).ToList();
            foreach (var id in notFoundIds)
            {
                result.Errors.Add(new BulkOperationErrorDto
                {
                    MemberId = id,
                    ErrorMessage = "Member not found"
                });
                result.FailureCount++;
            }

            if (result.SuccessCount > 0)
            {
                await _context.SaveChangesAsync(cancellationToken);
            }

            return ServiceResult<BulkOperationResultDto>.Ok(result);
        }

        public async Task<ServiceResult<BulkOperationResultDto>> BulkDeactivateMembersAsync(List<int> memberIds, string currentUser, CancellationToken cancellationToken = default)
        {
            var result = new BulkOperationResultDto();
            const int deactivatedStatusId = 2; // Assuming 2 is Deactivated

            var members = await _context.MemberInfos
                .Where(m => memberIds.Contains(m.Id) && !m.IsDeleted)
                .ToListAsync(cancellationToken);

            foreach (var member in members)
            {
                try
                {
                    member.StatusId = deactivatedStatusId;
                    member.UpdatedBy = currentUser;
                    member.UpdatedAt = DateTime.Now;
                    result.SuccessIds.Add(member.Id);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.Errors.Add(new BulkOperationErrorDto
                    {
                        MemberId = member.Id,
                        ErrorMessage = ex.Message
                    });
                    result.FailureCount++;
                }
            }

            // Add errors for members not found
            var foundIds = members.Select(m => m.Id).ToList();
            var notFoundIds = memberIds.Except(foundIds).ToList();
            foreach (var id in notFoundIds)
            {
                result.Errors.Add(new BulkOperationErrorDto
                {
                    MemberId = id,
                    ErrorMessage = "Member not found"
                });
                result.FailureCount++;
            }

            if (result.SuccessCount > 0)
            {
                await _context.SaveChangesAsync(cancellationToken);
            }

            return ServiceResult<BulkOperationResultDto>.Ok(result);
        }

        public async Task<ServiceResult<BulkOperationResultDto>> BulkUpdateMembersAsync(BulkMemberUpdateDto dto, string lang, string currentUser, CancellationToken cancellationToken = default)
        {
            var result = new BulkOperationResultDto();
            var memberIds = dto.Members.Select(m => m.MemberId).Distinct().ToList();

            var members = await _context.MemberInfos
                .Where(m => memberIds.Contains(m.Id) && !m.IsDeleted)
                .ToListAsync(cancellationToken);

            var memberDict = members.ToDictionary(m => m.Id);

            foreach (var updateDto in dto.Members)
            {
                if (!memberDict.TryGetValue(updateDto.MemberId, out var member))
                {
                    result.Errors.Add(new BulkOperationErrorDto
                    {
                        MemberId = updateDto.MemberId,
                        ErrorMessage = "Member not found"
                    });
                    result.FailureCount++;
                    continue;
                }

                try
                {
                    // Update fields if provided
                    if (!string.IsNullOrWhiteSpace(updateDto.FirstName))
                        member.FirstName = updateDto.FirstName;
                    if (!string.IsNullOrWhiteSpace(updateDto.MiddleName))
                        member.MiddleName = updateDto.MiddleName;
                    if (!string.IsNullOrWhiteSpace(updateDto.LastName))
                        member.LastName = updateDto.LastName;
                    if (!string.IsNullOrWhiteSpace(updateDto.FullName))
                        member.FullName = updateDto.FullName;
                    if (updateDto.StatusId.HasValue)
                    {
                        var statusExists = await _context.Statuses.AnyAsync(s => s.Id == updateDto.StatusId.Value, cancellationToken);
                        if (statusExists)
                        {
                            member.StatusId = updateDto.StatusId.Value;
                            if (updateDto.StatusId.Value == 1 && member.ActivatedDate == null)
                                member.ActivatedDate = DateTime.Now;
                        }
                        else
                        {
                            result.Errors.Add(new BulkOperationErrorDto
                            {
                                MemberId = updateDto.MemberId,
                                ErrorMessage = "Status not found"
                            });
                            result.FailureCount++;
                            continue;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(updateDto.JobTitle))
                        member.JobTitle = updateDto.JobTitle;
                    if (!string.IsNullOrWhiteSpace(updateDto.MobileNumber))
                        member.MobileNumber = updateDto.MobileNumber;
                    if (!string.IsNullOrWhiteSpace(updateDto.NationalId))
                        member.NationalId = updateDto.NationalId;
                    if (updateDto.BirthDate.HasValue)
                        member.BirthDate = updateDto.BirthDate.Value.ToDateTime(TimeOnly.MinValue);
                    if (updateDto.IsMale.HasValue)
                        member.IsMale = updateDto.IsMale;
                    if (updateDto.BranchId.HasValue)
                    {
                        var branchExists = await _context.Branches.AnyAsync(b => b.Id == updateDto.BranchId.Value && !b.IsDeleted, cancellationToken);
                        if (branchExists)
                            member.BranchId = updateDto.BranchId.Value;
                        else
                        {
                            result.Errors.Add(new BulkOperationErrorDto
                            {
                                MemberId = updateDto.MemberId,
                                ErrorMessage = "Branch not found"
                            });
                            result.FailureCount++;
                            continue;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(updateDto.Notes))
                        member.Notes = updateDto.Notes;
                    if (!string.IsNullOrWhiteSpace(updateDto.PrivateNotes))
                        member.PrivateNotes = updateDto.PrivateNotes;
                    if (!string.IsNullOrWhiteSpace(updateDto.VipStatus))
                        member.VipStatus = updateDto.VipStatus;

                    member.UpdatedBy = currentUser;
                    member.UpdatedAt = DateTime.Now;
                    result.SuccessIds.Add(member.Id);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.Errors.Add(new BulkOperationErrorDto
                    {
                        MemberId = updateDto.MemberId,
                        ErrorMessage = ex.Message
                    });
                    result.FailureCount++;
                }
            }

            if (result.SuccessCount > 0)
            {
                await _context.SaveChangesAsync(cancellationToken);
            }

            return ServiceResult<BulkOperationResultDto>.Ok(result);
        }

        public async Task<ServiceResult<BulkOperationResultDto>> BulkUploadImagesAsync(BulkImageUploadDto dto, string currentUser, CancellationToken cancellationToken = default)
        {
            var result = new BulkOperationResultDto();
            var memberIds = dto.Images.Select(i => i.MemberId).Distinct().ToList();

            var members = await _context.MemberInfos
                .Where(m => memberIds.Contains(m.Id) && !m.IsDeleted)
                .ToListAsync(cancellationToken);

            var memberDict = members.ToDictionary(m => m.Id);

            foreach (var imageDto in dto.Images)
            {
                if (!memberDict.TryGetValue(imageDto.MemberId, out var member))
                {
                    result.Errors.Add(new BulkOperationErrorDto
                    {
                        MemberId = imageDto.MemberId,
                        ErrorMessage = "Member not found"
                    });
                    result.FailureCount++;
                    continue;
                }

                try
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(member.MemberImage))
                        await _imageService.DeleteImageAsync(member.MemberImage);

                    // Save new image with member ID as filename
                    var fileName = member.Id > 0 ? member.Id.ToString() : null;
                    member.MemberImage = await _imageService.SaveImageAsync(imageDto.ImageFile, "members", fileName, cancellationToken);
                    member.UpdatedBy = currentUser;
                    member.UpdatedAt = DateTime.Now;
                    result.SuccessIds.Add(member.Id);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.Errors.Add(new BulkOperationErrorDto
                    {
                        MemberId = imageDto.MemberId,
                        ErrorMessage = ex.Message
                    });
                    result.FailureCount++;
                }
            }

            if (result.SuccessCount > 0)
            {
                await _context.SaveChangesAsync(cancellationToken);
            }

            return ServiceResult<BulkOperationResultDto>.Ok(result);
        }

        public async Task<ServiceResult<byte[]>> ExportToExcelAsync(string lang, string? searchColumn = null, string? searchTerm = null, int? statusId = null, CancellationToken cancellationToken = default)
        {
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
                    .ThenInclude(p => p.Program)
                .Where(m => !m.IsDeleted);

            // Filter by StatusId if provided
            if (statusId.HasValue)
            {
                query = query.Where(m => m.StatusId == statusId.Value);
            }

            // Apply search if provided
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var normalizedTerm = searchTerm.Trim().ToLowerInvariant();
                query = ApplyMemberSearch(query, searchColumn, normalizedTerm, lang);
            }

            var members = await query
                .OrderByDescending(m => m.Id)
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
            var worksheet = package.Workbook.Worksheets.Add("Members");

            // Set headers based on language
            var headers = lang == "ar"
                ? new[] { "ID", "تاريخ الميلاد", "العمر", "اسم العميل", "اسم الفرع", "اسم البرنامج", "الحالة", "الجوال" }
                : new[] { "ID", "Birth Date", "Age", "Client Name", "Branch Name", "Program Name", "Status", "Mobile" };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // Add data rows
            for (int i = 0; i < members.Count; i++)
            {
                var member = members[i];
                var row = i + 2;

                // Get latest policy for member
                var latestPolicy = member.MemberPolicies
                    .Where(p => !p.IsDeleted)
                    .OrderByDescending(p => p.AddDate)
                    .FirstOrDefault();

                // Get ClientName: prefer from policy, fallback to branch's client
                var clientName = latestPolicy?.Policy?.Client != null
                    ? (lang == "ar" ? latestPolicy.Policy.Client.ArabicName : latestPolicy.Policy.Client.EnglishName)
                    : (member.Branch?.Client != null
                        ? (lang == "ar" ? member.Branch.Client.ArabicName : member.Branch.Client.EnglishName)
                        : string.Empty);

                // Get BranchName: prefer from policy, fallback to member's branch
                var branchName = latestPolicy?.Branch != null
                    ? (lang == "ar" ? latestPolicy.Branch.ArabicName : latestPolicy.Branch.EnglishName)
                    : (member.Branch != null
                        ? (lang == "ar" ? member.Branch.ArabicName : member.Branch.EnglishName)
                        : string.Empty);

                worksheet.Cells[row, 1].Value = member.Id;
                worksheet.Cells[row, 2].Value = member.BirthDate?.ToString("yyyy-MM-dd") ?? string.Empty;
                worksheet.Cells[row, 3].Value = CalculateAge(member.BirthDate);
                worksheet.Cells[row, 4].Value = clientName;
                worksheet.Cells[row, 5].Value = branchName;
                worksheet.Cells[row, 6].Value = latestPolicy?.Program != null && latestPolicy.Program.ProgramName != null
                    ? (lang == "ar" ? latestPolicy.Program.ProgramName.NameAr : latestPolicy.Program.ProgramName.NameEn)
                    : string.Empty;
                worksheet.Cells[row, 7].Value = member.Status != null
                    ? (lang == "ar" ? member.Status.NameAr : member.Status.NameEn)
                    : string.Empty;
                worksheet.Cells[row, 8].Value = member.MobileNumber ?? string.Empty;
            }

            // Auto-fit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var excelBytes = package.GetAsByteArray();
            return ServiceResult<byte[]>.Ok(excelBytes);
        }

        public async Task<ServiceResult<BulkOperationResultDto>> BulkUpdateMembersFromExcelAsync(Stream excelStream, string lang, string currentUser, CancellationToken cancellationToken = default)
        {
            var result = new BulkOperationResultDto();

            try
            {
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

                using var package = new ExcelPackage(excelStream);
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null || worksheet.Dimension == null)
                {
                    return ServiceResult<BulkOperationResultDto>.Fail(ServiceErrorType.Validation, "Excel file is empty or invalid");
                }

                // Read header row (row 1)
                var headers = new Dictionary<string, int>();
                for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                {
                    var headerValue = worksheet.Cells[1, col].Value?.ToString()?.Trim();
                    if (!string.IsNullOrWhiteSpace(headerValue))
                    {
                        headers[headerValue.ToLowerInvariant()] = col;
                    }
                }

                // Map all columns
                var memberIdCol = GetColumnIndex(headers, "memberid", "member id", "id");
                var memberNameCol = GetColumnIndex(headers, "membername", "member name", "name");
                var clientIdCol = GetColumnIndex(headers, "clientid", "client id");
                var mobileCol = GetColumnIndex(headers, "mobile", "mobilenumber");
                var branchIdCol = GetColumnIndex(headers, "branchid", "branch id");
                var programIdCol = GetColumnIndex(headers, "programid", "program id");
                var vipStatusIdCol = GetColumnIndex(headers, "vipstatusid", "vip status id", "vipstatus");
                var levelIdCol = GetColumnIndex(headers, "levelid", "level id");
                var isMaleCol = GetColumnIndex(headers, "ismale", "is male", "gender");
                var jobTitleCol = GetColumnIndex(headers, "jobtitle", "job title");
                var hofIdCol = GetColumnIndex(headers, "hofid", "hof id", "head of family");
                var activatedDateCol = GetColumnIndex(headers, "activateddate", "activated date");
                var notesCol = GetColumnIndex(headers, "notes", "note");
                var privateNotesCol = GetColumnIndex(headers, "privatenotes", "private notes");
                var nationalIdCol = GetColumnIndex(headers, "nationalid", "national id");
                var birthdayCol = GetColumnIndex(headers, "birthday", "birth date", "birthdate");
                var companyCodeCol = GetColumnIndex(headers, "companycode", "company code");

                // MemberId is required to identify which member to update
                if (memberIdCol == -1)
                {
                    return ServiceResult<BulkOperationResultDto>.Fail(ServiceErrorType.Validation, 
                        "Excel file must contain a 'MemberId' column to identify which member to update");
                }

                // First pass: Collect all MemberIds and validate them
                var memberIdRows = new Dictionary<int, int>(); // MemberId -> Row number
                var invalidRows = new List<(int row, string error)>();

                for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                {
                    var memberIdValue = worksheet.Cells[row, memberIdCol].Value?.ToString()?.Trim();
                    if (string.IsNullOrWhiteSpace(memberIdValue))
                        continue;

                    if (!int.TryParse(memberIdValue, out var memberId) || memberId < 1)
                    {
                        invalidRows.Add((row, $"Invalid MemberId '{memberIdValue}'"));
                        continue;
                    }

                    memberIdRows[memberId] = row;
                }

                // Check which MemberIds exist in the database
                var memberIds = memberIdRows.Keys.ToList();
                var existingMembers = await _context.MemberInfos
                    .Include(m => m.MemberPolicies.Where(p => !p.IsDeleted))
                    .Where(m => memberIds.Contains(m.Id) && !m.IsDeleted)
                    .ToDictionaryAsync(m => m.Id, cancellationToken);

                // Add errors for invalid MemberIds
                foreach (var invalidRow in invalidRows)
                {
                    result.Errors.Add(new BulkOperationErrorDto
                    {
                        MemberId = 0,
                        ErrorMessage = $"Row {invalidRow.row}: {invalidRow.error}"
                    });
                    result.FailureCount++;
                }

                // Add errors for MemberIds not found in database
                foreach (var memberId in memberIds)
                {
                    if (!existingMembers.ContainsKey(memberId))
                    {
                        result.Errors.Add(new BulkOperationErrorDto
                        {
                            MemberId = memberId,
                            ErrorMessage = $"Row {memberIdRows[memberId]}: Member with ID {memberId} not found in database"
                        });
                        result.FailureCount++;
                    }
                }

                // Second pass: Process only valid members (those that exist in database)
                var membersToSkip = new HashSet<int>(); // Track members that should not be saved due to validation errors
                
                foreach (var kvp in memberIdRows)
                {
                    var memberId = kvp.Key;
                    var row = kvp.Value;

                    // Skip if member doesn't exist (already added to errors)
                    if (!existingMembers.TryGetValue(memberId, out var member))
                        continue;

                    // Skip if this member already has validation errors
                    if (membersToSkip.Contains(memberId))
                        continue;

                    try
                    {
                        // Update MemberInfo fields
                        if (memberNameCol > 0)
                        {
                            var memberName = worksheet.Cells[row, memberNameCol].Value?.ToString()?.Trim();
                            if (!string.IsNullOrWhiteSpace(memberName))
                            {
                                member.FullName = memberName;
                                var nameParts = memberName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                                
                                // Truncate to fit database column lengths (typically 20 chars for FirstName/LastName)
                                const int maxNameLength = 20;
                                member.FirstName = nameParts.Length > 0 
                                    ? (nameParts[0].Length > maxNameLength ? nameParts[0].Substring(0, maxNameLength) : nameParts[0])
                                    : (memberName.Length > maxNameLength ? memberName.Substring(0, maxNameLength) : memberName);
                                
                                var lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : string.Empty;
                                member.LastName = lastName.Length > maxNameLength ? lastName.Substring(0, maxNameLength) : lastName;
                            }
                        }

                        if (mobileCol > 0)
                        {
                            var mobile = worksheet.Cells[row, mobileCol].Value?.ToString()?.Trim();
                            if (!string.IsNullOrWhiteSpace(mobile) && mobile != member.MobileNumber)
                            {
                                var mobileExists = await _context.MemberInfos.AnyAsync(m => m.MobileNumber == mobile && m.Id != memberId && !m.IsDeleted, cancellationToken);
                                if (mobileExists)
                                {
                                    result.Errors.Add(new BulkOperationErrorDto
                                    {
                                        MemberId = memberId,
                                        ErrorMessage = $"Row {row}: Mobile number already exists"
                                    });
                                    result.FailureCount++;
                                    membersToSkip.Add(memberId);
                                    continue;
                                }
                                member.MobileNumber = mobile;
                            }
                        }

                        if (branchIdCol > 0)
                        {
                            var branchIdValue = worksheet.Cells[row, branchIdCol].Value?.ToString()?.Trim();
                            if (!string.IsNullOrWhiteSpace(branchIdValue) && int.TryParse(branchIdValue, out var branchId) && branchId > 0)
                            {
                                var branchExists = await _context.Branches.AnyAsync(b => b.Id == branchId && !b.IsDeleted, cancellationToken);
                                if (branchExists)
                                    member.BranchId = branchId;
                            }
                        }

                        if (isMaleCol > 0)
                        {
                            var isMaleValue = worksheet.Cells[row, isMaleCol].Value?.ToString()?.Trim();
                            if (!string.IsNullOrWhiteSpace(isMaleValue))
                            {
                                var lowerValue = isMaleValue.ToLowerInvariant();
                                if (lowerValue == "true" || lowerValue == "yes" || lowerValue == "1" || lowerValue == "male")
                                    member.IsMale = true;
                                else if (lowerValue == "false" || lowerValue == "no" || lowerValue == "0" || lowerValue == "female")
                                    member.IsMale = false;
                            }
                        }

                        if (jobTitleCol > 0)
                        {
                            var jobTitle = worksheet.Cells[row, jobTitleCol].Value?.ToString()?.Trim();
                            if (!string.IsNullOrWhiteSpace(jobTitle))
                                member.JobTitle = jobTitle;
                        }

                        if (notesCol > 0)
                        {
                            var notes = worksheet.Cells[row, notesCol].Value?.ToString()?.Trim();
                            if (!string.IsNullOrWhiteSpace(notes))
                                member.Notes = notes;
                        }

                        if (privateNotesCol > 0)
                        {
                            var privateNotes = worksheet.Cells[row, privateNotesCol].Value?.ToString()?.Trim();
                            if (!string.IsNullOrWhiteSpace(privateNotes))
                                member.PrivateNotes = privateNotes;
                        }

                        if (nationalIdCol > 0)
                        {
                            var nationalId = worksheet.Cells[row, nationalIdCol].Value?.ToString()?.Trim();
                            if (!string.IsNullOrWhiteSpace(nationalId) && nationalId != member.NationalId)
                            {
                                var exists = await _context.MemberInfos.AnyAsync(m => m.NationalId == nationalId && m.Id != memberId && !m.IsDeleted, cancellationToken);
                                if (exists)
                                {
                                    result.Errors.Add(new BulkOperationErrorDto
                                    {
                                        MemberId = memberId,
                                        ErrorMessage = $"Row {row}: National ID already exists"
                                    });
                                    result.FailureCount++;
                                    membersToSkip.Add(memberId);
                                    continue;
                                }
                                member.NationalId = nationalId;
                                // Infer birthdate and gender from NationalId
                                if (TryInferFromNationalId(nationalId, out var birthDate, out var isMale))
                                {
                                    member.BirthDate = birthDate;
                                    member.IsMale = isMale;
                                }
                            }
                        }

                        if (birthdayCol > 0)
                        {
                            var birthdayValue = worksheet.Cells[row, birthdayCol].Value;
                            if (birthdayValue != null)
                            {
                                if (birthdayValue is DateTime dt)
                                    member.BirthDate = dt;
                                else if (DateTime.TryParse(birthdayValue.ToString(), out var parsedDate))
                                    member.BirthDate = parsedDate;
                            }
                        }

                        if (activatedDateCol > 0)
                        {
                            var activatedDateValue = worksheet.Cells[row, activatedDateCol].Value;
                            if (activatedDateValue != null)
                            {
                                if (activatedDateValue is DateTime dt)
                                    member.ActivatedDate = dt;
                                else if (DateTime.TryParse(activatedDateValue.ToString(), out var parsedDate))
                                    member.ActivatedDate = parsedDate;
                            }
                        }

                        // Update VIP Status (need to get VIP status name from ID)
                        if (vipStatusIdCol > 0)
                        {
                            var vipStatusIdValue = worksheet.Cells[row, vipStatusIdCol].Value?.ToString()?.Trim();
                            if (!string.IsNullOrWhiteSpace(vipStatusIdValue) && int.TryParse(vipStatusIdValue, out var vipStatusId) && vipStatusId > 0)
                            {
                                var vipStatus = await _context.VipStatuses.FirstOrDefaultAsync(v => v.Id == vipStatusId && !v.IsDeleted, cancellationToken);
                                if (vipStatus != null)
                                {
                                    member.VipStatus = lang == "ar" ? vipStatus.NameAr : vipStatus.NameEn;
                                }
                            }
                        }

                        // Update MemberPolicyInfo if ProgramId, CompanyCode, or HofId are provided
                        var latestPolicy = member.MemberPolicies
                            .Where(p => !p.IsDeleted)
                            .OrderByDescending(p => p.AddDate)
                            .FirstOrDefault();

                        if (latestPolicy != null)
                        {
                            bool policyUpdated = false;

                            if (programIdCol > 0)
                            {
                                var programIdValue = worksheet.Cells[row, programIdCol].Value?.ToString()?.Trim();
                                if (!string.IsNullOrWhiteSpace(programIdValue) && int.TryParse(programIdValue, out var programId) && programId > 0)
                                {
                                    var programExists = await _context.GeneralPrograms.AnyAsync(p => p.Id == programId, cancellationToken);
                                    if (programExists)
                                    {
                                        latestPolicy.ProgramId = programId;
                                        policyUpdated = true;
                                    }
                                }
                            }

                            if (companyCodeCol > 0)
                            {
                                var companyCode = worksheet.Cells[row, companyCodeCol].Value?.ToString()?.Trim();
                                if (!string.IsNullOrWhiteSpace(companyCode))
                                {
                                    latestPolicy.CodeAtCompany = companyCode;
                                    policyUpdated = true;
                                }
                            }

                            if (hofIdCol > 0)
                            {
                                var hofIdValue = worksheet.Cells[row, hofIdCol].Value?.ToString()?.Trim();
                                if (!string.IsNullOrWhiteSpace(hofIdValue))
                                {
                                    if (int.TryParse(hofIdValue, out var hofId) && hofId > 0)
                                    {
                                        latestPolicy.HeadOfFamilyId = hofId;
                                    }
                                    else
                                    {
                                        latestPolicy.HeadOfFamilyId = null;
                                    }
                                    policyUpdated = true;
                                }
                            }

                            if (jobTitleCol > 0)
                            {
                                var jobTitle = worksheet.Cells[row, jobTitleCol].Value?.ToString()?.Trim();
                                if (!string.IsNullOrWhiteSpace(jobTitle))
                                {
                                    latestPolicy.JobTitle = jobTitle;
                                    policyUpdated = true;
                                }
                            }

                            if (vipStatusIdCol > 0)
                            {
                                var vipStatusIdValue = worksheet.Cells[row, vipStatusIdCol].Value?.ToString()?.Trim();
                                if (!string.IsNullOrWhiteSpace(vipStatusIdValue) && int.TryParse(vipStatusIdValue, out var vipStatusId) && vipStatusId > 0)
                                {
                                    latestPolicy.IsVip = vipStatusId > 1; // Assuming 1 is "No"
                                    policyUpdated = true;
                                }
                            }

                            if (policyUpdated)
                            {
                                latestPolicy.UpdatedBy = currentUser;
                                latestPolicy.UpdatedAt = DateTime.Now;
                            }
                        }

                        // Only mark for success if member passed all validations
                        if (!membersToSkip.Contains(memberId))
                        {
                            member.UpdatedBy = currentUser;
                            member.UpdatedAt = DateTime.Now;

                            result.SuccessIds.Add(member.Id);
                            result.SuccessCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add(new BulkOperationErrorDto
                        {
                            MemberId = memberId,
                            ErrorMessage = $"Row {row}: Error updating member - {ex.Message}"
                        });
                        result.FailureCount++;
                        membersToSkip.Add(memberId);
                    }
                }

                // Remove members with validation errors from tracking before save
                foreach (var memberId in membersToSkip)
                {
                    if (existingMembers.TryGetValue(memberId, out var member))
                    {
                        // Detach the entity to prevent saving changes
                        _context.Entry(member).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                    }
                }

                if (result.SuccessCount > 0)
                {
                    try
                    {
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                    catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
                    {
                        // Extract the inner exception message for more details
                        var errorMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                        var innerEx = dbEx.InnerException;
                        
                        // Try to get more specific error information
                        while (innerEx?.InnerException != null)
                        {
                            innerEx = innerEx.InnerException;
                        }
                        
                        var detailedError = innerEx?.Message ?? errorMessage;
                        
                        // Check if it's a constraint violation
                        if (detailedError.Contains("UNIQUE") || detailedError.Contains("duplicate") || detailedError.Contains("constraint") || detailedError.Contains("PRIMARY KEY"))
                        {
                            return ServiceResult<BulkOperationResultDto>.Fail(ServiceErrorType.Validation, 
                                $"Database constraint violation: {detailedError}. Please check for duplicate values (Mobile, NationalId, etc.) or invalid foreign key references.");
                        }
                        
                        // Check for foreign key violations
                        if (detailedError.Contains("FOREIGN KEY") || detailedError.Contains("reference"))
                        {
                            return ServiceResult<BulkOperationResultDto>.Fail(ServiceErrorType.Validation, 
                                $"Foreign key constraint violation: {detailedError}. Please check that BranchId, ProgramId, LevelId, VipStatusId, etc. are valid.");
                        }
                        
                        return ServiceResult<BulkOperationResultDto>.Fail(ServiceErrorType.Unexpected, 
                            $"Database error while saving changes: {detailedError}");
                    }
                    catch (Exception saveEx)
                    {
                        return ServiceResult<BulkOperationResultDto>.Fail(ServiceErrorType.Unexpected, 
                            $"Error saving changes: {saveEx.Message}");
                    }
                }

                return ServiceResult<BulkOperationResultDto>.Ok(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<BulkOperationResultDto>.Fail(ServiceErrorType.Unexpected, $"Error reading Excel file: {ex.Message}");
            }
        }

        private static int GetColumnIndex(Dictionary<string, int> headers, params string[] possibleNames)
        {
            foreach (var name in possibleNames)
            {
                if (headers.TryGetValue(name.ToLowerInvariant(), out var index))
                    return index;
            }
            return -1;
        }

        private static MemberInfoListItemDto MapListItem(MemberInfo member, string lang)
        {
            var latestPolicy = member.MemberPolicies
                .Where(p => !p.IsDeleted)
                .OrderByDescending(p => p.AddDate)
                .FirstOrDefault();

            // Get ClientName: prefer from policy, fallback to branch's client, then member's direct client
            var clientName = latestPolicy?.Policy?.Client != null
                ? (lang == "ar" ? latestPolicy.Policy.Client.ArabicName : latestPolicy.Policy.Client.EnglishName)
                : (member.Branch?.Client != null
                    ? (lang == "ar" ? member.Branch.Client.ArabicName : member.Branch.Client.EnglishName)
                    : (member.Client != null
                        ? (lang == "ar" ? member.Client.ArabicName : member.Client.EnglishName)
                        : null));

            // Get BranchName: prefer from policy, fallback to member's branch
            var branchName = latestPolicy?.Branch != null
                ? (lang == "ar" ? latestPolicy.Branch.ArabicName : latestPolicy.Branch.EnglishName)
                : (member.Branch != null
                    ? (lang == "ar" ? member.Branch.ArabicName : member.Branch.EnglishName)
                    : null);

            // Get ProgramName: prefer from member's direct Program, fallback to policy's Program
            string? programName = null;
            if (member.Program != null)
            {
                programName = lang == "ar" ? member.Program.NameAr : member.Program.NameEn;
            }
            else if (latestPolicy?.Program != null)
            {
                var program = latestPolicy.Program;
                if (program.ProgramName != null)
                {
                    programName = lang == "ar" ? program.ProgramName.NameAr : program.ProgramName.NameEn;
                }
            }

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

        private static MemberInfoDetailDto MapDetail(MemberInfo member, string lang)
        {
            // Get the latest policy (most recent AddDate)
            var latestPolicy = member.MemberPolicies
                .Where(p => !p.IsDeleted)
                .OrderByDescending(p => p.AddDate)
                .FirstOrDefault();

            // Get Client name: prefer from latest policy, fallback to member's direct client
            var clientName = latestPolicy?.Policy?.Client != null
                ? (lang == "ar" ? latestPolicy.Policy.Client.ArabicName : latestPolicy.Policy.Client.EnglishName)
                : (member.Client != null
                    ? (lang == "ar" ? member.Client.ArabicName : member.Client.EnglishName)
                    : null);

            // Get Branch name: prefer from latest policy, fallback to member's direct branch
            var branchName = latestPolicy?.Branch != null
                ? (lang == "ar" ? latestPolicy.Branch.ArabicName : latestPolicy.Branch.EnglishName)
                : (member.Branch != null
                    ? (lang == "ar" ? member.Branch.ArabicName : member.Branch.EnglishName)
                    : null);

            // Get Program name: prefer from member's direct Program, fallback to policy's Program
            string? programName = null;
            if (member.Program != null)
            {
                programName = lang == "ar" ? member.Program.NameAr : member.Program.NameEn;
            }
            else if (latestPolicy?.Program != null && latestPolicy.Program.ProgramName != null)
            {
                programName = lang == "ar" ? latestPolicy.Program.ProgramName.NameAr : latestPolicy.Program.ProgramName.NameEn;
            }

            return new MemberInfoDetailDto
            {
                Id = member.Id,
                Name = member.FullName,
                NationalId = member.NationalId,
                BirthDate = member.BirthDate?.ToString("yyyy-MM-dd"),
                Age = CalculateAge(member.BirthDate),
                IsMale = member.IsMale,
                JobTitle = member.JobTitle,
                Notes = member.Notes,
                PrivateNotes = member.PrivateNotes,
                MobileNumber = member.MobileNumber,
                Client = clientName,
                Branch = branchName,
                Program = programName,
                CompanyCode = latestPolicy?.CodeAtCompany,
                Status = member.Status != null ? (lang == "ar" ? member.Status.NameAr : member.Status.NameEn) : "Active",
                VipStatus = member.VipStatus,
                ImageUrl = member.MemberImage,
                ActivatedDate = member.ActivatedDate?.ToString("yyyy-MM-dd")
            };
        }

        private static int? CalculateAge(DateTime? birthDate)
        {
            if (!birthDate.HasValue) return null;
            var today = DateTime.Today;
            var age = today.Year - birthDate.Value.Year;
            if (birthDate.Value.Date > today.AddYears(-age)) age--;
            return age;
        }

        private static bool TryInferFromNationalId(string? nationalId, out DateTime? birthDate, out bool? isMale)
        {
            birthDate = null;
            isMale = null;
            if (string.IsNullOrWhiteSpace(nationalId) || nationalId.Length != 14)
                return false;

            try
            {
                var century = nationalId[0] == '2' ? 1900 : 2000;
                var year = int.Parse(nationalId.Substring(1, 2)) + century;
                var month = int.Parse(nationalId.Substring(3, 2));
                var day = int.Parse(nationalId.Substring(5, 2));
                birthDate = new DateTime(year, month, day);

                var genderDigit = int.Parse(nationalId.Substring(12, 1));
                isMale = genderDigit % 2 != 0;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

