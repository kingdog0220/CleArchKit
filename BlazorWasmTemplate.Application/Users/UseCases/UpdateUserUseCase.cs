using BlazorWasmTemplate.Application.Events;
using BlazorWasmTemplate.Application.Users.Dtos;
using BlazorWasmTemplate.Application.Users.Services;
using BlazorWasmTemplate.Domain.Users.Entities;

namespace BlazorWasmTemplate.Application.Users.UseCases
{
    /// <summary>
    /// ユーザー更新ユースケース
    /// </summary>
    public class UpdateUserUseCase
    {
        /// <summary>
        /// ユーザーサービス
        /// </summary>
        IUserService _userService;

        /// <summary>
        /// ドメインイベント管理
        /// </summary>
        private readonly IDomainEventBuffer _eventBuffer;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="eventBuffer"></param>
        public UpdateUserUseCase(IUserService userService, IDomainEventBuffer eventBuffer)
        {
            _userService = userService;
            _eventBuffer = eventBuffer;
        }

        /// <summary>
        /// ユーザー更新ユースケースを実行する
        /// </summary>
        /// <param name="userDto"></param>
        public async Task RunAsync(UserDto userDto)
        {
            var user = await _userService.GetByIdAsync(userDto.Id);
            if (user == null)
            {
                throw new Exception($"ユーザーはいません:{userDto.Id}");
            }

            var updateUser = new User(userDto.Id, userDto.Code, userDto.Name, userDto.IsActive, userDto.CreatedAt, userDto.UpdatedAt);
            var evt = updateUser.PublishUserUpdatedEvent();
            _eventBuffer.EnqueueEvent(evt);
            await _userService.UpdateAsync(updateUser);
        }
    }
}