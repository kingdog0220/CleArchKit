using BlazorWasmTemplate.Domain.Users.Entities;
using BlazorWasmTemplate.Domain.Users.Repositories;
using BlazorWasmTemplate.Infrastructure.Persistence.Postgresql;
using Microsoft.EntityFrameworkCore;

namespace BlazorWasmTemplate.Infrastructure.Persistence.Users.Repositories
{
    /// <inheritdoc/>
    public class UserRepository : IUserRepository
    {
        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="dbContext"></param>
        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc/>
        public async Task<List<User>> GetAllAsync()
        {
            return await _dbContext.Users.AsNoTracking().ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Users.FindAsync(id);
        }

        /// <inheritdoc/>
        public async Task AddAsync(User user)
        {
            _dbContext.Users.Add(user);
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(User user)
        {
            _dbContext.Users.Update(user);
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(User user)
        {
            _dbContext.Users.Remove(user);
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null)
        {
            var query = _dbContext.Users.AsNoTracking().Where(u => u.Code == code);

            if (excludeId.HasValue) {
                query = query.Where(u => u.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}