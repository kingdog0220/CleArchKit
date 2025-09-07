using CleArchKit.Infrastructure.Persistence.Postgresql;
using Microsoft.EntityFrameworkCore;

namespace CleArchKit.MigrationsChecker
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                IConfiguration configuration = builder.Configuration;

                var host = Host.CreateDefaultBuilder(args)
                    .ConfigureServices(services =>
                    {
                        var connectionString = configuration.GetConnectionString("DefaultConnection");
                        services.AddDbContext<AppDbContext>(options =>
                            options.UseNpgsql(connectionString,
                            b => b.MigrationsAssembly("CleArchKit.Migrations")));
                    })
                    .Build();

                using var scope = host.Services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var pendingMigrations = context.Database.GetPendingMigrations();

                if (pendingMigrations.Any())
                {
                    Console.Error.WriteLine("ERROR: Pending migrations exist:");
                    foreach (var migration in pendingMigrations)
                    {
                        Console.Error.WriteLine($"  - {migration}");
                    }
                    return 1;
                }

                Console.WriteLine("All migrations are applied.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Exception during migration check.");
                Console.Error.WriteLine(ex);
                return 1;
            }

        }
    }
}

