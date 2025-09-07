using CleArchKit.Domain.Users.Entities;
using CleArchKit.Domain.Users.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace CleArchKit.Application.Users.Cache
{
    /// <inheritdoc/>
    public class UserCache : IUserCache
    {
        /// <summary>
        /// キャッシュ
        /// </summary>
        private readonly IMemoryCache _cache;

        /// <summary>
        /// スコープ
        /// </summary>
        private readonly IServiceScopeFactory _scopeFactory;

        /// <summary>
        /// キャッシュキー
        /// </summary>
        private const string CACHE_KEY = "UserCache";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="scopeFactory"></param>
        public UserCache(IMemoryCache cache, IServiceScopeFactory scopeFactory)
        {
            _cache = cache;
            _scopeFactory = scopeFactory;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            if (!_cache.TryGetValue(CACHE_KEY, out IEnumerable<User>? users))
            {
                await RefreshAsync();
                _cache.TryGetValue(CACHE_KEY, out users);
            }

            return users!;
        }

        /// <inheritdoc/>
        public async Task<User?> GetByIdAsync(Guid id)
        {
            var users = await this.GetAllAsync();
            return users.FirstOrDefault(u => u.Id == id);
        }

        /// <inheritdoc/>
        public async Task RefreshAsync()
        {
            // スコープを作ってScopedサービスを取得
            using (var scope = _scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var users = await repository.GetAllAsync();
                _cache.Set(CACHE_KEY, users);
            }
        }
    }
}