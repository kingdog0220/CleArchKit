using BlazorWasmTemplate.Domain.Users.Entities;

namespace BlazorWasmTemplate.Application.Users.Cache
{
    /// <summary>
    /// ユーザーキャッシュ
    /// </summary>
    public interface IUserCache
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
        /// キャッシュ更新
        /// </summary>
        Task RefreshAsync();

    }
}