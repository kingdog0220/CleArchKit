using CleArchKit.Application.Persistence;
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
        /// Unit of Workパターン
        /// </summary>
        IUnitOfWork _unitOfWork;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="userService"></param>
        /// <param name="unitOfWork"></param>
        public UserCommandUseCase(IUserRepository userRepository, IUserService userService, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        /// <inheritdoc/>
        public async Task CreateAsync(CreateUserDto createUserDto)
        {
            var createUser = createUserDto.ToEntity();
            var user = await _userRepository.GetByIdAsync(createUser.Id);
            if (user != null)
            {
                throw new Exception($"主キーが重複しています:{createUser.Id}");
            }

            var existCode = await _userService.ExistsByCodeAsync(createUser.Code);
            if (existCode)
            {
                throw new Exception($"CODEが重複しています:{createUser.Code}");
            }

            await _userRepository.AddAsync(createUser);
            await _unitOfWork.CommitAsync();
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetByIdAsync(updateUserDto.Id);
            if (user == null)
            {
                throw new Exception($"ユーザーはいません:{updateUserDto.Id}");
            }

            var existCode = await _userService.ExistsByCodeAsync(updateUserDto.Code, updateUserDto.Id);
            if (existCode)
            {
                throw new Exception($"CODEが重複しています:{updateUserDto.Code}");
            }

            var updateUser = updateUserDto.ToEntity();
            await _userRepository.UpdateAsync(updateUser);
            await _unitOfWork.CommitAsync();
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new Exception($"ユーザーはいません:{id}");
            }

            await _userRepository.DeleteAsync(user);
            await _unitOfWork.CommitAsync();
        }

    }
}