using System.Linq;
using MCIApi.Application.Common;
using MCIApi.Application.Relations.DTOs;
using MCIApi.Application.Relations.Interfaces;
using MCIApi.Domain.Abstractions;
using MCIApi.Domain.Entities;

namespace MCIApi.Infrastructure.Services
{
    public class RelationService : IRelationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RelationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<IReadOnlyList<RelationDto>>> GetAllAsync(string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<Relation>();
            var relations = await repo.ListAsync(cancellationToken);
            var data = relations
                .OrderBy(r => r.Name)
                .Select(Map)
                .ToList()
                .AsReadOnly();

            return ServiceResult<IReadOnlyList<RelationDto>>.Ok(data);
        }

        public async Task<ServiceResult<RelationDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<Relation>();
            var relation = await repo.GetByIdAsync(id, cancellationToken);
            if (relation == null)
                return ServiceResult<RelationDto>.Fail(ServiceErrorType.NotFound, "RelationNotFound");

            return ServiceResult<RelationDto>.Ok(Map(relation));
        }

        public async Task<ServiceResult<RelationDto>> CreateAsync(RelationCreateDto dto, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<Relation>();

            var entity = new Relation
            {
                Name = dto.Name.Trim()
            };

            await repo.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult<RelationDto>.Ok(Map(entity));
        }

        public async Task<ServiceResult<RelationDto>> UpdateAsync(int id, RelationUpdateDto dto, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<Relation>();
            var relation = await repo.GetByIdAsync(id, cancellationToken);
            if (relation == null)
                return ServiceResult<RelationDto>.Fail(ServiceErrorType.NotFound, "RelationNotFound");

            relation.Name = dto.Name.Trim();
            repo.Update(relation);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult<RelationDto>.Ok(Map(relation));
        }

        public async Task<ServiceResult> DeleteAsync(int id, string lang, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<Relation>();
            var relation = await repo.GetByIdAsync(id, cancellationToken);
            if (relation == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "RelationNotFound");

            repo.Delete(relation);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }

        private static RelationDto Map(Relation relation) => new RelationDto
        {
            Id = relation.Id,
            Name = relation.Name
        };
    }
}

