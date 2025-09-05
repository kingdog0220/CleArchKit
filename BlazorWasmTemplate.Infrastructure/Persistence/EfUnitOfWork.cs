using BlazorWasmTemplate.Application.Persistence;
using BlazorWasmTemplate.Domain.Events;
using BlazorWasmTemplate.Infrastructure.Persistence.Postgresql;

namespace BlazorWasmTemplate.Infrastructure.Persistence
{
    /// <inheritdoc/>
    public class EfUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="dbContext"></param>
        public EfUnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc/>
        public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            var result = await _dbContext.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}