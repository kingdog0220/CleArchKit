using BlazorWasmTemplate.Application.Persistence;
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
        /// nit of Workパターンを実装するインターフェース
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="userCache"></param>
        /// <param name="unitOfWork"></param>
        public UserService(IUserRepository userRepository, IUserCache userCache, IUnitOfWork unitOfWork)
        {
            _repository = userRepository;
            _cache = userCache;
            _unitOfWork = unitOfWork;
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
            await _unitOfWork.CommitAsync();
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(User user)
        {
            await _repository.UpdateAsync(user);
            await _unitOfWork.CommitAsync();
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(User user)
        {
            await _repository.DeleteAsync(user);
            await _unitOfWork.CommitAsync();
        }

    }
}