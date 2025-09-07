using CleArchKit.Domain.Users.Entities;

namespace CleArchKit.Domain.Users.Repositories
{
    /// <summary>
    /// ユーザーリポジトリ
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// 全件取得
        /// </summary>
        /// <returns>ユーザーリスト</returns>
        Task<List<User>> GetAllAsync();

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
        /// <param name="user"></param>
        Task DeleteAsync(User user);

        /// <summary>
        /// コードが存在するかチェックする
        /// </summary>
        /// <param name="code"></param>
        /// <param name="id">更新時に自分自身を除外するID（登録時は null）</param>
        /// <returns>すでに存在すればTrue</returns>
        Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null);
    }
}