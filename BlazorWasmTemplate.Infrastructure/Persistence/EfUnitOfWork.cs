using BlazorWasmTemplate.Domain.Persistence;
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
        public async Task<int> CommitAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}