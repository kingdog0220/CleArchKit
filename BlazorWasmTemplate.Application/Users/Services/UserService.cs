using BlazorWasmTemplate.Application.Users.Cache;
using BlazorWasmTemplate.Domain.Users.Entities;
using BlazorWasmTemplate.Domain.Users.Repositories;

namespace BlazorWasmTemplate.Application.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IUserCache _cache;

        public UserService(IUserRepository userRepository, IUserCache userCache)
        {
            _repository = userRepository;
            _cache = userCache;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _cache.GetAllAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _cache.GetByIdAsync(id);
        }

        public async Task AddAsync(User user)
        {
            await _repository.AddAsync(user);
            await _cache.RefreshAsync();
        }

        public async Task UpdateAsync(User user)
        {
            await _repository.UpdateAsync(user);
            await _cache.RefreshAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
            await _cache.RefreshAsync();
        }

    }
}