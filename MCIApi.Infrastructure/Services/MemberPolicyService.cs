using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;
using MCIApi.Application.Common;
using MCIApi.Application.MemberPolicies.DTOs;
using MCIApi.Application.MemberPolicies.Interfaces;
using MCIApi.Application.Common.Interfaces;
using MCIApi.Domain.Entities;
using MCIApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MCIApi.Infrastructure.Services
{
    public class MemberPolicyService : IMemberPolicyService
    {
        private readonly AppDbContext _context;
        private readonly IImageService _imageService;

        public MemberPolicyService(AppDbContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public async Task<ServiceResult<IReadOnlyCollection<MemberPolicyDto>>> GetAllAsync(string lang, CancellationToken cancellationToken = default)
        {
            var policies = await _context.MemberPolicyInfos
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .ToListAsync(cancellationToken);

            var data = policies.Select(MapDto).ToList().AsReadOnly();
            return ServiceResult<IReadOnlyCollection<MemberPolicyDto>>.Ok(data);
        }

        public async Task<ServiceResult<MemberPolicyDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default)
        {
            var policy = await _context.MemberPolicyInfos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);

            if (policy == null)
                return ServiceResult<MemberPolicyDto>.Fail(ServiceErrorType.NotFound, "MemberPolicyNotFound");

            return ServiceResult<MemberPolicyDto>.Ok(MapDto(policy));
        }

        public async Task<ServiceResult<MemberPolicyDto>> CreateAsync(MemberPolicyCreateDto dto, string lang, string currentUser, CancellationToken cancellationToken = default)
        {
            var validationResult = await ValidateForeignKeysAsync(dto.MemberId, dto.PolicyId, dto.ProgramId, dto.RelationId, dto.BranchId, dto.HeadOfFamilyId, cancellationToken);
            if (validationResult is not null)
                return ServiceResult<MemberPolicyDto>.Fail(ServiceErrorType.Validation, validationResult);

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var emailExists = await _context.MemberPolicyInfos.AnyAsync(p => !p.IsDeleted && p.Email == dto.Email, cancellationToken);
                if (emailExists)
                    return ServiceResult<MemberPolicyDto>.Fail(ServiceErrorType.Conflict, "EmailExists");
            }

            string? imagePath = null;
            if (dto.ImageFile is not null)
                imagePath = await _imageService.SaveImageAsync(dto.ImageFile, "member-policies", cancellationToken);

            var policy = new MemberPolicyInfo
            {
                MemberId = dto.MemberId,
                PolicyId = dto.PolicyId,
                ProgramId = dto.ProgramId,
                RelationId = dto.RelationId,
                BranchId = dto.BranchId,
                HeadOfFamilyId = dto.HeadOfFamilyId,
                IsVip = dto.IsVip,
                JobTitle = dto.JobTitle,
                Notes = dto.Notes,
                Address = dto.Address,
                CardPrinted = dto.CardPrinted,
                AddDate = dto.AddDate,
                CodeAtCompany = dto.CodeAtCompany,
                FirebaseToken = dto.FirebaseToken,
                Email = dto.Email,
                IsHr = dto.IsHr,
                ImageUrl = imagePath,
                AppPassword = string.IsNullOrWhiteSpace(dto.AppPassword) ? null : BC.HashPassword(dto.AppPassword),
                CreatedBy = currentUser,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            _context.MemberPolicyInfos.Add(policy);
            await _context.SaveChangesAsync(cancellationToken);

            return ServiceResult<MemberPolicyDto>.Ok(MapDto(policy));
        }

        public async Task<ServiceResult<MemberPolicyDto>> UpdateAsync(int id, MemberPolicyUpdateDto dto, string lang, string currentUser, CancellationToken cancellationToken = default)
        {
            var policy = await _context.MemberPolicyInfos.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);
            if (policy == null)
                return ServiceResult<MemberPolicyDto>.Fail(ServiceErrorType.NotFound, "MemberPolicyNotFound");

            var validationResult = await ValidateForeignKeysAsync(dto.MemberId, dto.PolicyId, dto.ProgramId, dto.RelationId, dto.BranchId, dto.HeadOfFamilyId, cancellationToken);
            if (validationResult is not null)
                return ServiceResult<MemberPolicyDto>.Fail(ServiceErrorType.Validation, validationResult);

            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != policy.Email)
            {
                var emailExists = await _context.MemberPolicyInfos.AnyAsync(p => !p.IsDeleted && p.Email == dto.Email && p.Id != id, cancellationToken);
                if (emailExists)
                    return ServiceResult<MemberPolicyDto>.Fail(ServiceErrorType.Conflict, "EmailExists");
            }

            if (dto.MemberId.HasValue) policy.MemberId = dto.MemberId.Value;
            if (dto.PolicyId.HasValue) policy.PolicyId = dto.PolicyId.Value;
            if (dto.ProgramId.HasValue) policy.ProgramId = dto.ProgramId.Value;
            if (dto.RelationId.HasValue) policy.RelationId = dto.RelationId.Value;
            if (dto.BranchId.HasValue) policy.BranchId = dto.BranchId.Value;
            if (dto.HeadOfFamilyId.HasValue) policy.HeadOfFamilyId = dto.HeadOfFamilyId.Value;
            if (dto.IsVip.HasValue) policy.IsVip = dto.IsVip.Value;
            if (dto.JobTitle is not null) policy.JobTitle = dto.JobTitle;
            if (dto.Notes is not null) policy.Notes = dto.Notes;
            if (dto.Address is not null) policy.Address = dto.Address;
            if (dto.FirebaseToken is not null) policy.FirebaseToken = dto.FirebaseToken;
            if (dto.Email is not null) policy.Email = dto.Email;
            if (dto.IsHr.HasValue) policy.IsHr = dto.IsHr.Value;
            if (dto.CardPrinted.HasValue) policy.CardPrinted = dto.CardPrinted.Value;
            if (dto.CodeAtCompany is not null) policy.CodeAtCompany = dto.CodeAtCompany;

            if (!string.IsNullOrWhiteSpace(dto.AppPassword))
                policy.AppPassword = BC.HashPassword(dto.AppPassword);

            if (dto.RemoveImage == true && !string.IsNullOrEmpty(policy.ImageUrl))
            {
                await _imageService.DeleteImageAsync(policy.ImageUrl);
                policy.ImageUrl = null;
            }

            if (dto.ImageFile is not null)
            {
                if (!string.IsNullOrEmpty(policy.ImageUrl))
                    await _imageService.DeleteImageAsync(policy.ImageUrl);
                policy.ImageUrl = await _imageService.SaveImageAsync(dto.ImageFile, "member-policies", cancellationToken);
            }

            policy.UpdatedAt = DateTime.Now;
            policy.UpdatedBy = currentUser;

            await _context.SaveChangesAsync(cancellationToken);

            return ServiceResult<MemberPolicyDto>.Ok(MapDto(policy));
        }

        public async Task<ServiceResult> DeleteAsync(int id, string lang, string currentUser, CancellationToken cancellationToken = default)
        {
            var policy = await _context.MemberPolicyInfos.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);
            if (policy == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "MemberPolicyNotFound");

            policy.IsDeleted = true;
            policy.DeleteDate = DateTime.Now;
            policy.UpdatedBy = currentUser;
            policy.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok();
        }

        private static MemberPolicyDto MapDto(MemberPolicyInfo policy) => new()
        {
            Id = policy.Id,
            MemberId = policy.MemberId,
            PolicyId = policy.PolicyId,
            ProgramId = policy.ProgramId,
            RelationId = policy.RelationId,
            BranchId = policy.BranchId,
            HeadOfFamilyId = policy.HeadOfFamilyId,
            IsVip = policy.IsVip,
            JobTitle = policy.JobTitle,
            Notes = policy.Notes,
            Address = policy.Address,
            ImageUrl = policy.ImageUrl,
            FirebaseToken = policy.FirebaseToken,
            Email = policy.Email,
            IsHr = policy.IsHr,
            AddDate = policy.AddDate,
            CardPrinted = policy.CardPrinted,
            CodeAtCompany = policy.CodeAtCompany
        };

        private async Task<string?> ValidateForeignKeysAsync(int? memberId, int? policyId, int? programId, int? relationId, int? branchId, int? headOfFamilyId, CancellationToken cancellationToken)
        {
            if (memberId.HasValue)
            {
                var member = await _context.MemberInfos.FirstOrDefaultAsync(m => m.Id == memberId.Value && !m.IsDeleted, cancellationToken);
                if (member == null)
                    return "InvalidMember";
            }

            if (policyId.HasValue)
            {
                var policy = await _context.Policies.FirstOrDefaultAsync(p => p.Id == policyId.Value && !p.IsDeleted, cancellationToken);
                if (policy == null)
                    return "InvalidPolicy";
            }

            if (programId.HasValue)
            {
                var program = await _context.GeneralPrograms.FirstOrDefaultAsync(p => p.Id == programId.Value, cancellationToken);
                if (program == null)
                    return "InvalidProgram";
            }

            if (relationId.HasValue)
            {
                var relation = await _context.Relations.FirstOrDefaultAsync(r => r.Id == relationId.Value, cancellationToken);
                if (relation == null)
                    return "InvalidRelation";
            }

            if (branchId.HasValue)
            {
                var branch = await _context.Branches.FirstOrDefaultAsync(b => b.Id == branchId.Value && !b.IsDeleted, cancellationToken);
                if (branch == null)
                    return "InvalidBranch";
            }

            if (headOfFamilyId.HasValue)
            {
                var head = await _context.MemberInfos.FirstOrDefaultAsync(m => m.Id == headOfFamilyId.Value && !m.IsDeleted, cancellationToken);
                if (head == null)
                    return "InvalidHeadOfFamily";
            }

            return null;
        }
    }
}

