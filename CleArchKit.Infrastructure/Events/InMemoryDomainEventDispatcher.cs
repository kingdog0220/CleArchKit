using CleArchKit.Domain.Events;
using Microsoft.Extensions.DependencyInjection;

namespace CleArchKit.Infrastructure.Events
{
    /// <inheritdoc/>
    public class InMemoryDomainEventDispatcher : IDomainEventDispatcher
    {
        /// <summary>
        /// サービスプロバイダー
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="serviceProvider">サービスプロバイダー</param>
        public InMemoryDomainEventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(IEventHandler<>).MakeGenericType(domainEvent.GetType());
            var handlers = _serviceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                // dynamic を使って型安全に HandleAsync を呼ぶ
                if (handler != null)
                {
                    await ((dynamic)handler).HandleAsync((dynamic)domainEvent, cancellationToken);
                }
            }
        }
    }
}
