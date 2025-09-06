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
            return await _dbContext.Users.ToListAsync();
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
        public async Task<bool> ExistsByCodeAsync(Guid id, string code)
        {
            return await _dbContext.Users.Where(e => e.Id != id && e.Code == code).AsNoTracking().AnyAsync();
        }
    }
}