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
                new User { Id = Guid.NewGuid(), Code = "USER001", Name = "テストユーザー1", IsActive = true },
                new User { Id = Guid.NewGuid(), Code = "USER002", Name = "テストユーザー2", IsActive = false },
                new User { Id = Guid.NewGuid(), Code = "USER003", Name = "テストユーザー3", IsActive = true }
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
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Code = "USER001", Name = "テストユーザー", IsActive = true };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
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
            var user = new User
            {
                Id = Guid.NewGuid(),
                Code = "USER001",
                Name = "新規ユーザー",
                IsActive = true
            };

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

        /// <summary>
        /// AddAsync - 複数のユーザーを追加できることを確認
        /// </summary>
        [Fact]
        public async Task AddAsync_WhenMultipleUsers_AddsAllUsersToDatabase()
        {
            // Arrange
            var user1 = new User { Id = Guid.NewGuid(), Code = "USER001", Name = "ユーザー1", IsActive = true };
            var user2 = new User { Id = Guid.NewGuid(), Code = "USER002", Name = "ユーザー2", IsActive = false };

            // Act
            await _userRepository.AddAsync(user1);
            await _userRepository.AddAsync(user2);

            // Assert
            var users = await _dbContext.Users.ToListAsync();
            Assert.Equal(2, users.Count);
            Assert.Contains(users, u => u.Code == "USER001");
            Assert.Contains(users, u => u.Code == "USER002");
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
            var user = new User { Id = Guid.NewGuid(), Code = "USER001", Name = "元の名前", IsActive = true };
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

        /// <summary>
        /// UpdateAsync - ユーザーのコードも更新できることを確認
        /// </summary>
        [Fact]
        public async Task UpdateAsync_WhenUpdatingCode_UpdatesCodeInDatabase()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Code = "USER001", Name = "テストユーザー", IsActive = true };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // 更新内容
            user.Code = "UPDATED001";

            // Act
            await _userRepository.UpdateAsync(user);

            // Assert
            var updatedUser = await _dbContext.Users.FindAsync(user.Id);
            Assert.NotNull(updatedUser);
            Assert.Equal("UPDATED001", updatedUser.Code);
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
            var user = new User { Id = Guid.NewGuid(), Code = "USER001", Name = "削除対象ユーザー", IsActive = true };
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
            var user1 = new User { Id = Guid.NewGuid(), Code = "USER001", Name = "ユーザー1", IsActive = true };
            var user2 = new User { Id = Guid.NewGuid(), Code = "USER002", Name = "ユーザー2", IsActive = true };

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

        #region Integration Tests

        /// <summary>
        /// 統合テスト - CRUD操作の一連の流れを確認
        /// </summary>
        [Fact]
        public async Task IntegrationTest_CrudOperations_WorksCorrectly()
        {
            // Create
            var user = new User { Id = Guid.NewGuid(), Code = "USER001", Name = "統合テストユーザー", IsActive = true };
            await _userRepository.AddAsync(user);

            // Read
            var retrievedUser = await _userRepository.GetByIdAsync(user.Id);
            Assert.NotNull(retrievedUser);
            Assert.Equal("USER001", retrievedUser.Code);

            // Update
            retrievedUser.Name = "更新された統合テストユーザー";
            retrievedUser.IsActive = false;
            await _userRepository.UpdateAsync(retrievedUser);

            var updatedUser = await _userRepository.GetByIdAsync(user.Id);
            Assert.NotNull(updatedUser);
            Assert.Equal("更新された統合テストユーザー", updatedUser.Name);
            Assert.False(updatedUser.IsActive);

            // Delete
            await _userRepository.DeleteAsync(user.Id);
            var deletedUser = await _userRepository.GetByIdAsync(user.Id);
            Assert.Null(deletedUser);
        }

        /// <summary>
        /// 統合テスト - 複数ユーザーでの操作を確認
        /// </summary>
        [Fact]
        public async Task IntegrationTest_MultipleUsers_WorksCorrectly()
        {
            // 複数ユーザーを追加
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Code = "USER001", Name = "ユーザー1", IsActive = true },
                new User { Id = Guid.NewGuid(), Code = "USER002", Name = "ユーザー2", IsActive = false },
                new User { Id = Guid.NewGuid(), Code = "USER003", Name = "ユーザー3", IsActive = true }
            };

            foreach (var user in users)
            {
                await _userRepository.AddAsync(user);
            }

            // 全件取得で確認
            var allUsers = await _userRepository.GetAllAsync();
            Assert.Equal(3, allUsers.Count);

            // 1つのユーザーを更新
            var userToUpdate = users[1];
            userToUpdate.IsActive = true;
            await _userRepository.UpdateAsync(userToUpdate);

            // 1つのユーザーを削除
            await _userRepository.DeleteAsync(users[0].Id);

            // 最終状態を確認
            var finalUsers = await _userRepository.GetAllAsync();
            Assert.Equal(2, finalUsers.Count);
            Assert.DoesNotContain(finalUsers, u => u.Id == users[0].Id);
            Assert.Contains(finalUsers, u => u.Id == users[1].Id && u.IsActive == true);
            Assert.Contains(finalUsers, u => u.Id == users[2].Id);
        }

        #endregion
    }
}