using MCIApi.Domain.Abstractions;

namespace MCIApi.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity);
            if (!_repositories.TryGetValue(type, out var repo))
            {
                repo = new GenericRepository<TEntity>(_context);
                _repositories[type] = repo;
            }

            return (IGenericRepository<TEntity>)repo;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await _context.SaveChangesAsync(cancellationToken);

        public ValueTask DisposeAsync() => _context.DisposeAsync();
    }
}


