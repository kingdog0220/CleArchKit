using BlazorWasmTemplate.Application.Users.Dtos;

namespace BlazorWasmTemplate.Application.Users.UseCases.Command
{
    /// <summary>
    /// ユーザーコマンド ユースケース
    /// </summary>
    public interface IUserCommandUseCase
    {
        /// <summary>
        /// ユーザー登録ユースケースを実行する
        /// </summary>
        /// <param name="userDto"></param>
        Task CreateAsync(UserDto userDto);

        /// <summary>
        /// ユーザー更新ユースケースを実行する
        /// </summary>
        /// <param name="userDto"></param>
        Task UpdateAsync(UserDto userDto);

        /// <summary>
        /// ユーザー削除ユースケースを実行する
        /// </summary>
        /// <param name="userDto"></param>
        Task DeleteAsync(UserDto userDto);
    }
}