using BlazorWasmTemplate.Domain.Users.Entities;
using BlazorWasmTemplate.Domain.Users.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorWasmTemplate.Application.Users.Cache
{
    public class UserCache : IUserCache
    {
        private readonly IUserRepository _repository;
        private readonly IMemoryCache _cache;

        private const string CACHE_KEY = "UserCache";

        public UserCache(IUserRepository repository, IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            if (!_cache.TryGetValue(CACHE_KEY, out IEnumerable<User>? users))
            {
                users = await _repository.GetAllAsync();
                _cache.Set(CACHE_KEY, users);
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
            var users = await _repository.GetAllAsync();
            _cache.Set(CACHE_KEY, users);
        }
    }
}