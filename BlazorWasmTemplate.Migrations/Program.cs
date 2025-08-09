using BlazorWasmTemplate.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace BlazorWasmTemplate.Migrations
{
    class Program
    {
        static void Main(string[] args)
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

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    var connectionString = configuration.GetConnectionString("DefaultConnection");
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseNpgsql(connectionString));
                })
                .Build();

            using var scope = host.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // マイグレーションを適用
            context.Database.Migrate();

            Console.WriteLine("Migrations applied successfully.");

        }
    }
}

