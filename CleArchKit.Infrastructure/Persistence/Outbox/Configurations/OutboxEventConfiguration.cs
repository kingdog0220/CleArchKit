using CleArchKit.Domain.Outbox.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleArchKit.Infrastructure.Persistence.Outbox.Configurations
{
    /// <summary>
    /// OutboxEventエンティティのEF Core設定
    /// </summary>
    public class OutboxEventConfiguration : IEntityTypeConfiguration<OutboxEvent>
    {
        public void Configure(EntityTypeBuilder<OutboxEvent> builder)
        {
            // テーブル名
            builder.ToTable("outbox_events");

            // 主キー
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                .IsRequired();

            // イベントタイプ
            builder.Property(e => e.EventType)
                .HasColumnName("event_type")
                .HasMaxLength(255)
                .IsRequired();

            // イベントデータ
            builder.Property(e => e.EventData)
                .HasColumnName("event_data")
                .HasColumnType("text");

            // 作成日時
            builder.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            // 処理済みフラグ
            builder.Property(e => e.IsProcessed)
                .HasColumnName("is_processed")
                .IsRequired()
                .HasDefaultValue(false);

            // 処理日時
            builder.Property(e => e.ProcessedAt)
                .HasColumnName("processed_at")
                .HasColumnType("timestamp with time zone");

            // 再試行回数
            builder.Property(e => e.RetryCount)
                .HasColumnName("retry_count")
                .IsRequired()
                .HasDefaultValue(0);

            // 最後のエラーメッセージ
            builder.Property(e => e.LastError)
                .HasColumnName("last_error")
                .HasColumnType("text");

            // 次回処理予定日時
            builder.Property(e => e.NextRetryAt)
                .HasColumnName("next_retry_at")
                .HasColumnType("timestamp with time zone");

            // インデックス
            builder.HasIndex(e => new { e.IsProcessed, e.CreatedAt })
                .HasDatabaseName("ix_outbox_events_is_processed_created_at");

            builder.HasIndex(e => e.NextRetryAt)
                .HasDatabaseName("ix_outbox_events_next_retry_at");

            builder.HasIndex(e => e.EventType)
                .HasDatabaseName("ix_outbox_events_event_type");
        }
    }
}