using BlazorWasmTemplate.Infrastructure.Persistence.Postgresql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BlazorWasmTemplate.Migrations
{
    /// <summary>
    /// dotnet ef コマンド実行時に DbContext を生成するためのファクトリ
    /// </summary>
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            // Webプロジェクトのディレクトリを基準にする
            var solutionDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
            var webProjectPath = Path.Combine(solutionDir, "BlazorWasmTemplate.Presentation.Web");

            var builder = new ConfigurationBuilder()
                    .SetBasePath(webProjectPath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();

            IConfiguration configuration = builder.Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");


            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(
                connectionString,
                // マイグレーションの出力先
                b => b.MigrationsAssembly("BlazorWasmTemplate.Migrations")
            );
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}