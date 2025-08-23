using BlazorWasmTemplate.Application.Users.Cache;
using BlazorWasmTemplate.Domain.Events;
using BlazorWasmTemplate.Domain.Users.Entities;
using BlazorWasmTemplate.Domain.Users.Events;
using BlazorWasmTemplate.Domain.Users.Repositories;

namespace BlazorWasmTemplate.Application.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IUserCache _cache;

        private readonly IDomainEventDispatcher _dispatcher;

        public UserService(IUserRepository userRepository, IUserCache userCache, IDomainEventDispatcher dispatcher)
        {
            _repository = userRepository;
            _cache = userCache;
            _dispatcher = dispatcher;
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
            await _dispatcher.DispatchAsync(new UserUpdatedEvent(user.Id));
        }

        public async Task UpdateAsync(User user)
        {
            await _repository.UpdateAsync(user);
            await _dispatcher.DispatchAsync(new UserUpdatedEvent(user.Id));
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
            await _dispatcher.DispatchAsync(new UserUpdatedEvent(id));
        }

    }
}