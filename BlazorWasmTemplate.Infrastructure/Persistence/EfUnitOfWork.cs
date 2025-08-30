using BlazorWasmTemplate.Domain.Events;
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
        /// ドメインイベントディスパッチャ
        /// </summary>
        private readonly IDomainEventDispatcher _dispatcher;

        /// <summary>
        /// ドメインイベントリスト
        /// </summary>
        /// <returns></returns>
        private readonly List<IDomainEvent> _eventBuffers = new();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="dispatcher"></param>
        public EfUnitOfWork(AppDbContext dbContext, IDomainEventDispatcher dispatcher)
        {
            _dbContext = dbContext;
            _dispatcher = dispatcher;
        }

        /// <inheritdoc/>
        public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            var result = await _dbContext.SaveChangesAsync(cancellationToken);

            // コミット後にバッファ内のイベントを発火
            foreach (var domainEvent in _eventBuffers)
            {
                await _dispatcher.DispatchAsync(domainEvent);
            }
            _eventBuffers.Clear();

            return result;
        }

        /// <summary>
        /// ドメインイベントをバッファに追加する
        /// </summary>
        /// <param name="domainEvent"></param>
        public void EnqueueEvent(IDomainEvent domainEvent)
        {
            _eventBuffers.Add(domainEvent);
        }
    }
}