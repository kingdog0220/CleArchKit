using BlazorWasmTemplate.Application.Users.Cache;
using BlazorWasmTemplate.Domain.Events;
using BlazorWasmTemplate.Domain.Users.Entities;
using BlazorWasmTemplate.Domain.Users.Events;
using BlazorWasmTemplate.Domain.Users.Repositories;

namespace BlazorWasmTemplate.Application.Users.Services
{
    /// <inheritdoc/>
    public class UserService : IUserService
    {
        /// <summary>
        /// ユーザーリポジトリ
        /// </summary>
        private readonly IUserRepository _repository;

        /// <summary>
        /// ユーザーキャッシュ
        /// </summary>
        private readonly IUserCache _cache;

        /// <summary>
        /// ドメインイベントをメモリ内でディスパッチするクラス
        /// </summary>
        private readonly IDomainEventDispatcher _dispatcher;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="userCache"></param>
        /// <param name="dispatcher"></param>
        public UserService(IUserRepository userRepository, IUserCache userCache, IDomainEventDispatcher dispatcher)
        {
            _repository = userRepository;
            _cache = userCache;
            _dispatcher = dispatcher;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _cache.GetAllAsync();
        }

        /// <inheritdoc/>
        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _cache.GetByIdAsync(id);
        }

        /// <inheritdoc/>
        public async Task AddAsync(User user)
        {
            await _repository.AddAsync(user);
            await _dispatcher.DispatchAsync(new UserUpdatedEvent(user.Id));
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(User user)
        {
            await _repository.UpdateAsync(user);
            await _dispatcher.DispatchAsync(new UserUpdatedEvent(user.Id));
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
            await _dispatcher.DispatchAsync(new UserUpdatedEvent(id));
        }

    }
}