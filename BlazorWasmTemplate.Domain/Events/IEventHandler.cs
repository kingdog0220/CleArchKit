namespace BlazorWasmTemplate.Domain.Events
{
    /// <summary>
    /// イベントハンドラーインターフェース
    /// </summary>
    /// <typeparam name="TEvent">イベントの型</typeparam>
    public interface IEventHandler<in TEvent> where TEvent : class
    {
        /// <summary>
        /// イベント処理
        /// </summary>
        /// <param name="event">イベント</param>
        Task Handle(TEvent @event);
    }
}
