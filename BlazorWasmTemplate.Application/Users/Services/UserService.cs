using BlazorWasmTemplate.Application.Users.Cache;
using BlazorWasmTemplate.Domain.Users.Entities;
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
        /// コンストラクタ
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="userCache"></param>
        public UserService(IUserRepository userRepository, IUserCache userCache)
        {
            _repository = userRepository;
            _cache = userCache;
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
        public async Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null)
        {
            return await _repository.ExistsByCodeAsync(code, excludeId);
        }

    }
}