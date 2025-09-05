using BlazorWasmTemplate.Application.Users.Cache;
using BlazorWasmTemplate.Domain.Users.Entities;
using BlazorWasmTemplate.Domain.Users.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BlazorWasmTemplate.Tests.Application.Users.Cache
{
    /// <summary>
    /// UserCacheのテストクラス
    /// </summary>
    public class UserCacheTests : IDisposable
    {
        private readonly IMemoryCache _memoryCache;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IServiceScopeFactory> _mockScopeFactory;
        private readonly Mock<IServiceScope> _mockScope;
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly UserCache _userCache;

        /// <summary>
        /// コンストラクタ - テスト用のモックオブジェクトを設定
        /// </summary>
        public UserCacheTests()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _mockUserRepository = new Mock<IUserRepository>();
            _mockScopeFactory = new Mock<IServiceScopeFactory>();
            _mockScope = new Mock<IServiceScope>();
            _mockServiceProvider = new Mock<IServiceProvider>();

            // モックの設定
            _mockScopeFactory.Setup(x => x.CreateScope()).Returns(_mockScope.Object);
            _mockScope.Setup(x => x.ServiceProvider).Returns(_mockServiceProvider.Object);
            _mockServiceProvider.Setup(x => x.GetService(typeof(IUserRepository))).Returns(_mockUserRepository.Object);

            _userCache = new UserCache(_memoryCache, _mockScopeFactory.Object);
        }

        /// <summary>
        /// リソースの解放
        /// </summary>
        public void Dispose()
        {
            _memoryCache.Dispose();
        }

        #region GetAllAsync Tests

        /// <summary>
        /// GetAllAsync - キャッシュが空の場合、リポジトリからデータを取得してキャッシュに保存することを確認
        /// </summary>
        [Fact]
        public async Task GetAllAsync_WhenCacheIsEmpty_FetchesFromRepositoryAndCachesData()
        {
            // Arrange
            var expectedUsers = new List<User>
            {
                new User(Guid.NewGuid(), "USER001", "テストユーザー1", true, DateTime.Now, DateTime.Now),
                new User(Guid.NewGuid(), "USER002", "テストユーザー2", false, DateTime.Now, DateTime.Now)
            };

            _mockUserRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(expectedUsers);

            // Act
            var result = await _userCache.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, u => u.Code == "USER001");
            Assert.Contains(result, u => u.Code == "USER002");

            // リポジトリが1回呼ばれることを確認
            _mockUserRepository.Verify(x => x.GetAllAsync(), Times.Once);

            // キャッシュにデータが保存されていることを確認
            var cachedData = _memoryCache.Get("UserCache") as IEnumerable<User>;
            Assert.NotNull(cachedData);
            Assert.Equal(2, cachedData.Count());
        }

        /// <summary>
        /// GetAllAsync - キャッシュにデータが存在する場合、リポジトリを呼ばずにキャッシュからデータを返すことを確認
        /// </summary>
        [Fact]
        public async Task GetAllAsync_WhenCacheHasData_ReturnsFromCacheWithoutCallingRepository()
        {
            // Arrange
            var cachedUsers = new List<User>
            {
                new User(Guid.NewGuid(), "CACHED001", "キャッシュユーザー1", true, DateTime.Now, DateTime.Now),
                new User(Guid.NewGuid(), "CACHED002", "キャッシュユーザー2", false, DateTime.Now, DateTime.Now)
            };

            _memoryCache.Set("UserCache", cachedUsers);

            // Act
            var result = await _userCache.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, u => u.Code == "CACHED001");
            Assert.Contains(result, u => u.Code == "CACHED002");

            // リポジトリが呼ばれないことを確認
            _mockUserRepository.Verify(x => x.GetAllAsync(), Times.Never);
        }

        /// <summary>
        /// GetAllAsync - リポジトリが空のリストを返す場合、空のリストがキャッシュされることを確認
        /// </summary>
        [Fact]
        public async Task GetAllAsync_WhenRepositoryReturnsEmptyList_CachesEmptyList()
        {
            // Arrange
            _mockUserRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<User>());

            // Act
            var result = await _userCache.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            // キャッシュに空のリストが保存されていることを確認
            var cachedData = _memoryCache.Get("UserCache") as IEnumerable<User>;
            Assert.NotNull(cachedData);
            Assert.Empty(cachedData);
        }

        /// <summary>
        /// GetAllAsync - 複数回呼び出した場合、最初の1回のみリポジトリが呼ばれることを確認
        /// </summary>
        [Fact]
        public async Task GetAllAsync_WhenCalledMultipleTimes_CallsRepositoryOnlyOnce()
        {
            // Arrange
            var expectedUsers = new List<User>
            {
                new User(Guid.NewGuid(), "USER001", "テストユーザー", true, DateTime.Now, DateTime.Now)
            };

            _mockUserRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(expectedUsers);

            // Act
            var result1 = await _userCache.GetAllAsync();
            var result2 = await _userCache.GetAllAsync();
            var result3 = await _userCache.GetAllAsync();

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotNull(result3);
            Assert.Equal(result1.Count(), result2.Count());
            Assert.Equal(result2.Count(), result3.Count());

            // リポジトリが1回のみ呼ばれることを確認
            _mockUserRepository.Verify(x => x.GetAllAsync(), Times.Once);
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
            var expectedUsers = new List<User>
            {
                new User(userId, "USER001", "テストユーザー1", true, DateTime.Now, DateTime.Now),
                new User(Guid.NewGuid(), "USER002", "テストユーザー2", false, DateTime.Now, DateTime.Now)
            };

            _mockUserRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(expectedUsers);

            // Act
            var result = await _userCache.GetByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("USER001", result.Code);
            Assert.Equal("テストユーザー1", result.Name);
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
            var expectedUsers = new List<User>
            {
                new User(Guid.NewGuid(), "USER001", "テストユーザー1", true, DateTime.Now, DateTime.Now),
                new User(Guid.NewGuid(), "USER002", "テストユーザー2", false, DateTime.Now, DateTime.Now)
            };

            _mockUserRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(expectedUsers);

            // Act
            var result = await _userCache.GetByIdAsync(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// GetByIdAsync - キャッシュが空の場合、リポジトリからデータを取得してから検索することを確認
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_WhenCacheIsEmpty_FetchesFromRepositoryThenSearches()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedUsers = new List<User>
            {
                new User(userId, "USER001", "テストユーザー", true, DateTime.Now, DateTime.Now)
            };

            _mockUserRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(expectedUsers);

            // Act
            var result = await _userCache.GetByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);

            // リポジトリが呼ばれることを確認
            _mockUserRepository.Verify(x => x.GetAllAsync(), Times.Once);
        }

        /// <summary>
        /// GetByIdAsync - キャッシュにデータが存在する場合、リポジトリを呼ばずに検索することを確認
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_WhenCacheHasData_SearchesInCacheWithoutCallingRepository()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cachedUsers = new List<User>
            {
                new User(userId, "CACHED001", "キャッシュユーザー", true, DateTime.Now, DateTime.Now)
            };

            _memoryCache.Set("UserCache", cachedUsers);

            // Act
            var result = await _userCache.GetByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("CACHED001", result.Code);

            // リポジトリが呼ばれないことを確認
            _mockUserRepository.Verify(x => x.GetAllAsync(), Times.Never);
        }

        #endregion

        #region RefreshAsync Tests

        /// <summary>
        /// RefreshAsync - リポジトリからデータを取得してキャッシュを更新することを確認
        /// </summary>
        [Fact]
        public async Task RefreshAsync_WhenCalled_FetchesFromRepositoryAndUpdatesCache()
        {
            // Arrange
            var newUsers = new List<User>
            {
                new User(Guid.NewGuid(), "REFRESH001", "リフレッシュユーザー1", true, DateTime.Now, DateTime.Now),
                new User(Guid.NewGuid(), "REFRESH002", "リフレッシュユーザー2", false, DateTime.Now, DateTime.Now)
            };

            _mockUserRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(newUsers);

            // Act
            await _userCache.RefreshAsync();

            // Assert
            // リポジトリが呼ばれることを確認
            _mockUserRepository.Verify(x => x.GetAllAsync(), Times.Once);

            // キャッシュが更新されていることを確認
            var cachedData = _memoryCache.Get("UserCache") as IEnumerable<User>;
            Assert.NotNull(cachedData);
            Assert.Equal(2, cachedData.Count());
            Assert.Contains(cachedData, u => u.Code == "REFRESH001");
            Assert.Contains(cachedData, u => u.Code == "REFRESH002");
        }

        /// <summary>
        /// RefreshAsync - 既存のキャッシュデータを上書きすることを確認
        /// </summary>
        [Fact]
        public async Task RefreshAsync_WhenCacheHasOldData_OverwritesWithNewData()
        {
            // Arrange
            var oldUsers = new List<User>
            {
                new User(Guid.NewGuid(), "OLD001", "古いユーザー", true, DateTime.Now, DateTime.Now)
            };

            var newUsers = new List<User>
            {
                new User(Guid.NewGuid(), "NEW001", "新しいユーザー1", true, DateTime.Now, DateTime.Now),
                new User(Guid.NewGuid(), "NEW002", "新しいユーザー2", false, DateTime.Now, DateTime.Now)
            };

            // 古いデータをキャッシュに設定
            _memoryCache.Set("UserCache", oldUsers);

            _mockUserRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(newUsers);

            // Act
            await _userCache.RefreshAsync();

            // Assert
            var cachedData = _memoryCache.Get("UserCache") as IEnumerable<User>;
            Assert.NotNull(cachedData);
            Assert.Equal(2, cachedData.Count());
            Assert.Contains(cachedData, u => u.Code == "NEW001");
            Assert.Contains(cachedData, u => u.Code == "NEW002");
            Assert.DoesNotContain(cachedData, u => u.Code == "OLD001");
        }

        /// <summary>
        /// RefreshAsync - スコープが正しく作成され、破棄されることを確認
        /// </summary>
        [Fact]
        public async Task RefreshAsync_WhenCalled_CreatesAndDisposesScope()
        {
            // Arrange
            var users = new List<User>
            {
                new User(Guid.NewGuid(), "USER001", "テストユーザー", true, DateTime.Now, DateTime.Now)
            };

            _mockUserRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(users);

            // Act
            await _userCache.RefreshAsync();

            // Assert
            _mockScopeFactory.Verify(x => x.CreateScope(), Times.Once);
            _mockScope.Verify(x => x.Dispose(), Times.Once);
            _mockServiceProvider.Verify(x => x.GetService(typeof(IUserRepository)), Times.Once);
        }

        /// <summary>
        /// RefreshAsync - 複数回呼び出した場合、毎回リポジトリが呼ばれることを確認
        /// </summary>
        [Fact]
        public async Task RefreshAsync_WhenCalledMultipleTimes_CallsRepositoryEachTime()
        {
            // Arrange
            var users = new List<User>
            {
                new User(Guid.NewGuid(), "USER001", "テストユーザー", true, DateTime.Now, DateTime.Now)
            };

            _mockUserRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(users);

            // Act
            await _userCache.RefreshAsync();
            await _userCache.RefreshAsync();
            await _userCache.RefreshAsync();

            // Assert
            _mockUserRepository.Verify(x => x.GetAllAsync(), Times.Exactly(3));
            _mockScopeFactory.Verify(x => x.CreateScope(), Times.Exactly(3));
        }

        #endregion

        #region Error Handling Tests

        /// <summary>
        /// エラーハンドリング - リポジトリが例外を投げた場合、例外が伝播することを確認
        /// </summary>
        [Fact]
        public async Task ErrorHandling_WhenRepositoryThrowsException_PropagatesException()
        {
            // Arrange
            _mockUserRepository.Setup(x => x.GetAllAsync()).ThrowsAsync(new InvalidOperationException("Repository error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _userCache.GetAllAsync());
            await Assert.ThrowsAsync<InvalidOperationException>(() => _userCache.RefreshAsync());
        }

        /// <summary>
        /// エラーハンドリング - スコープファクトリが例外を投げた場合、例外が伝播することを確認
        /// </summary>
        [Fact]
        public async Task ErrorHandling_WhenScopeFactoryThrowsException_PropagatesException()
        {
            // Arrange
            _mockScopeFactory.Setup(x => x.CreateScope()).Throws(new InvalidOperationException("Scope creation error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _userCache.RefreshAsync());
        }

        #endregion
    }
}