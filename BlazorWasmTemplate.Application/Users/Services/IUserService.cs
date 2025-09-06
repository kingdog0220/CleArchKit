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
        /// コードが存在するかチェックする
        /// </summary>
        /// <param name="code"></param>
        /// <param name="id">更新時に自分自身を除外するID（登録時は null）</param>
        /// <returns>すでに存在すればTrue</returns>
        Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null);
    }
}