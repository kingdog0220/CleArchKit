using BlazorWasmTemplate.Application.Events;
using BlazorWasmTemplate.Domain.Events;

namespace BlazorWasmTemplate.Infrastructure.Events
{
    /// <inheritdoc/>
    public class DomainEventBuffer : IDomainEventBuffer
    {
        /// <summary>
        /// ドメインイベントリスト
        /// </summary>
        /// <returns></returns>
        private readonly List<IDomainEvent> _eventBuffers = new();

        /// <summary>
        /// ドメインイベントディスパッチャ
        /// </summary>
        private readonly IDomainEventDispatcher _dispatcher;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="dispatcher"></param>
        public DomainEventBuffer(IDomainEventDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        /// <inheritdoc/>
        public void EnqueueEvent(IDomainEvent domainEvent)
        {
            _eventBuffers.Add(domainEvent);
        }

        /// <inheritdoc/>
        public async Task FlushAsync(CancellationToken cancellationToken = default)
        {
            foreach (var domainEvent in _eventBuffers)
            {
                await _dispatcher.DispatchAsync(domainEvent);
            }
            _eventBuffers.Clear();
        }
    }
}