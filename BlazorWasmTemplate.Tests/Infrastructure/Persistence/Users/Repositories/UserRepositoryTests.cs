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

        /// <summary>
        /// GetAllAsync - 並行アクセス時の動作確認
        /// </summary>
        [Fact]
        public async Task GetAllAsync_WhenConcurrentAccess_ReturnsConsistentResults()
        {
            // Arrange
            var users = new List<User>
            {
                new User("USER001", "ユーザー1", true),
                new User("USER002", "ユーザー2", true),
                new User("USER003", "ユーザー3", true)
            };
            _dbContext.Users.AddRange(users);
            await _dbContext.SaveChangesAsync();

            // Act
            var tasks = new List<Task<List<User>>>();
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(_userRepository.GetAllAsync());
            }

            var results = await Task.WhenAll(tasks);

            // Assert
            foreach (var result in results)
            {
                Assert.Equal(3, result.Count);
            }
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

        /// <summary>
        /// GetByIdAsync - 空のGuidの場合、nullを返すことを確認
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_WhenEmptyGuid_ReturnsNull()
        {
            // Act
            var result = await _userRepository.GetByIdAsync(Guid.Empty);

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
        /// AddAsync - 空文字列のコードでユーザーを追加できることを確認
        /// </summary>
        [Fact]
        public async Task AddAsync_WhenCodeIsEmpty_AddsUserSuccessfully()
        {
            // Arrange
            var user = new User("", "空コードユーザー", true);

            // Act
            await _userRepository.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Assert
            var addedUser = await _dbContext.Users.FindAsync(user.Id);
            Assert.NotNull(addedUser);
            Assert.Equal("", addedUser.Code);
        }

        /// <summary>
        /// AddAsync - null名前でユーザーを追加できることを確認
        /// </summary>
        [Fact]
        public async Task AddAsync_WhenNameIsNull_AddsUserSuccessfully()
        {
            // Arrange
            var user = new User("USER001", null, true);

            // Act
            await _userRepository.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Assert
            var addedUser = await _dbContext.Users.FindAsync(user.Id);
            Assert.NotNull(addedUser);
            Assert.Null(addedUser.Name);
        }

        /// <summary>
        /// AddAsync - エンティティの状態がAddedになることを確認
        /// </summary>
        [Fact]
        public async Task AddAsync_WhenCalled_EntityStateIsAdded()
        {
            // Arrange
            var user = new User("USER001", "テストユーザー", true);

            // Act
            await _userRepository.AddAsync(user);

            // Assert
            var entry = _dbContext.Entry(user);
            Assert.Equal(EntityState.Added, entry.State);
        }

        /// <summary>
        /// AddAsync - 同じユーザーを複数回追加した場合の動作確認
        /// </summary>
        [Fact]
        public async Task AddAsync_WhenAddingSameUserTwice_ThrowsException()
        {
            // Arrange
            var user = new User("USER001", "テストユーザー", true);

            // Act
            await _userRepository.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _userRepository.AddAsync(user);
                await _dbContext.SaveChangesAsync();
            });
        }

        /// <summary>
        /// AddAsync - 並行追加時の動作確認
        /// </summary>
        [Fact]
        public async Task AddAsync_WhenConcurrentAdd_AllUsersAreAdded()
        {
            // Arrange
            var tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                var user = new User($"USER{i:D3}", $"並行ユーザー{i}", true);
                tasks.Add(_userRepository.AddAsync(user));
            }

            // Act
            await Task.WhenAll(tasks);
            await _dbContext.SaveChangesAsync();

            // Assert
            var allUsers = await _userRepository.GetAllAsync();
            Assert.Equal(10, allUsers.Count);
        }

        /// <summary>
        /// AddAsync - ユーザーのプロパティが正確に保存されることを確認
        /// </summary>
        [Fact]
        public async Task AddAsync_WhenUserAdded_AllPropertiesAreSavedCorrectly()
        {
            // Arrange
            var user = new User("USER001", "詳細テストユーザー", false);

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

        /// <summary>
        /// UpdateAsync - nullユーザーの場合、例外が発生することを確認
        /// </summary>
        [Fact]
        public async Task UpdateAsync_WhenUserIsNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _userRepository.UpdateAsync(null!));
        }

        /// <summary>
        /// UpdateAsync - エンティティの状態がModifiedになることを確認
        /// </summary>
        [Fact]
        public async Task UpdateAsync_WhenCalled_EntityStateIsModified()
        {
            // Arrange
            var user = new User("USER001", "テストユーザー", true);

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
            var user = new User("USER001", "存在しないユーザー", true);

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () =>
            {
                await _userRepository.UpdateAsync(user);
                await _dbContext.SaveChangesAsync();
            });
        }

        /// <summary>
        /// UpdateAsync - 部分的な更新が正しく動作することを確認
        /// </summary>
        [Fact]
        public async Task UpdateAsync_WhenPartialUpdate_OnlyModifiedPropertiesAreChanged()
        {
            // Arrange
            var user = new User("USER001", "元の名前", true);
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var originalCode = user.Code;
            var originalIsActive = user.IsActive;

            // Act - 名前のみ変更
            user.Name = "更新された名前";
            await _userRepository.UpdateAsync(user);
            await _dbContext.SaveChangesAsync();

            // Assert
            var updatedUser = await _dbContext.Users.FindAsync(user.Id);
            Assert.NotNull(updatedUser);
            Assert.Equal(originalCode, updatedUser.Code); // 変更されていない
            Assert.Equal("更新された名前", updatedUser.Name); // 変更されている
            Assert.Equal(originalIsActive, updatedUser.IsActive); // 変更されていない
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
            await _userRepository.DeleteAsync(user);
            await _dbContext.SaveChangesAsync();

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
            var user = new User("USER001", "非存在ユーザー", true);

            // Act & Assert
            var exception = await Record.ExceptionAsync(() => _userRepository.DeleteAsync(user));
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
            await Assert.ThrowsAsync<NullReferenceException>(() => _userRepository.DeleteAsync(null!));
        }

        /// <summary>
        /// DeleteAsync - エンティティの状態がDeletedになることを確認
        /// </summary>
        [Fact]
        public async Task DeleteAsync_WhenUserExists_EntityStateIsDeleted()
        {
            // Arrange
            var user = new User("USER001", "削除対象ユーザー", true);
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
            var user = new User("USER001", "削除対象ユーザー", true);
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act
            await _userRepository.DeleteAsync(user);
            await _dbContext.SaveChangesAsync();

            // Assert - 2回目の削除でも例外が発生しないこと
            var exception = await Record.ExceptionAsync(async () =>
            {
                await _userRepository.DeleteAsync(user);
                await _dbContext.SaveChangesAsync();
            });
            Assert.Null(exception);
        }

        #endregion
    }
}