namespace BlazorWasmTemplate.Domain.Events
{
    /// <summary>
    /// ドメインイベントをメモリ内でディスパッチするクラス
    /// </summary>
    public interface IDomainEventDispatcher
    {
        /// <summary>
        /// イベントをディスパッチする
        /// </summary>
        /// <typeparam name="TEvent">イベントの型</typeparam>
        /// <param name="event">イベント</param>
        Task DispatchAsync<TEvent>(TEvent @event) where TEvent : class;
    }
}