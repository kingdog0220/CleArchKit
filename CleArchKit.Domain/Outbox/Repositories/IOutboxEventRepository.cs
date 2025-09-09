using CleArchKit.Domain.Outbox.Entities;

namespace CleArchKit.Domain.Outbox.Repositories
{
    /// <summary>
    /// Outboxイベントリポジトリ
    /// </summary>
    public interface IOutboxEventRepository
    {
        /// <summary>
        /// Outboxイベントを追加する
        /// </summary>
        /// <param name="outboxEvent">Outboxイベント</param>
        Task AddAsync(OutboxEvent outboxEvent);

        /// <summary>
        /// Outboxイベントを更新する
        /// </summary>
        /// <param name="outboxEvent">Outboxイベント</param>
        Task UpdateAsync(OutboxEvent outboxEvent);

        /// <summary>
        /// 処理済みの古いOutboxイベントを削除する
        /// </summary>
        /// <param name="olderThan">この日時より古いイベントを削除</param>
        Task DeleteProcessedEventsOlderThanAsync(DateTime olderThan);

        /// <summary>
        /// 未処理のOutboxイベントを取得する
        /// </summary>
        /// <param name="maxRetryCount">最大再試行回数</param>
        /// <param name="batchSize">バッチサイズ</param>
        /// <returns>未処理のOutboxイベントリスト</returns>
        Task<IEnumerable<OutboxEvent>> GetUnprocessedEventsAsync(int maxRetryCount = 3, int batchSize = 100);
    }
}