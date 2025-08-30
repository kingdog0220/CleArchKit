using BlazorWasmTemplate.Domain.Events;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorWasmTemplate.Infrastructure.Events
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
            var eventType = domainEvent.GetType();
            var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
            var handlers = _serviceProvider.GetServices(handlerType);

            var method = handlerType.GetMethod(nameof(IEventHandler<IDomainEvent>.HandleAsync));

            foreach (var handler in handlers)
            {
                if (method != null)
                {
                    await (Task)method.Invoke(handler, [domainEvent, cancellationToken])!;
                }
            }
        }
    }
}
