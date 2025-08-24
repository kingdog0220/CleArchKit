using BlazorWasmTemplate.Domain.Users.Entities;

namespace BlazorWasmTemplate.Application.Users.Services
{
    /// <summary>
    /// ユーザーサービス
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 全件取得
        /// </summary>
        /// <returns>ユーザーリスト</returns>
        Task<IEnumerable<User>> GetAllAsync();

        /// <summary>
        /// IDによるユーザー取得
        /// </summary>
        /// <param name="id">物理ID</param>
        /// <returns>ユーザー</returns>
        Task<User?> GetByIdAsync(Guid id);

        /// <summary>
        /// ユーザー登録
        /// </summary>
        /// <param name="user"></param>
        Task AddAsync(User user);

        /// <summary>
        /// ユーザー更新
        /// </summary>
        /// <param name="user"></param>
        Task UpdateAsync(User user);

        /// <summary>
        /// ユーザー削除
        /// </summary>
        /// <param name="id">物理ID</param>
        Task DeleteAsync(Guid id);
    }
}