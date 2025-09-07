using CleArchKit.Application.Events;
using CleArchKit.Application.Users.Dtos;
using CleArchKit.Application.Users.Services;
using CleArchKit.Domain.Users.Repositories;

namespace CleArchKit.Application.Users.UseCases.Command
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
        public UserCommandUseCase(IUserRepository userRepository, IUserService userService, IDomainEventBuffer eventBuffer)
        {
            _userRepository = userRepository;
            _userService = userService;
            _eventBuffer = eventBuffer;
        }

        /// <inheritdoc/>
        public async Task CreateAsync(CreateUserDto createUserDto)
        {
            var createUser = createUserDto.ToEntity();
            var user = await _userService.GetByIdAsync(createUser.Id);
            if (user != null)
            {
                throw new Exception($"主キーが重複しています:{createUser.Id}");
            }

            var existCode = await _userService.ExistsByCodeAsync(createUser.Code);
            if (existCode)
            {
                throw new Exception($"CODEが重複しています:{createUser.Code}");
            }

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

            var existCode = await _userService.ExistsByCodeAsync(userDto.Code, userDto.Id);
            if (existCode)
            {
                throw new Exception($"CODEが重複しています:{userDto.Code}");
            }

            var updateUser = userDto.ToEntity();
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

            var deleteUser = userDto.ToEntity();
            var evt = deleteUser.PublishUserUpdatedEvent();
            _eventBuffer.EnqueueEvent(evt);
            await _userRepository.DeleteAsync(deleteUser);
        }

    }
}