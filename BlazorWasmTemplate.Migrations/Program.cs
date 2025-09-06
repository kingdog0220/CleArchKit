using BlazorWasmTemplate.Infrastructure.Persistence.Postgresql;
using Microsoft.EntityFrameworkCore;

namespace BlazorWasmTemplate.Migrations
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            IConfiguration configuration = builder.Configuration;

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    var connectionString = configuration.GetConnectionString("DefaultConnection");
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseNpgsql(connectionString,
                        b => b.MigrationsAssembly("BlazorWasmTemplate.Migrations")));
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

