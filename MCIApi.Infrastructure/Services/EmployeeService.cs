using System;
using System.IO;
using System.Linq;
using BCrypt.Net;
using MCIApi.Application.Common;
using MCIApi.Application.Employees.DTOs;
using MCIApi.Application.Employees.Interfaces;
using MCIApi.Domain.Abstractions;
using MCIApi.Domain.Entities;
using MCIApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MCIApi.Infrastructure.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public EmployeeService(
            AppDbContext context,
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _environment = environment;
        }

        public async Task<ServiceResult<IReadOnlyList<EmployeeDto>>> GetAllAsync(string lang, CancellationToken cancellationToken = default)
        {
            var employees = await _context.Employees
                .AsNoTracking()
                .Where(e => !e.IsDeleted)
                .Include(e => e.Department)
                .Include(e => e.JobTitle)
                .ToListAsync(cancellationToken);

            var result = employees.Select(e => MapToDto(e, lang)).ToList().AsReadOnly();
            return ServiceResult<IReadOnlyList<EmployeeDto>>.Ok(result);
        }

        public async Task<ServiceResult<EmployeeDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default)
        {
            var employee = await _context.Employees
                .AsNoTracking()
                .Where(e => e.Id == id && !e.IsDeleted)
                .Include(e => e.Department)
                .Include(e => e.JobTitle)
                .FirstOrDefaultAsync(cancellationToken);

            if (employee == null)
                return ServiceResult<EmployeeDto>.Fail(ServiceErrorType.NotFound, "EmployeeNotFound");

            return ServiceResult<EmployeeDto>.Ok(MapToDto(employee, lang));
        }

        public async Task<ServiceResult<EmployeeDto>> CreateAsync(CreateEmployeeDto dto, string lang, CancellationToken cancellationToken = default)
        {
            if (!await _context.Departments.AnyAsync(d => d.Id == dto.DepartmentId, cancellationToken))
                return ServiceResult<EmployeeDto>.Fail(ServiceErrorType.Validation, "DepartmentNotFound");

            if (!dto.JobTitleId.HasValue || !await _context.JobTitles.AnyAsync(j => j.Id == dto.JobTitleId.Value, cancellationToken))
                return ServiceResult<EmployeeDto>.Fail(ServiceErrorType.Validation, "JobTitleNotFound");

            if (await _context.Employees.AnyAsync(e => e.Email == dto.Email && !e.IsDeleted, cancellationToken))
                return ServiceResult<EmployeeDto>.Fail(ServiceErrorType.Conflict, "EmailAlreadyExists");

            if (!string.IsNullOrEmpty(dto.Mobile) &&
                await _context.Employees.AnyAsync(e => e.Mobile == dto.Mobile && !e.IsDeleted, cancellationToken))
                return ServiceResult<EmployeeDto>.Fail(ServiceErrorType.Conflict, "MobileAlreadyExists");

            var identityUser = new IdentityUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                PhoneNumber = dto.Mobile
            };

            var identityResult = await _userManager.CreateAsync(identityUser, dto.Password);
            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors.Select(e => new ServiceError(e.Code, e.Description));
                return ServiceResult<EmployeeDto>.Fail(ServiceErrorType.Validation, errors, "IdentityError");
            }

            var imagePath = await SaveImageAsync(dto.ImageFile, cancellationToken);

            var employee = new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Mobile = dto.Mobile,
                DepartmentId = dto.DepartmentId,
                JobTitleId = dto.JobTitleId,
                IdentityUserId = identityUser.Id,
                ImageUrl = imagePath,
                IsDeleted = false,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            await _unitOfWork.Repository<Employee>().AddAsync(employee, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // reload with navigation props
            var created = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.JobTitle)
                .FirstAsync(e => e.Id == employee.Id, cancellationToken);

            return ServiceResult<EmployeeDto>.Ok(MapToDto(created, lang));
        }

        public async Task<ServiceResult<EmployeeDto>> UpdateAsync(int id, UpdateEmployeeDto dto, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<Employee>();
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
            if (employee == null || employee.IsDeleted)
                return ServiceResult<EmployeeDto>.Fail(ServiceErrorType.NotFound, "EmployeeNotFound");

            if (dto.DepartmentId.HasValue)
            {
                if (!await _context.Departments.AnyAsync(d => d.Id == dto.DepartmentId.Value, cancellationToken))
                    return ServiceResult<EmployeeDto>.Fail(ServiceErrorType.Validation, "DepartmentNotFound");
                employee.DepartmentId = dto.DepartmentId.Value;
            }

            if (dto.JobTitleId.HasValue)
            {
                if (!await _context.JobTitles.AnyAsync(j => j.Id == dto.JobTitleId.Value, cancellationToken))
                    return ServiceResult<EmployeeDto>.Fail(ServiceErrorType.Validation, "JobTitleNotFound");
                employee.JobTitleId = dto.JobTitleId.Value;
            }

            if (!string.IsNullOrEmpty(dto.Email) &&
                await _context.Employees.AnyAsync(e => e.Email == dto.Email && e.Id != id && !e.IsDeleted, cancellationToken))
                return ServiceResult<EmployeeDto>.Fail(ServiceErrorType.Conflict, "EmailAlreadyExists");

            if (!string.IsNullOrWhiteSpace(dto.Mobile) &&
                await _context.Employees.AnyAsync(e => e.Mobile == dto.Mobile && e.Id != id && !e.IsDeleted, cancellationToken))
                return ServiceResult<EmployeeDto>.Fail(ServiceErrorType.Conflict, "MobileAlreadyExists");

            if (!string.IsNullOrEmpty(dto.FirstName))
                employee.FirstName = dto.FirstName;
            if (!string.IsNullOrEmpty(dto.LastName))
                employee.LastName = dto.LastName;
            if (!string.IsNullOrEmpty(dto.Email))
                employee.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.Mobile))
                employee.Mobile = dto.Mobile;

            if (!string.IsNullOrEmpty(dto.Password))
                employee.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            if (dto.DeleteImage == true && !string.IsNullOrEmpty(employee.ImageUrl))
            {
                DeleteImage(employee.ImageUrl);
                employee.ImageUrl = null;
            }

            if (dto.ImageFile != null)
            {
                var imagePath = await SaveImageAsync(dto.ImageFile, cancellationToken);
                if (!string.IsNullOrEmpty(employee.ImageUrl))
                    DeleteImage(employee.ImageUrl);
                employee.ImageUrl = imagePath;
            }

            repo.Update(employee);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var updated = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.JobTitle)
                .FirstAsync(e => e.Id == employee.Id, cancellationToken);

            return ServiceResult<EmployeeDto>.Ok(MapToDto(updated, lang));
        }

        public async Task<ServiceResult<EmployeeDto>> DeleteAsync(int id, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<Employee>();
            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.JobTitle)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            if (employee == null || employee.IsDeleted)
                return ServiceResult<EmployeeDto>.Fail(ServiceErrorType.NotFound, "EmployeeNotFound");

            employee.IsDeleted = true;
            repo.Update(employee);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult<EmployeeDto>.Ok(MapToDto(employee, lang));
        }

        private EmployeeDto MapToDto(Employee employee, string lang)
        {
            return new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Mobile = employee.Mobile,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department == null
                    ? null
                    : (lang == "ar" ? employee.Department.NameAr : employee.Department.NameEn),
                JobTitleId = employee.JobTitleId,
                JobTitleName = employee.JobTitle == null
                    ? null
                    : (lang == "ar" ? employee.JobTitle.NameAr : employee.JobTitle.NameEn),
                ImageUrl = employee.ImageUrl
            };
        }

        private async Task<string?> SaveImageAsync(IFormFile? file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return null;

            var webRoot = _environment.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRoot))
            {
                webRoot = Path.Combine(_environment.ContentRootPath, "wwwroot");
            }

            var uploadsFolder = Path.Combine(webRoot, "uploads", "employees");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var physicalPath = Path.Combine(uploadsFolder, fileName);

            await using var stream = new FileStream(physicalPath, FileMode.Create);
            await file.CopyToAsync(stream, cancellationToken);

            return Path.Combine("uploads", "employees", fileName).Replace("\\", "/");
        }

        private void DeleteImage(string relativePath)
        {
            var webRoot = _environment.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRoot))
            {
                webRoot = Path.Combine(_environment.ContentRootPath, "wwwroot");
            }

            var physicalPath = Path.Combine(webRoot, relativePath);
            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }
        }
    }
}


