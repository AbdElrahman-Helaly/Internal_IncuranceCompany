using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MCIApi.Application.Common;
using MCIApi.Application.CPTs.DTOs;
using MCIApi.Application.CPTs.Interfaces;
using MCIApi.Domain.Entities;
using MCIApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MCIApi.Infrastructure.Services
{
    public class CPTService : ICPTService
    {
        private readonly AppDbContext _context;

        public CPTService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<CPTPagedResultDto>> GetAllCPTAsync(CPTFilterDto filter, string lang, CancellationToken cancellationToken = default)
        {
            var query = _context.CPTs
                .Include(c => c.Status)
                .Include(c => c.Category)
                .AsQueryable();

            // Filter: Minimum CPT code length is 3 AND does NOT start with "D" (case-insensitive)
            query = query.Where(c => c.CPTCode.Length >= 3 && !c.CPTCode.ToUpper().StartsWith("D"));

            // Apply search if provided
            if (!string.IsNullOrWhiteSpace(filter.Search) && !string.IsNullOrWhiteSpace(filter.SearchColumn))
            {
                var search = filter.Search.Trim().ToLower();
                query = filter.SearchColumn.ToLower() switch
                {
                    "arname" or "namear" => query.Where(c => c.ArName.ToLower().Contains(search)),
                    "enname" or "nameen" => query.Where(c => c.EnName.ToLower().Contains(search)),
                    "cptcode" or "code" => query.Where(c => c.CPTCode.ToLower().Contains(search)),
                    "description" => query.Where(c => c.CPTDescription != null && c.CPTDescription.ToLower().Contains(search)),
                    "ichi" => query.Where(c => c.ICHI != null && c.ICHI.ToLower().Contains(search)),
                    _ => query
                };
            }

            var total = await query.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(total / (double)filter.Limit);

            // Get the CPT IDs for the current page
            var pagedCptIds = await query
                .OrderBy(c => c.CPTCode)
                .Skip((filter.Page - 1) * filter.Limit)
                .Take(filter.Limit)
                .Select(c => c.Id)
                .ToListAsync(cancellationToken);

            // Get price list service counts only for CPTs in the current page
            var priceListCounts = await _context.ProviderPriceListServices
                .Where(p => pagedCptIds.Contains(p.CptId))
                .GroupBy(p => p.CptId)
                .Select(g => new { CptId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CptId, x => x.Count, cancellationToken);

            var data = await query
                .Where(c => pagedCptIds.Contains(c.Id))
                .OrderBy(c => c.CPTCode)
                .Select(c => new CPTListItemDto
                {
                    Id = c.Id,
                    ArName = c.ArName,
                    EnName = c.EnName,
                    CPTCode = c.CPTCode,
                    CPTDescription = c.CPTDescription,
                    StatusId = c.StatusId,
                    StatusName = lang == "ar" ? c.Status!.NameAr : c.Status!.NameEn,
                    CategoryId = c.CategoryId,
                    CategoryName = c.Category != null ? c.Category.Name : null,
                    ICHI = c.ICHI,
                    CountInPriceList = 0 // Will be set after materialization
                })
                .ToListAsync(cancellationToken);

            // Set the counts after materializing the query
            foreach (var item in data)
            {
                item.CountInPriceList = priceListCounts.GetValueOrDefault(item.Id, 0);
            }

            var result = new CPTPagedResultDto
            {
                Total = total,
                CurrentPage = filter.Page,
                Limit = filter.Limit,
                TotalPages = totalPages,
                Data = data
            };

            return ServiceResult<CPTPagedResultDto>.Ok(result);
        }

        public async Task<ServiceResult<CPTPagedResultDto>> GetAllCDTAsync(CPTFilterDto filter, string lang, CancellationToken cancellationToken = default)
        {
            var query = _context.CPTs
                .Include(c => c.Status)
                .Include(c => c.Category)
                .AsQueryable();

            // Filter: CPT code starts with "D" or "d"
            query = query.Where(c => c.CPTCode.StartsWith("D") || c.CPTCode.StartsWith("d"));

            // Apply search if provided
            if (!string.IsNullOrWhiteSpace(filter.Search) && !string.IsNullOrWhiteSpace(filter.SearchColumn))
            {
                var search = filter.Search.Trim().ToLower();
                query = filter.SearchColumn.ToLower() switch
                {
                    "arname" or "namear" => query.Where(c => c.ArName.ToLower().Contains(search)),
                    "enname" or "nameen" => query.Where(c => c.EnName.ToLower().Contains(search)),
                    "cptcode" or "code" => query.Where(c => c.CPTCode.ToLower().Contains(search)),
                    "description" => query.Where(c => c.CPTDescription != null && c.CPTDescription.ToLower().Contains(search)),
                    "ichi" => query.Where(c => c.ICHI != null && c.ICHI.ToLower().Contains(search)),
                    _ => query
                };
            }

            var total = await query.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(total / (double)filter.Limit);

            // Get the CPT IDs for the current page
            var pagedCptIds = await query
                .OrderBy(c => c.CPTCode)
                .Skip((filter.Page - 1) * filter.Limit)
                .Take(filter.Limit)
                .Select(c => c.Id)
                .ToListAsync(cancellationToken);

            // Get price list service counts only for CPTs in the current page
            var priceListCounts = await _context.ProviderPriceListServices
                .Where(p => pagedCptIds.Contains(p.CptId))
                .GroupBy(p => p.CptId)
                .Select(g => new { CptId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CptId, x => x.Count, cancellationToken);

            var data = await query
                .Where(c => pagedCptIds.Contains(c.Id))
                .OrderBy(c => c.CPTCode)
                .Select(c => new CPTListItemDto
                {
                    Id = c.Id,
                    ArName = c.ArName,
                    EnName = c.EnName,
                    CPTCode = c.CPTCode,
                    CPTDescription = c.CPTDescription,
                    StatusId = c.StatusId,
                    StatusName = lang == "ar" ? c.Status!.NameAr : c.Status!.NameEn,
                    CategoryId = c.CategoryId,
                    CategoryName = c.Category != null ? c.Category.Name : null,
                    ICHI = c.ICHI,
                    CountInPriceList = 0 // Will be set after materialization
                })
                .ToListAsync(cancellationToken);

            // Set the counts after materializing the query
            foreach (var item in data)
            {
                item.CountInPriceList = priceListCounts.GetValueOrDefault(item.Id, 0);
            }

            var result = new CPTPagedResultDto
            {
                Total = total,
                CurrentPage = filter.Page,
                Limit = filter.Limit,
                TotalPages = totalPages,
                Data = data
            };

            return ServiceResult<CPTPagedResultDto>.Ok(result);
        }

        public async Task<ServiceResult<CPTPagedResultDto>> GetALNOTFOUNDAsync(CPTFilterDto filter, string lang, CancellationToken cancellationToken = default)
        {
            var query = _context.CPTs
                .Include(c => c.Status)
                .Include(c => c.Category)
                .AsQueryable();

            // Filter: Maximum CPT code length is 3
            query = query.Where(c => c.CPTCode.Length <= 3);

            // Apply search if provided
            if (!string.IsNullOrWhiteSpace(filter.Search) && !string.IsNullOrWhiteSpace(filter.SearchColumn))
            {
                var search = filter.Search.Trim().ToLower();
                query = filter.SearchColumn.ToLower() switch
                {
                    "arname" or "namear" => query.Where(c => c.ArName.ToLower().Contains(search)),
                    "enname" or "nameen" => query.Where(c => c.EnName.ToLower().Contains(search)),
                    "cptcode" or "code" => query.Where(c => c.CPTCode.ToLower().Contains(search)),
                    "description" => query.Where(c => c.CPTDescription != null && c.CPTDescription.ToLower().Contains(search)),
                    "ichi" => query.Where(c => c.ICHI != null && c.ICHI.ToLower().Contains(search)),
                    _ => query
                };
            }

            var total = await query.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(total / (double)filter.Limit);

            // Get the CPT IDs for the current page
            var pagedCptIds = await query
                .OrderBy(c => c.CPTCode)
                .Skip((filter.Page - 1) * filter.Limit)
                .Take(filter.Limit)
                .Select(c => c.Id)
                .ToListAsync(cancellationToken);

            // Get price list service counts only for CPTs in the current page
            var priceListCounts = await _context.ProviderPriceListServices
                .Where(p => pagedCptIds.Contains(p.CptId))
                .GroupBy(p => p.CptId)
                .Select(g => new { CptId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CptId, x => x.Count, cancellationToken);

            var data = await query
                .Where(c => pagedCptIds.Contains(c.Id))
                .OrderBy(c => c.CPTCode)
                .Select(c => new CPTListItemDto
                {
                    Id = c.Id,
                    ArName = c.ArName,
                    EnName = c.EnName,
                    CPTCode = c.CPTCode,
                    CPTDescription = c.CPTDescription,
                    StatusId = c.StatusId,
                    StatusName = lang == "ar" ? c.Status!.NameAr : c.Status!.NameEn,
                    CategoryId = c.CategoryId,
                    CategoryName = c.Category != null ? c.Category.Name : null,
                    ICHI = c.ICHI,
                    CountInPriceList = 0 // Will be set after materialization
                })
                .ToListAsync(cancellationToken);

            // Set the counts after materializing the query
            foreach (var item in data)
            {
                item.CountInPriceList = priceListCounts.GetValueOrDefault(item.Id, 0);
            }

            var result = new CPTPagedResultDto
            {
                Total = total,
                CurrentPage = filter.Page,
                Limit = filter.Limit,
                TotalPages = totalPages,
                Data = data
            };

            return ServiceResult<CPTPagedResultDto>.Ok(result);
        }

        public async Task<ServiceResult<CPTListItemDto>> CreateCPTAsync(CPTCreateDto dto, string lang, CancellationToken cancellationToken = default)
        {
            // Check if CPT code already exists
            var existingCpt = await _context.CPTs
                .FirstOrDefaultAsync(c => c.CPTCode == dto.CPTCode, cancellationToken);

            if (existingCpt != null)
                return ServiceResult<CPTListItemDto>.Fail(ServiceErrorType.Conflict, "CPTCodeExists");

            // Verify Status exists
            var status = await _context.Statuses
                .FirstOrDefaultAsync(s => s.Id == dto.StatusId, cancellationToken);

            if (status == null)
                return ServiceResult<CPTListItemDto>.Fail(ServiceErrorType.NotFound, "StatusNotFound");

            // Verify Category exists if provided
            if (dto.CategoryId.HasValue)
            {
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == dto.CategoryId.Value, cancellationToken);

                if (category == null)
                    return ServiceResult<CPTListItemDto>.Fail(ServiceErrorType.NotFound, "CategoryNotFound");
            }

            var entity = new CPT
            {
                ArName = dto.ArName.Trim(),
                EnName = dto.EnName.Trim(),
                CPTCode = dto.CPTCode.Trim(),
                CPTDescription = string.IsNullOrWhiteSpace(dto.CPTDescription) ? null : dto.CPTDescription.Trim(),
                StatusId = dto.StatusId,
                CategoryId = dto.CategoryId,
                ICHI = string.IsNullOrWhiteSpace(dto.ICHI) ? null : dto.ICHI.Trim()
            };

            _context.CPTs.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            // Load the entity with related data for response
            var createdCpt = await _context.CPTs
                .Include(c => c.Status)
                .Include(c => c.Category)
                .FirstOrDefaultAsync(c => c.Id == entity.Id, cancellationToken);

            // Count price list services for this CPT (should be 0 for newly created CPT)
            var countInPriceList = await _context.ProviderPriceListServices
                .CountAsync(p => p.CptId == createdCpt!.Id, cancellationToken);

            var result = new CPTListItemDto
            {
                Id = createdCpt!.Id,
                ArName = createdCpt.ArName,
                EnName = createdCpt.EnName,
                CPTCode = createdCpt.CPTCode,
                CPTDescription = createdCpt.CPTDescription,
                StatusId = createdCpt.StatusId,
                StatusName = lang == "ar" ? createdCpt.Status!.NameAr : createdCpt.Status!.NameEn,
                CategoryId = createdCpt.CategoryId,
                CategoryName = createdCpt.Category != null ? createdCpt.Category.Name : null,
                ICHI = createdCpt.ICHI,
                CountInPriceList = countInPriceList
            };

            return ServiceResult<CPTListItemDto>.Ok(result);
        }
    }
}

