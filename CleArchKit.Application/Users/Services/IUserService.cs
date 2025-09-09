namespace CleArchKit.Application.Users.Services
{
    /// <summary>
    /// ユーザーサービス
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// コードが存在するかチェックする
        /// </summary>
        /// <param name="code"></param>
        /// <param name="id">更新時に自分自身を除外するID（登録時は null）</param>
        /// <returns>すでに存在すればTrue</returns>
        Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null);
    }
}