using BlazorWasmTemplate.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazorWasmTemplate.Infrastructure.Persistence.Users.Configurations
{
    /// <summary>
    /// ユーザーエンティティの構成
    /// </summary>
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        /// <summary>
        /// ユーザーエンティティの構成
        /// </summary>
        /// <param name="builder">EntityTypeBuilder</param>
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // テーブル名
            builder.ToTable("users");

            // 主キー
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id);

            // Code
            builder.Property(u => u.Code)
                .IsRequired()
                .HasMaxLength(10);

            // Name
            builder.Property(u => u.Name)
                .HasMaxLength(50);

            // IsActive
            builder.Property(u => u.IsActive)
                .IsRequired()
                .HasColumnName("is_active");

            //ユニークキー制約
            builder.HasIndex(u => u.Code).IsUnique();
        }
    }
}
