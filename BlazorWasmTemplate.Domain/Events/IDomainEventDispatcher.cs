namespace BlazorWasmTemplate.Domain.Events
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync<TEvent>(TEvent @event) where TEvent : class;
    }
}