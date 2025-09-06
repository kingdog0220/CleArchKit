using BlazorWasmTemplate.Application.Events;
using BlazorWasmTemplate.Application.Users.Dtos;
using BlazorWasmTemplate.Application.Users.Services;
using BlazorWasmTemplate.Domain.Users.Entities;
using BlazorWasmTemplate.Domain.Users.Repositories;

namespace BlazorWasmTemplate.Application.Users.UseCases.Command
{
    /// <inheritdoc/>
    public class UserCommandUseCase : IUserCommandUseCase
    {
        /// <summary>
        /// ユーザーリポジトリ
        /// </summary>
        IUserRepository _userRepository;

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
        /// <param name="userRepository"></param>
        /// <param name="userService"></param>
        /// <param name="eventBuffer"></param>
        public UserCommandUseCase(IUserRepository userRepository,IUserService userService, IDomainEventBuffer eventBuffer)
        {
            _userRepository = userRepository;
            _userService = userService;
            _eventBuffer = eventBuffer;
        }

        /// <inheritdoc/>
        public async Task CreateAsync(UserDto userDto)
        {
            var user = await _userService.GetByIdAsync(userDto.Id);
            if (user != null)
            {
                throw new Exception($"主キーが重複しています:{userDto.Id}");
            }

            var createUser = new User(userDto.Id, userDto.Code, userDto.Name, userDto.IsActive, userDto.CreatedAt, userDto.UpdatedAt);
            var evt = createUser.PublishUserUpdatedEvent();
            _eventBuffer.EnqueueEvent(evt);
            await _userRepository.AddAsync(createUser);
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(UserDto userDto)
        {
            var user = await _userService.GetByIdAsync(userDto.Id);
            if (user == null)
            {
                throw new Exception($"ユーザーはいません:{userDto.Id}");
            }

            var updateUser = new User(userDto.Id, userDto.Code, userDto.Name, userDto.IsActive, userDto.CreatedAt, userDto.UpdatedAt);
            var evt = updateUser.PublishUserUpdatedEvent();
            _eventBuffer.EnqueueEvent(evt);
            await _userRepository.UpdateAsync(updateUser);
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(UserDto userDto)
        {
            var user = await _userService.GetByIdAsync(userDto.Id);
            if (user == null)
            {
                throw new Exception($"ユーザーはいません:{userDto.Id}");
            }

            var deleteUser = new User(userDto.Id, userDto.Code, userDto.Name, userDto.IsActive, userDto.CreatedAt, userDto.UpdatedAt);
            var evt = deleteUser.PublishUserUpdatedEvent();
            _eventBuffer.EnqueueEvent(evt);
            await _userRepository.DeleteAsync(deleteUser);
        }

    }
}