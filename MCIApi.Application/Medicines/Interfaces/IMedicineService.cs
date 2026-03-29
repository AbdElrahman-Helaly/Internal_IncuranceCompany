using MCIApi.Application.Common;
using MCIApi.Application.Medicines.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace MCIApi.Application.Medicines.Interfaces
{
    public interface IMedicineService
    {
        Task<ServiceResult<IReadOnlyList<MedicineListDto>>> GetAllMedicinesAsync(string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<MedicineReadDto>> CreateAsync(MedicineCreateDto dto, string createdBy, string lang, CancellationToken cancellationToken = default);
    }
}

