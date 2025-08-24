using BlazorWasmTemplate.Domain.Users.Entities;
using BlazorWasmTemplate.Infrastructure.Persistence.Users.Configurations;
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

            // 全テーブル・全カラムを小文字に変換
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // テーブル名
                entity.SetTableName(entity.GetTableName()?.ToLowerInvariant());

                // カラム名
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.Name.ToLowerInvariant());
                }
            }

            // Fluent API
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
