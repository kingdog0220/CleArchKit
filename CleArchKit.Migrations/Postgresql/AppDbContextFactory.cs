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
            var builder = WebApplication.CreateBuilder(args);

            IConfiguration configuration = builder.Configuration;

            var connectionString = configuration.GetConnectionString("DefaultConnection");


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