namespace BlazorWasmTemplate.Domain.Events
{
    /// <summary>
    /// イベントハンドラーインターフェース
    /// </summary>
    /// <typeparam name="TEvent">イベントの型</typeparam>
    public interface IEventHandler<in TEvent> where TEvent : IDomainEvent
    {
        /// <summary>
        /// イベント処理
        /// </summary>
        /// <param name="event">イベント</param>
        /// <param name="cancellationToken"></param>
        Task HandleAsync(TEvent @event,CancellationToken cancellationToken = default);
    }
}
