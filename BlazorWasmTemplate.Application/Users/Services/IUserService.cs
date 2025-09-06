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
        /// <param name="id">物理ID</param>
        /// <param name="code"></param>
        /// <returns>すでに存在すればTrue</returns>
        Task<bool> ExistsByCodeAsync(Guid id, string code);
    }
}