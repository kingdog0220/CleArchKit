using CleArchKit.Domain.Events;

namespace CleArchKit.Application.Events
{
    /// <summary>
    /// ドメインイベント管理
    /// </summary>
    public interface IDomainEventBuffer
    {
        /// <summary>
        /// ドメインイベントをキューに積む
        /// </summary>
        /// <param name="domainEvent"></param>
        void EnqueueEvent(IDomainEvent domainEvent);

        /// <summary>
        /// イベントを発火する
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task FlushAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// ドメインイベントを初期化する
        /// </summary>
        void Clear();
    }
}