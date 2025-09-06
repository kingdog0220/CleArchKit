using BlazorWasmTemplate.Application.Users.Services;
using BlazorWasmTemplate.Application.Users.UseCases.Query;
using BlazorWasmTemplate.Domain.Users.Entities;
using Moq;

namespace BlazorWasmTemplate.Tests.Application.Users.UseCases.Query
{
    /// <summary>
    /// UserQueryUseCaseのテストクラス
    /// </summary>
    public class UserQueryUseCaseTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UserQueryUseCase _userQueryUseCase;

        /// <summary>
        /// コンストラクタ - テスト用のモックオブジェクトを設定
        /// </summary>
        public UserQueryUseCaseTests()
        {
            _mockUserService = new Mock<IUserService>();
            _userQueryUseCase = new UserQueryUseCase(_mockUserService.Object);
        }

        #region GetAllAsync Tests

        /// <summary>
        /// GetAllAsync - ユーザーが存在する場合、全ユーザーが正常に取得されることを確認
        /// </summary>
        [Fact]
        public async Task GetAllAsync_WhenUsersExist_ReturnsAllUsers()
        {
            // Arrange
            var expectedUsers = new List<User>
            {
                new User(Guid.NewGuid(), "USER001", "テストユーザー1", true, DateTime.Now.AddDays(-2), DateTime.Now.AddDays(-1)),
                new User(Guid.NewGuid(), "USER002", "テストユーザー2", false, DateTime.Now.AddDays(-3), DateTime.Now.AddDays(-2)),
                new User(Guid.NewGuid(), "USER003", "テストユーザー3", true, DateTime.Now.AddDays(-1), DateTime.Now)
            };

            _mockUserService.Setup(x => x.GetAllAsync()).ReturnsAsync(expectedUsers);

            // Act
            var result = await _userQueryUseCase.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUsers.Count, result.Count());
            Assert.Equal(expectedUsers, result);
            _mockUserService.Verify(x => x.GetAllAsync(), Times.Once);
        }

        /// <summary>
        /// GetAllAsync - ユーザーが存在しない場合、空のコレクションが返されることを確認
        /// </summary>
        [Fact]
        public async Task GetAllAsync_WhenNoUsersExist_ReturnsEmptyCollection()
        {
            // Arrange
            var expectedUsers = new List<User>();
            _mockUserService.Setup(x => x.GetAllAsync()).ReturnsAsync(expectedUsers);

            // Act
            var result = await _userQueryUseCase.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockUserService.Verify(x => x.GetAllAsync(), Times.Once);
        }

        #endregion

        #region GetByIdAsync Tests

        /// <summary>
        /// GetByIdAsync - 指定されたIDのユーザーが存在する場合、そのユーザーが正常に取得されることを確認
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_WhenUserExists_ReturnsUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedUser = new User(userId, "USER001", "テストユーザー", true, DateTime.Now.AddDays(-1), DateTime.Now);
            _mockUserService.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(expectedUser);

            // Act
            var result = await _userQueryUseCase.GetByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUser.Id, result.Id);
            Assert.Equal(expectedUser.Code, result.Code);
            Assert.Equal(expectedUser.Name, result.Name);
            Assert.Equal(expectedUser.IsActive, result.IsActive);
            Assert.Equal(expectedUser.CreatedAt, result.CreatedAt);
            Assert.Equal(expectedUser.UpdatedAt, result.UpdatedAt);
            _mockUserService.Verify(x => x.GetByIdAsync(userId), Times.Once);
        }

        /// <summary>
        /// GetByIdAsync - 指定されたIDのユーザーが存在しない場合、nullが返されることを確認
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_WhenUserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockUserService.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync((User?)null);

            // Act
            var result = await _userQueryUseCase.GetByIdAsync(userId);

            // Assert
            Assert.Null(result);
            _mockUserService.Verify(x => x.GetByIdAsync(userId), Times.Once);
        }

        /// <summary>
        /// GetByIdAsync - 空のGuidが指定された場合、UserServiceに正しく渡されることを確認
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_WhenEmptyGuidProvided_PassesToUserService()
        {
            // Arrange
            var emptyGuid = Guid.Empty;
            _mockUserService.Setup(x => x.GetByIdAsync(emptyGuid)).ReturnsAsync((User?)null);

            // Act
            var result = await _userQueryUseCase.GetByIdAsync(emptyGuid);

            // Assert
            Assert.Null(result);
            _mockUserService.Verify(x => x.GetByIdAsync(emptyGuid), Times.Once);
        }

        #endregion

    }
}