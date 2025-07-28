using BlazorWasmTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazorWasmTemplate.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Fluent API設定が必要であればここで行う
        }
    }
}