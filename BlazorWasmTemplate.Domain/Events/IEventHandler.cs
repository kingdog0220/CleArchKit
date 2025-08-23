namespace BlazorWasmTemplate.Domain.Events
{
    public interface IEventHandler<in TEvent> where TEvent : class
    {
        Task Handle(TEvent @event);
    }
}