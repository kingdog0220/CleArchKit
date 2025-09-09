using CleArchKit.Domain.Users.Repositories;

namespace CleArchKit.Application.Users.Services
{
    /// <inheritdoc/>
    public class UserService : IUserService
    {
        /// <summary>
        /// ユーザーリポジトリ
        /// </summary>
        private readonly IUserRepository _repository;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="userRepository"></param>
        public UserService(IUserRepository userRepository)
        {
            _repository = userRepository;
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null)
        {
            return await _repository.ExistsByCodeAsync(code, excludeId);
        }

    }
}