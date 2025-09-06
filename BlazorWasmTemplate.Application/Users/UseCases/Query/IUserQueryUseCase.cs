using BlazorWasmTemplate.Domain.Users.Entities;

namespace BlazorWasmTemplate.Application.Users.UseCases.Query
{
    /// <summary>
    /// ユーザークエリ ユースケース
    /// </summary>
    public interface IUserQueryUseCase
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

    }
}