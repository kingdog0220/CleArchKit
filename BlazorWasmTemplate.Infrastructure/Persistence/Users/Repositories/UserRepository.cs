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
            var deleteUser = await _dbContext.Users.FindAsync(user.Id);
            if (deleteUser == null)
            {
                throw new NullReferenceException($"対象のユーザーは存在しません。UserId:{user.Id}");
            }

            _dbContext.Users.Remove(deleteUser);

            await Task.CompletedTask;
        }

    }
}