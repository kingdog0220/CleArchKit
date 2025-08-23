using BlazorWasmTemplate.Domain.Events;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorWasmTemplate.Infrastructure.Events
{
    public class InMemoryDomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public InMemoryDomainEventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

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