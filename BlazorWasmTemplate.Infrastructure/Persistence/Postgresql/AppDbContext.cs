using BlazorWasmTemplate.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazorWasmTemplate.Infrastructure.Persistence.Postgresql
{
    /// <summary>
    /// DBコンテキスト
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// ユーザーエンティティの DbSet
        /// </summary>
        public DbSet<User> Users => Set<User>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="options">DbContextOptions</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /// <summary>
        /// モデルの作成時に呼ばれるメソッド
        /// </summary>
        /// <param name="modelBuilder">ModelBuilder</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("tpl");

            // すべてのエンティティのテーブル名を小文字に変換
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // テーブル名を小文字に設定
                var tableName = entity.GetTableName();
                if (tableName != null)
                {
                    entity.SetTableName(tableName.ToLower());
                }

                // カラム名も小文字にする
                foreach (var property in entity.GetProperties())
                {
                    if (property != null)
                    {
                        property.SetColumnName(property.GetColumnName().ToLower());
                    }
                }
            }

            // Fluent API設定が必要であればここで行う
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Code)
                    .IsUnique();
            });
        }
    }
}
