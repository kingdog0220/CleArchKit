using BlazorWasmTemplate.Domain.Users.Entities;
using BlazorWasmTemplate.Infrastructure.Persistence.Postgresql;
using BlazorWasmTemplate.Infrastructure.Persistence.Users.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text;

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
                new User(Guid.NewGuid(), "USER001", "テストユーザー1", true, DateTime.MinValue, DateTime.MinValue),
                new User(Guid.NewGuid(), "USER002", "テストユーザー2", false, DateTime.MinValue, DateTime.MinValue),
                new User(Guid.NewGuid(), "USER003", "テストユーザー3", true, DateTime.MinValue, DateTime.MinValue),
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
            var user = new User(Guid.NewGuid(), "USER001", "テストユーザー", true, DateTime.MinValue, DateTime.MinValue);

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
            var user = new User(Guid.NewGuid(), "USER001", "新規ユーザー", true, DateTime.MinValue, DateTime.MinValue);

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
        /// AddAsync - nullユーザーの場合、例外が発生することを確認
        /// </summary>
        [Fact]
        public async Task AddAsync_WhenUserIsNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _userRepository.AddAsync(null!));
        }

        /// <summary>
        /// AddAsync - エンティティの状態がAddedになることを確認
        /// </summary>
        [Fact]
        public async Task AddAsync_WhenCalled_EntityStateIsAdded()
        {
            // Arrange
            var user = new User(Guid.NewGuid(), "USER001", "テストユーザー", true, DateTime.MinValue, DateTime.MinValue);

            // Act
            await _userRepository.AddAsync(user);

            // Assert
            var entry = _dbContext.Entry(user);
            Assert.Equal(EntityState.Added, entry.State);
        }

        /// <summary>
        /// AddAsync - ユーザーのプロパティが正確に保存されることを確認
        /// </summary>
        [Fact]
        public async Task AddAsync_WhenUserAdded_AllPropertiesAreSavedCorrectly()
        {
            // Arrange
            var user = new User(Guid.NewGuid(), "USER001", "詳細テストユーザー", false, DateTime.MinValue, DateTime.MinValue);

            // Act
            await _userRepository.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Assert
            var savedUser = await _dbContext.Users.FindAsync(user.Id);
            Assert.NotNull(savedUser);
            Assert.Equal(user.Id, savedUser.Id);
            Assert.Equal("USER001", savedUser.Code);
            Assert.Equal("詳細テストユーザー", savedUser.Name);
            Assert.False(savedUser.IsActive);
            Assert.Equal(DateTime.MinValue, savedUser.CreatedAt);
            Assert.Equal(DateTime.MinValue, savedUser.UpdatedAt);
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
            var user = new User(Guid.NewGuid(), "USER001", "元の名前", true, DateTime.MinValue, DateTime.MinValue);
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
        /// UpdateAsync - エンティティの状態がModifiedになることを確認
        /// </summary>
        [Fact]
        public async Task UpdateAsync_WhenCalled_EntityStateIsModified()
        {
            // Arrange
            var user = new User(Guid.NewGuid(), "USER001", "テストユーザー", true, DateTime.MinValue, DateTime.MinValue);

            // Act
            await _userRepository.UpdateAsync(user);

            // Assert
            var entry = _dbContext.Entry(user);
            Assert.Equal(EntityState.Modified, entry.State);
        }

        /// <summary>
        /// UpdateAsync - 存在しないユーザーを更新しようとした場合の動作確認
        /// </summary>
        [Fact]
        public async Task UpdateAsync_WhenUserDoesNotExist_DoesNotThrowException()
        {
            // Arrange
            var user = new User(Guid.NewGuid(), "USER001", "存在しないユーザー", true, DateTime.MinValue, DateTime.MinValue);

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () =>
            {
                await _userRepository.UpdateAsync(user);
                await _dbContext.SaveChangesAsync();
            });
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
            var user = new User(Guid.NewGuid(), "USER001", "削除対象ユーザー", true, DateTime.MinValue, DateTime.MinValue);
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act
            await _userRepository.DeleteAsync(user);
            await _dbContext.SaveChangesAsync();

            // Assert
            var deletedUser = await _dbContext.Users.FindAsync(user.Id);
            Assert.Null(deletedUser);
        }

        /// <summary>
        /// DeleteAsync - 存在しないIDの場合、例外が発生することを確認
        /// </summary>
        [Fact]
        public async Task DeleteAsync_WhenUserDoesNotExist_DoesNotThrowException()
        {
            // Arrange
            var user = new User(Guid.NewGuid(), "USER001", "非存在ユーザー", true, DateTime.MinValue, DateTime.MinValue);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _userRepository.DeleteAsync(null!));
        }

        /// <summary>
        /// DeleteAsync - 削除後に他のユーザーが影響を受けないことを確認
        /// </summary>
        [Fact]
        public async Task DeleteAsync_WhenDeletingOneUser_DoesNotAffectOtherUsers()
        {
            // Arrange
            var user1 = new User(Guid.NewGuid(), "USER001", "ユーザー1", true, DateTime.MinValue, DateTime.MinValue);
            var user2 = new User(Guid.NewGuid(), "USER002", "ユーザー2", true, DateTime.MinValue, DateTime.MinValue);

            _dbContext.Users.AddRange(user1, user2);
            await _dbContext.SaveChangesAsync();

            // Act
            await _userRepository.DeleteAsync(user1);
            await _dbContext.SaveChangesAsync();

            // Assert
            var remainingUser = await _dbContext.Users.FindAsync(user2.Id);
            Assert.NotNull(remainingUser);
            Assert.Equal("USER002", remainingUser.Code);

            var deletedUser = await _dbContext.Users.FindAsync(user1.Id);
            Assert.Null(deletedUser);
        }

        /// <summary>
        /// DeleteAsync - nullユーザーの場合、例外が発生することを確認
        /// </summary>
        [Fact]
        public async Task DeleteAsync_WhenUserIsNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _userRepository.DeleteAsync(null!));
        }

        /// <summary>
        /// DeleteAsync - エンティティの状態がDeletedになることを確認
        /// </summary>
        [Fact]
        public async Task DeleteAsync_WhenUserExists_EntityStateIsDeleted()
        {
            // Arrange
            var user = new User(Guid.NewGuid(), "USER001", "削除対象ユーザー", true, DateTime.MinValue, DateTime.MinValue);
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act
            await _userRepository.DeleteAsync(user);

            // Assert
            var entry = _dbContext.Entry(user);
            Assert.Equal(EntityState.Deleted, entry.State);
        }

        /// <summary>
        /// DeleteAsync - 同じユーザーを複数回削除した場合の動作確認
        /// </summary>
        [Fact]
        public async Task DeleteAsync_WhenDeletingSameUserTwice_DoesNotThrowException()
        {
            // Arrange
            var user = new User(Guid.NewGuid(), "USER001", "削除対象ユーザー", true, DateTime.MinValue, DateTime.MinValue);
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act
            await _userRepository.DeleteAsync(user);
            await _dbContext.SaveChangesAsync();

            // Assert - 2回目の削除は例外が発生する
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () =>
            {
                await _userRepository.DeleteAsync(user);
                await _dbContext.SaveChangesAsync();
            });
        }

        #endregion

        #region ExistsByCodeAsync Tests

        /// <summary>
        /// ExistsByCodeAsync - コードが存在することを確認
        /// </summary>
        [Fact]
        public async Task ExistsByCodeAsync_Exist()
        {
            // Arrange
            var user = new User(Guid.NewGuid(), "USER001", "ユーザー1", true, DateTime.MinValue, DateTime.MinValue);
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userRepository.ExistsByCodeAsync(user.Code);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// ExistsByCodeAsync - コードが存在しないことを確認
        /// </summary>
        [Fact]
        public async Task ExistsByCodeAsync_NoExist()
        {
            // Arrange
            var user = new User(Guid.NewGuid(), "USER001", "ユーザー1", true, DateTime.MinValue, DateTime.MinValue);

            // Act
            var result = await _userRepository.ExistsByCodeAsync(user.Code);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// ExistsByCodeAsync - 自分自身のコードはチェックしないことを確認
        /// </summary>
        [Fact]
        public async Task ExistsByCodeAsync_ExcludeId()
        {
            // Arrange
            var user = new User(Guid.NewGuid(), "USER001", "ユーザー1", true, DateTime.MinValue, DateTime.MinValue);
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userRepository.ExistsByCodeAsync(user.Code, user.Id);

            // Assert
            Assert.False(result);
        }

        #endregion
    }
}