using CleArchKit.Domain.Users.Entities;
using CleArchKit.Domain.Users.Repositories;

namespace CleArchKit.Application.Users.UseCases.Query
{
    /// <inheritdoc/>
    public class UserQueryUseCase : IUserQueryUseCase
    {
        /// <summary>
        /// ユーザーリポジトリ
        /// </summary>
        IUserRepository _userRepository;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="userRepository"></param>

        public UserQueryUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        /// <inheritdoc/>
        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _userRepository.GetByIdAsync(id);
        }
    }
}