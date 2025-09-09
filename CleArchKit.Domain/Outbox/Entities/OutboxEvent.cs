using System.Text.Json;
using CleArchKit.Domain.Events;

namespace CleArchKit.Domain.Outbox.Entities
{
    /// <summary>
    /// Outboxイベントエンティティ
    /// </summary>
    public class OutboxEvent
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// イベントタイプ
        /// </summary>
        public string EventType { get; private set; } = string.Empty;

        /// <summary>
        /// イベントデータ（JSON形式）
        /// </summary>
        public string? EventData { get; private set; }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// 処理済みフラグ
        /// </summary>
        public bool IsProcessed { get; private set; }

        /// <summary>
        /// 処理日時
        /// </summary>
        public DateTime? ProcessedAt { get; private set; }

        /// <summary>
        /// 再試行回数
        /// </summary>
        public int RetryCount { get; private set; }

        /// <summary>
        /// 最後のエラーメッセージ
        /// </summary>
        public string? LastError { get; private set; }

        /// <summary>
        /// 次回処理予定日時
        /// </summary>
        public DateTime? NextRetryAt { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <remarks>ORM用</remarks>
        protected OutboxEvent() { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="eventType">イベントタイプ</param>
        /// <param name="eventData">イベントデータ</param>
        public OutboxEvent(Guid id, string eventType, string? eventData = null)
        {
            Id = id;
            EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));

            if (eventData != null && !IsValidJson(eventData))
            {
                throw new ArgumentException("Invalid JSON format", nameof(eventData));
            }
            EventData = eventData ?? "{}";
            CreatedAt = DateTime.UtcNow;
            IsProcessed = false;
            RetryCount = 0;
        }

        /// <summary>
        /// 処理完了をマークする
        /// </summary>
        public void MarkAsProcessed()
        {
            IsProcessed = true;
            ProcessedAt = DateTime.UtcNow;
            LastError = null;
            NextRetryAt = null;
        }

        /// <summary>
        /// 処理失敗をマークする
        /// </summary>
        /// <param name="error">エラーメッセージ</param>
        /// <param name="nextRetryAt">次回再試行日時</param>
        public void MarkAsFailed(string error, DateTime? nextRetryAt = null)
        {
            RetryCount++;
            LastError = error;
            NextRetryAt = nextRetryAt;
        }

        /// <summary>
        /// 再試行可能かどうかを判定する
        /// </summary>
        /// <param name="maxRetryCount">最大再試行回数</param>
        /// <returns>再試行可能な場合はtrue</returns>
        public bool CanRetry(int maxRetryCount)
        {
            return !IsProcessed && RetryCount < maxRetryCount && (NextRetryAt == null || NextRetryAt <= DateTime.UtcNow);
        }

        /// <summary>
        /// JSON形式に変換できるかどうかを判定する
        /// </summary>
        /// <param name="json"></param>
        /// <returns>変換可能な場合はtrue</returns>
        private bool IsValidJson(string? json)
        {
            if (json == null)
            {
                return false;
            }

            try
            {
                JsonDocument.Parse(json);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }
    }
}