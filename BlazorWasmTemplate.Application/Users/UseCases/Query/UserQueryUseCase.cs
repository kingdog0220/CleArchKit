using BlazorWasmTemplate.Application.Users.Services;
using BlazorWasmTemplate.Domain.Users.Entities;

namespace BlazorWasmTemplate.Application.Users.UseCases.Query
{
    /// <inheritdoc/>
    public class UserQueryUseCase : IUserQueryUseCase
    {
        /// <summary>
        /// ユーザーサービス
        /// </summary>
        IUserService _userService;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="userService"></param>

        public UserQueryUseCase(IUserService userService)
        {
            _userService = userService;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _userService.GetAllAsync();
        }

        /// <inheritdoc/>
        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _userService.GetByIdAsync(id);
        }
    }
}