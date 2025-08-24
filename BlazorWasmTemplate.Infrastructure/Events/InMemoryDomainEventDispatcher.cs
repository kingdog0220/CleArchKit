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
        public async Task DispatchAsync<TEvent>(TEvent @event) where TEvent : class
        {
            var handlers = _serviceProvider.GetServices<IEventHandler<TEvent>>();
            foreach (var handler in handlers)
            {
                await handler.Handle(@event);
            }
        }
    }
}
