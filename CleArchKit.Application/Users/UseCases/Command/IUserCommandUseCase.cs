using CleArchKit.Application.Users.Dtos;

namespace CleArchKit.Application.Users.UseCases.Command
{
    /// <summary>
    /// ユーザーコマンド ユースケース
    /// </summary>
    public interface IUserCommandUseCase
    {
        /// <summary>
        /// ユーザー登録ユースケースを実行する
        /// </summary>
        /// <param name="createUserDto"></param>
        Task CreateAsync(CreateUserDto createUserDto);

        /// <summary>
        /// ユーザー更新ユースケースを実行する
        /// </summary>
        /// <param name="updateUserDto"></param>
        Task UpdateAsync(UpdateUserDto updateUserDto);

        /// <summary>
        /// ユーザー削除ユースケースを実行する
        /// </summary>
        /// <param name="userDto"></param>
        Task DeleteAsync(UserDto userDto);
    }
}