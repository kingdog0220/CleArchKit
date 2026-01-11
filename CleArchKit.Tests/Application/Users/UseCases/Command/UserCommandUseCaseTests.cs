using CleArchKit.Application.Persistence;
using CleArchKit.Application.Users.Dtos;
using CleArchKit.Application.Users.Services;
using CleArchKit.Application.Users.UseCases.Command;
using CleArchKit.Domain.Users.Entities;
using CleArchKit.Domain.Users.Repositories;
using Moq;

namespace CleArchKit.Tests.Application.Users.UseCases.Command
{
    /// <summary>
    /// UserCommandUseCaseのテストクラス
    /// </summary>
    public class UserCommandUseCaseTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IUserService> _mockUserService;
        private readonly UserCommandUseCase _userCommandUseCase;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;

        /// <summary>
        /// コンストラクタ - テスト用のモックオブジェクトを設定
        /// </summary>
        public UserCommandUseCaseTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockUserService = new Mock<IUserService>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _userCommandUseCase = new UserCommandUseCase(
                _mockUserRepository.Object,
                _mockUserService.Object,
                _mockUnitOfWork.Object);
        }

        #region CreateAsync Tests

        /// <summary>
        /// CreateAsync - 正常なユーザー作成の場合、ユーザーが正常に作成されることを確認
        /// </summary>
        [Fact]
        public async Task CreateAsync_WhenValidUser_CreatesUserSuccessfully()
        {
            // Arrange
            var createUserDto = new CreateUserDto
            {
                Code = "USER001",
                Name = "テストユーザー",
            };

            _mockUserRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);
            _mockUserService.Setup(x => x.ExistsByCodeAsync(createUserDto.Code, null)).ReturnsAsync(false);

            // Act
            await _userCommandUseCase.CreateAsync(createUserDto);

            // Assert
            _mockUserRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
            _mockUserService.Verify(x => x.ExistsByCodeAsync(createUserDto.Code, null), Times.Once);
            _mockUserRepository.Verify(x => x.AddAsync(It.Is<User>(u =>
                u.Id != Guid.Empty &&
                u.Code == createUserDto.Code &&
                u.Name == createUserDto.Name)), Times.Once);
        }

        /// <summary>
        /// CreateAsync - 主キーが重複している場合、例外が発生することを確認
        /// </summary>
        [Fact]
        public async Task CreateAsync_WhenIdAlreadyExists_ThrowsException()
        {
            // Arrange
            var createUserDto = new CreateUserDto
            {
                Code = "USER001",
                Name = "テストユーザー",
            };

            var existingUser = new User(Guid.NewGuid(), "EXISTING001", "既存ユーザー", DateTime.Now, DateTime.Now);
            _mockUserRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(existingUser);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _userCommandUseCase.CreateAsync(createUserDto));
            Assert.StartsWith($"主キーが重複しています:", exception.Message);

            // 主キーチェック後は処理が停止することを確認
            _mockUserRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
            _mockUserService.Verify(x => x.ExistsByCodeAsync(It.IsAny<string>(), It.IsAny<Guid?>()), Times.Never);
            _mockUserRepository.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
        }

        /// <summary>
        /// CreateAsync - コードが重複している場合、例外が発生することを確認
        /// </summary>
        [Fact]
        public async Task CreateAsync_WhenCodeAlreadyExists_ThrowsException()
        {
            // Arrange
            var createUserDto = new CreateUserDto
            {
                Code = "USER001",
                Name = "テストユーザー",
            };

            _mockUserRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);
            _mockUserService.Setup(x => x.ExistsByCodeAsync(createUserDto.Code, null)).ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _userCommandUseCase.CreateAsync(createUserDto));
            Assert.Equal($"CODEが重複しています:{createUserDto.Code}", exception.Message);

            // コードチェック後は処理が停止することを確認
            _mockUserRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
            _mockUserService.Verify(x => x.ExistsByCodeAsync(createUserDto.Code, null), Times.Once);
            _mockUserRepository.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
        }

        #endregion

        #region UpdateAsync Tests

        /// <summary>
        /// UpdateAsync - 正常なユーザー更新の場合、ユーザーが正常に更新されることを確認
        /// </summary>
        [Fact]
        public async Task UpdateAsync_WhenValidUser_UpdatesUserSuccessfully()
        {
            // Arrange
            var updateUserDto = new UpdateUserDto
            {
                Id = Guid.NewGuid(),
                Code = "USER001",
                Name = "更新後ユーザー",
                CreatedAt = DateTime.Now.AddDays(-1),
            };

            var existingUser = new User(updateUserDto.Id, "OLD_CODE", "更新前ユーザー", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-1));
            _mockUserRepository.Setup(x => x.GetByIdAsync(updateUserDto.Id)).ReturnsAsync(existingUser);
            _mockUserService.Setup(x => x.ExistsByCodeAsync(updateUserDto.Code, updateUserDto.Id)).ReturnsAsync(false);

            // Act
            await _userCommandUseCase.UpdateAsync(updateUserDto);

            // Assert
            _mockUserRepository.Verify(x => x.GetByIdAsync(updateUserDto.Id), Times.Once);
            _mockUserService.Verify(x => x.ExistsByCodeAsync(updateUserDto.Code, updateUserDto.Id), Times.Once);
            _mockUserRepository.Verify(x => x.UpdateAsync(It.Is<User>(u =>
                u.Id == updateUserDto.Id &&
                u.Code == updateUserDto.Code &&
                u.Name == updateUserDto.Name)), Times.Once);
        }

        /// <summary>
        /// UpdateAsync - 存在しないユーザーを更新しようとした場合、例外が発生することを確認
        /// </summary>
        [Fact]
        public async Task UpdateAsync_WhenUserDoesNotExist_ThrowsException()
        {
            // Arrange
            var updateUserDto = new UpdateUserDto
            {
                Id = Guid.NewGuid(),
                Code = "USER001",
                Name = "存在しないユーザー",
                CreatedAt = DateTime.Now,
            };

            _mockUserRepository.Setup(x => x.GetByIdAsync(updateUserDto.Id)).ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _userCommandUseCase.UpdateAsync(updateUserDto));
            Assert.Equal($"ユーザーはいません:{updateUserDto.Id}", exception.Message);

            // ユーザー存在チェック後は処理が停止することを確認
            _mockUserRepository.Verify(x => x.GetByIdAsync(updateUserDto.Id), Times.Once);
            _mockUserService.Verify(x => x.ExistsByCodeAsync(It.IsAny<string>(), It.IsAny<Guid?>()), Times.Never);
            _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        /// <summary>
        /// UpdateAsync - コードが他のユーザーと重複している場合、例外が発生することを確認
        /// </summary>
        [Fact]
        public async Task UpdateAsync_WhenCodeAlreadyExistsForOtherUser_ThrowsException()
        {
            // Arrange
            var updateUserDto = new UpdateUserDto
            {
                Id = Guid.NewGuid(),
                Code = "USER001",
                Name = "更新ユーザー",
                CreatedAt = DateTime.Now,
            };

            var existingUser = new User(updateUserDto.Id, "OLD_CODE", "既存ユーザー", DateTime.Now, DateTime.Now);
            _mockUserRepository.Setup(x => x.GetByIdAsync(updateUserDto.Id)).ReturnsAsync(existingUser);
            _mockUserService.Setup(x => x.ExistsByCodeAsync(updateUserDto.Code, updateUserDto.Id)).ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _userCommandUseCase.UpdateAsync(updateUserDto));
            Assert.Equal($"CODEが重複しています:{updateUserDto.Code}", exception.Message);

            // コードチェック後は処理が停止することを確認
            _mockUserRepository.Verify(x => x.GetByIdAsync(updateUserDto.Id), Times.Once);
            _mockUserService.Verify(x => x.ExistsByCodeAsync(updateUserDto.Code, updateUserDto.Id), Times.Once);
            _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        /// <summary>
        /// UpdateAsync - 同じコードで自分自身を更新する場合、正常に処理されることを確認
        /// </summary>
        [Fact]
        public async Task UpdateAsync_WhenUpdatingSameCodeForSameUser_ProcessesNormally()
        {
            // Arrange
            var updateUserDto = new UpdateUserDto
            {
                Id = Guid.NewGuid(),
                Code = "USER001",
                Name = "更新後ユーザー",
                CreatedAt = DateTime.Now,
            };

            var existingUser = new User(updateUserDto.Id, "USER001", "更新前ユーザー", DateTime.Now, DateTime.Now);
            _mockUserRepository.Setup(x => x.GetByIdAsync(updateUserDto.Id)).ReturnsAsync(existingUser);
            _mockUserService.Setup(x => x.ExistsByCodeAsync(updateUserDto.Code, updateUserDto.Id)).ReturnsAsync(false);

            // Act
            await _userCommandUseCase.UpdateAsync(updateUserDto);

            // Assert
            _mockUserRepository.Verify(x => x.GetByIdAsync(updateUserDto.Id), Times.Once);
            _mockUserService.Verify(x => x.ExistsByCodeAsync(updateUserDto.Code, updateUserDto.Id), Times.Once);
            _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
        }

        #endregion

        #region DeleteAsync Tests

        /// <summary>
        /// DeleteAsync - 正常なユーザー削除の場合、ユーザーが正常に削除されることを確認
        /// </summary>
        [Fact]
        public async Task DeleteAsync_WhenValidUser_DeletesUserSuccessfully()
        {
            // Arrange
            var existingUser = new User(Guid.NewGuid(), "USER001", "削除対象ユーザー", DateTime.Now.AddDays(-1), DateTime.Now);
            _mockUserRepository.Setup(x => x.GetByIdAsync(existingUser.Id)).ReturnsAsync(existingUser);

            // Act
            await _userCommandUseCase.DeleteAsync(existingUser.Id);

            // Assert
            _mockUserRepository.Verify(x => x.GetByIdAsync(existingUser.Id), Times.Once);
            _mockUserRepository.Verify(x => x.DeleteAsync(It.Is<User>(u =>
                u.Id == existingUser.Id &&
                u.Code == existingUser.Code &&
                u.Name == existingUser.Name)), Times.Once);
        }

        /// <summary>
        /// DeleteAsync - 存在しないユーザーを削除しようとした場合、例外が発生することを確認
        /// </summary>
        [Fact]
        public async Task DeleteAsync_WhenUserDoesNotExist_ThrowsException()
        {
            // Arrange
            var notExistingUser = new User(Guid.NewGuid(), "USER001", "存在しないユーザー", DateTime.Now.AddDays(-1), DateTime.Now);
            _mockUserRepository.Setup(x => x.GetByIdAsync(notExistingUser.Id)).ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _userCommandUseCase.DeleteAsync(notExistingUser.Id));
            Assert.Equal($"ユーザーはいません:{notExistingUser.Id}", exception.Message);

            // ユーザー存在チェック後は処理が停止することを確認
            _mockUserRepository.Verify(x => x.GetByIdAsync(notExistingUser.Id), Times.Once);
            _mockUserRepository.Verify(x => x.DeleteAsync(It.IsAny<User>()), Times.Never);
        }

        #endregion
    }
}