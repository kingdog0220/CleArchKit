namespace BlazorWasmTemplate.Domain.Events
{
    /// <summary>
    /// ドメインイベント
    /// </summary>
    public interface IDomainEvent { }

    /// <summary>
    /// ドメインイベントをメモリ内でディスパッチする
    /// </summary>
    public interface IDomainEventDispatcher
    {
        /// <summary>
        /// イベントをディスパッチする
        /// </summary>
        /// <param name="domainEvent">イベント</param>
        /// <param name="cancellationToken"></param>
        Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
    }
}