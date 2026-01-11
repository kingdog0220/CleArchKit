using CleArchKit.Infrastructure.Persistence.Postgresql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CleArchKit.Migrations.Postgresql
{
    /// <summary>
    /// dotnet ef コマンド実行時に DbContext を生成するためのファクトリ
    /// </summary>
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // 作業ディレクトリ（dotnet ef 実行位置）
            var basePath = Directory.GetCurrentDirectory();

            // appsettings を直接読む
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddJsonFile("appsettings.Production.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string not found.");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(
                connectionString,
                // マイグレーションの出力先
                b => b.MigrationsAssembly("CleArchKit.Migrations")
            );
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}