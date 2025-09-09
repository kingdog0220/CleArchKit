using CleArchKit.Domain.Outbox.Entities;
using CleArchKit.Domain.Outbox.Repositories;
using CleArchKit.Infrastructure.Persistence.Postgresql;
using Microsoft.EntityFrameworkCore;

namespace CleArchKit.Infrastructure.Persistence.Outbox.Repositories
{
    /// <inheritdoc/>
    public class OutboxEventRepository : IOutboxEventRepository
    {
        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="dbContext">データベースコンテキスト</param>
        public OutboxEventRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc/>
        public async Task AddAsync(OutboxEvent outboxEvent)
        {
            await _dbContext.OutboxEvents.AddAsync(outboxEvent);
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(OutboxEvent outboxEvent)
        {
            _dbContext.OutboxEvents.Update(outboxEvent);
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task DeleteProcessedEventsOlderThanAsync(DateTime olderThan)
        {
            var eventsToDelete = await _dbContext.OutboxEvents
                .Where(e => e.IsProcessed && e.ProcessedAt < olderThan)
                .ToListAsync();

            _dbContext.OutboxEvents.RemoveRange(eventsToDelete);
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<OutboxEvent>> GetUnprocessedEventsAsync(int maxRetryCount = 3, int batchSize = 100)
        {
            var now = DateTime.UtcNow;

            return await _dbContext.OutboxEvents
                .Where(e => !e.IsProcessed && e.RetryCount < maxRetryCount && (e.NextRetryAt == null || e.NextRetryAt <= now))
                .OrderBy(e => e.CreatedAt)
                .Take(batchSize)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}