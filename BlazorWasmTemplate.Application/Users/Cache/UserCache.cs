using BlazorWasmTemplate.Domain.Users.Entities;
using BlazorWasmTemplate.Domain.Users.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorWasmTemplate.Application.Users.Cache
{
    public class UserCache : IUserCache
    {
        private readonly IMemoryCache _cache;

        private readonly IServiceScopeFactory _scopeFactory;

        private const string CACHE_KEY = "UserCache";

        public UserCache(IMemoryCache cache, IServiceScopeFactory scopeFactory)
        {
            _cache = cache;
            _scopeFactory = scopeFactory;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            if (!_cache.TryGetValue(CACHE_KEY, out IEnumerable<User>? users))
            {
                await RefreshAsync();
                _cache.TryGetValue(CACHE_KEY, out users);
            }

            return users!;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            var users = await this.GetAllAsync();
            return users.FirstOrDefault(u => u.Id == id);
        }

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