using BlazorWasmTemplate.Domain.Users.Entities;
using BlazorWasmTemplate.Infrastructure.Persistence.Postgresql;
using BlazorWasmTemplate.Infrastructure.Persistence.Users.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BlazorWasmTemplate.Tests.Infrastructure.Persistence.Users.Repositories
{
    /// <summary>
    /// UserRepositoryのテストクラス
    /// </summary>
    public class UserRepositoryTests : IDisposable
    {
        private readonly AppDbContext _dbContext;
        private readonly UserRepository _userRepository;

        /// <summary>
        /// コンストラクタ - テスト用のInMemoryデータベースを設定
        /// </summary>
        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new AppDbContext(options);
            _userRepository = new UserRepository(_dbContext);
        }

        /// <summary>
        /// リソースの解放
        /// </summary>
        public void Dispose()
        {
            _dbContext.Dispose();
        }

        #region GetAllAsync Tests

        /// <summary>
        /// GetAllAsync - データが存在しない場合、空のリストを返すことを確認
        /// </summary>
        [Fact]
        public async Task GetAllAsync_WhenNoData_ReturnsEmptyList()
        {
            // Act
            var result = await _userRepository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        /// <summary>
        /// GetAllAsync - データが存在する場合、全てのユーザーを返すことを確認
        /// </summary>
        [Fact]
        public async Task GetAllAsync_WhenDataExists_ReturnsAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User("USER001", "テストユーザー1", true),
                new User("USER002", "テストユーザー2", false),
                new User("USER003", "テストユーザー3", true),
            };

            _dbContext.Users.AddRange(users);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(result, u => u.Code == "USER001");
            Assert.Contains(result, u => u.Code == "USER002");
            Assert.Contains(result, u => u.Code == "USER003");
        }

        #endregion

        #region GetByIdAsync Tests

        /// <summary>
        /// GetByIdAsync - 存在するIDの場合、対応するユーザーを返すことを確認
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_WhenUserExists_ReturnsUser()
        {
            // Arrange
            var user = new User("USER001", "テストユーザー", true);

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetByIdAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("USER001", result.Code);
            Assert.Equal("テストユーザー", result.Name);
            Assert.True(result.IsActive);
        }

        /// <summary>
        /// GetByIdAsync - 存在しないIDの場合、nullを返すことを確認
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_WhenUserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _userRepository.GetByIdAsync(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region AddAsync Tests

        /// <summary>
        /// AddAsync - 新しいユーザーが正常に追加されることを確認
        /// </summary>
        [Fact]
        public async Task AddAsync_WhenValidUser_AddsUserToDatabase()
        {
            // Arrange
            var user = new User("USER001", "新規ユーザー", true);

            // Act
            await _userRepository.AddAsync(user);

            // Assert
            var addedUser = await _dbContext.Users.FindAsync(user.Id);
            Assert.NotNull(addedUser);
            Assert.Equal(user.Id, addedUser.Id);
            Assert.Equal(user.Code, addedUser.Code);
            Assert.Equal(user.Name, addedUser.Name);
            Assert.Equal(user.IsActive, addedUser.IsActive);
        }

        #endregion

        #region UpdateAsync Tests

        /// <summary>
        /// UpdateAsync - 既存のユーザーが正常に更新されることを確認
        /// </summary>
        [Fact]
        public async Task UpdateAsync_WhenUserExists_UpdatesUserInDatabase()
        {
            // Arrange
            var user = new User("USER001", "元の名前", true);
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // 更新内容
            user.Name = "更新された名前";
            user.IsActive = false;

            // Act
            await _userRepository.UpdateAsync(user);

            // Assert
            var updatedUser = await _dbContext.Users.FindAsync(user.Id);
            Assert.NotNull(updatedUser);
            Assert.Equal("更新された名前", updatedUser.Name);
            Assert.False(updatedUser.IsActive);
        }

        #endregion

        #region DeleteAsync Tests

        /// <summary>
        /// DeleteAsync - 存在するユーザーが正常に削除されることを確認
        /// </summary>
        [Fact]
        public async Task DeleteAsync_WhenUserExists_DeletesUserFromDatabase()
        {
            // Arrange
            var user = new User("USER001", "削除対象ユーザー", true);
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act
            await _userRepository.DeleteAsync(user.Id);

            // Assert
            var deletedUser = await _dbContext.Users.FindAsync(user.Id);
            Assert.Null(deletedUser);
        }

        /// <summary>
        /// DeleteAsync - 存在しないIDの場合、例外が発生しないことを確認
        /// </summary>
        [Fact]
        public async Task DeleteAsync_WhenUserDoesNotExist_DoesNotThrowException()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act & Assert
            var exception = await Record.ExceptionAsync(() => _userRepository.DeleteAsync(nonExistentId));
            Assert.Null(exception);
        }

        /// <summary>
        /// DeleteAsync - 削除後に他のユーザーが影響を受けないことを確認
        /// </summary>
        [Fact]
        public async Task DeleteAsync_WhenDeletingOneUser_DoesNotAffectOtherUsers()
        {
            // Arrange
            var user1 = new User("USER001", "ユーザー1", true);
            var user2 = new User("USER002", "ユーザー2", true);

            _dbContext.Users.AddRange(user1, user2);
            await _dbContext.SaveChangesAsync();

            // Act
            await _userRepository.DeleteAsync(user1.Id);

            // Assert
            var remainingUser = await _dbContext.Users.FindAsync(user2.Id);
            Assert.NotNull(remainingUser);
            Assert.Equal("USER002", remainingUser.Code);

            var deletedUser = await _dbContext.Users.FindAsync(user1.Id);
            Assert.Null(deletedUser);
        }

        #endregion
    }
}