using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MCIApi.Application.Common;
using MCIApi.Application.Medicines.DTOs;
using MCIApi.Application.Medicines.Helpers;
using MCIApi.Application.Medicines.Interfaces;
using MCIApi.Domain.Entities;
using MCIApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MCIApi.Infrastructure.Services
{
    public class MedicineService : IMedicineService
    {
        private readonly AppDbContext _context;

        public MedicineService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<IReadOnlyList<MedicineListDto>>> GetAllMedicinesAsync(string lang, CancellationToken cancellationToken = default)
        {
            var medicines = await _context.Medicines
                .Include(m => m.Unit1)
                .Include(m => m.Unit2)
                .Where(m => !m.IsDeleted)
                .ToListAsync(cancellationToken);

            var result = medicines.Select(m => new MedicineListDto
            {
                EnName = m.EnName,
                ArName = m.ArName,
                Price = MedicinePriceHelper.CalculatePrice(
                    m.MedicinePrice,
                    m.Unit1?.NameEn,
                    m.Unit2?.NameEn,
                    m.Unit1Count,
                    m.Unit2Count),
                FullForm = m.FullForm ?? (m.Unit1 != null && m.Unit2 != null 
                    ? $"({m.Unit2Count} {m.Unit2.NameEn}) / ({m.Unit1Count} {m.Unit1.NameEn})" 
                    : null),
                Source = m.IsLocal ? "LOCAL" : "IMPORTED"
            }).ToList();

            return ServiceResult<IReadOnlyList<MedicineListDto>>.Ok(result);
        }

        public async Task<ServiceResult<MedicineReadDto>> CreateAsync(MedicineCreateDto dto, string createdBy, string lang, CancellationToken cancellationToken = default)
        {
            // Validate Unit1Id exists
            var unit1Exists = await _context.Unit1s.AnyAsync(u => u.Id == dto.Unit1Id && !u.IsDeleted, cancellationToken);
            if (!unit1Exists)
                return ServiceResult<MedicineReadDto>.Fail(ServiceErrorType.Validation, "Unit1NotFound");

            // Validate Unit2Id exists
            var unit2Exists = await _context.Unit2s.AnyAsync(u => u.Id == dto.Unit2Id && !u.IsDeleted, cancellationToken);
            if (!unit2Exists)
                return ServiceResult<MedicineReadDto>.Fail(ServiceErrorType.Validation, "Unit2NotFound");

            // Load units to calculate FullForm
            var unit1 = await _context.Unit1s.FindAsync(new object[] { dto.Unit1Id }, cancellationToken);
            var unit2 = await _context.Unit2s.FindAsync(new object[] { dto.Unit2Id }, cancellationToken);

            var medicine = new Medicine
            {
                EnName = dto.EnName.Trim(),
                ArName = dto.ArName.Trim(),
                Unit1Id = dto.Unit1Id,
                Unit2Id = dto.Unit2Id,
                Unit1Count = dto.Unit1Count,
                Unit2Count = dto.Unit2Count,
                IsLocal = dto.IsLocal,
                MedicinePrice = dto.MedicinePrice,
                ActiveIngredient = dto.ActiveIngredient?.Trim(),
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreatedBy = createdBy
            };

            // Calculate FullForm
            medicine.Unit1 = unit1;
            medicine.Unit2 = unit2;
            medicine.CalculateFullForm();

            _context.Medicines.Add(medicine);
            await _context.SaveChangesAsync(cancellationToken);

            // Reload with navigation properties
            var createdMedicine = await _context.Medicines
                .Include(m => m.Unit1)
                .Include(m => m.Unit2)
                .FirstOrDefaultAsync(m => m.Id == medicine.Id, cancellationToken);

            if (createdMedicine == null)
                return ServiceResult<MedicineReadDto>.Fail(ServiceErrorType.Unexpected, "MedicineCreationFailed");

            var result = new MedicineReadDto
            {
                Id = createdMedicine.Id,
                EnName = createdMedicine.EnName,
                ArName = createdMedicine.ArName,
                Unit1Id = createdMedicine.Unit1Id,
                Unit1Name = createdMedicine.Unit1?.NameEn,
                Unit2Id = createdMedicine.Unit2Id,
                Unit2Name = createdMedicine.Unit2?.NameEn,
                Unit1Count = createdMedicine.Unit1Count,
                Unit2Count = createdMedicine.Unit2Count,
                FullForm = createdMedicine.FullForm,
                IsLocal = createdMedicine.IsLocal,
                MedicinePrice = MedicinePriceHelper.CalculatePrice(
                    createdMedicine.MedicinePrice,
                    createdMedicine.Unit1?.NameEn,
                    createdMedicine.Unit2?.NameEn,
                    createdMedicine.Unit1Count,
                    createdMedicine.Unit2Count),
                ActiveIngredient = createdMedicine.ActiveIngredient
            };

            return ServiceResult<MedicineReadDto>.Ok(result);
        }
   
    }
}

