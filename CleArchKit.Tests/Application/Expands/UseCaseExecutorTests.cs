using CleArchKit.Application.Events;
using CleArchKit.Application.Expands;
using CleArchKit.Application.Persistence;
using Moq;

namespace CleArchKit.Tests.Application.Expands
{
    /// <summary>
    /// UseCaseExecutorのテストクラス
    /// </summary>
    public class UseCaseExecutorTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IDomainEventBuffer> _mockEventBuffer;
        private readonly UseCaseExecutor _useCaseExecutor;

        /// <summary>
        /// コンストラクタ - テスト用のモックオブジェクトを設定
        /// </summary>
        public UseCaseExecutorTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockEventBuffer = new Mock<IDomainEventBuffer>();
            _useCaseExecutor = new UseCaseExecutor(_mockUnitOfWork.Object, _mockEventBuffer.Object);
        }

        #region Constructor Tests

        /// <summary>
        /// コンストラクタ - 正常なパラメータで初期化できることを確認
        /// </summary>
        [Fact]
        public void Constructor_WithValidParameters_CreatesInstance()
        {
            // Arrange & Act
            var executor = new UseCaseExecutor(_mockUnitOfWork.Object, _mockEventBuffer.Object);

            // Assert
            Assert.NotNull(executor);
        }

        #endregion

        #region ExecuteAsync Success Tests

        /// <summary>
        /// ExecuteAsync - 正常なユースケースが正しい順序で実行されることを確認
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_WithSuccessfulUseCase_ExecutesInCorrectOrder()
        {
            // Arrange
            var useCaseExecuted = false;
            var useCase = new Func<Task>(() =>
            {
                useCaseExecuted = true;
                return Task.CompletedTask;
            });

            _mockUnitOfWork.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _mockEventBuffer.Setup(x => x.FlushAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            await _useCaseExecutor.ExecuteAsync(useCase);

            // Assert
            Assert.True(useCaseExecuted);
            _mockUnitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockEventBuffer.Verify(x => x.FlushAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockEventBuffer.Verify(x => x.Clear(), Times.Never);
        }

        /// <summary>
        /// ExecuteAsync - CancellationTokenが正しく渡されることを確認
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_WithCancellationToken_PassesTokenToMethods()
        {
            // Arrange
            var cancellationToken = new CancellationToken();
            var useCase = new Func<Task>(() => Task.CompletedTask);

            _mockUnitOfWork.Setup(x => x.CommitAsync(cancellationToken)).ReturnsAsync(1);
            _mockEventBuffer.Setup(x => x.FlushAsync(cancellationToken)).Returns(Task.CompletedTask);

            // Act
            await _useCaseExecutor.ExecuteAsync(useCase, cancellationToken);

            // Assert
            _mockUnitOfWork.Verify(x => x.CommitAsync(cancellationToken), Times.Once);
            _mockEventBuffer.Verify(x => x.FlushAsync(cancellationToken), Times.Once);
        }

        /// <summary>
        /// ExecuteAsync - デフォルトのCancellationTokenが使用されることを確認
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_WithoutCancellationToken_UsesDefaultToken()
        {
            // Arrange
            var useCase = new Func<Task>(() => Task.CompletedTask);

            _mockUnitOfWork.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _mockEventBuffer.Setup(x => x.FlushAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            await _useCaseExecutor.ExecuteAsync(useCase);

            // Assert
            _mockUnitOfWork.Verify(x => x.CommitAsync(default), Times.Once);
            _mockEventBuffer.Verify(x => x.FlushAsync(default), Times.Once);
        }

        #endregion

        #region ExecuteAsync Exception Handling Tests

        /// <summary>
        /// ExecuteAsync - ユースケースで例外が発生した場合、イベントバッファがクリアされ例外が再スローされることを確認
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_WhenUseCaseThrowsException_ClearsEventBufferAndRethrows()
        {
            // Arrange
            var expectedException = new InvalidOperationException("UseCase error");
            var useCase = new Func<Task>(() => throw expectedException);

            // Act & Assert
            var actualException = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _useCaseExecutor.ExecuteAsync(useCase));

            Assert.Same(expectedException, actualException);
            _mockEventBuffer.Verify(x => x.Clear(), Times.Once);
            _mockUnitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
            _mockEventBuffer.Verify(x => x.FlushAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        /// <summary>
        /// ExecuteAsync - CommitAsyncで例外が発生した場合、イベントバッファがクリアされ例外が再スローされることを確認
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_WhenCommitThrowsException_ClearsEventBufferAndRethrows()
        {
            // Arrange
            var useCaseExecuted = false;
            var useCase = new Func<Task>(() =>
            {
                useCaseExecuted = true;
                return Task.CompletedTask;
            });

            var expectedException = new InvalidOperationException("Commit error");
            _mockUnitOfWork.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>())).ThrowsAsync(expectedException);

            // Act & Assert
            var actualException = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _useCaseExecutor.ExecuteAsync(useCase));

            Assert.True(useCaseExecuted);
            Assert.Same(expectedException, actualException);
            _mockEventBuffer.Verify(x => x.Clear(), Times.Once);
            _mockUnitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockEventBuffer.Verify(x => x.FlushAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        /// <summary>
        /// ExecuteAsync - FlushAsyncで例外が発生した場合、イベントバッファがクリアされ例外が再スローされることを確認
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_WhenFlushThrowsException_ClearsEventBufferAndRethrows()
        {
            // Arrange
            var useCaseExecuted = false;
            var useCase = new Func<Task>(() =>
            {
                useCaseExecuted = true;
                return Task.CompletedTask;
            });

            var expectedException = new InvalidOperationException("Flush error");
            _mockUnitOfWork.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _mockEventBuffer.Setup(x => x.FlushAsync(It.IsAny<CancellationToken>())).ThrowsAsync(expectedException);

            // Act & Assert
            var actualException = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _useCaseExecutor.ExecuteAsync(useCase));

            Assert.True(useCaseExecuted);
            Assert.Same(expectedException, actualException);
            _mockEventBuffer.Verify(x => x.Clear(), Times.Once);
            _mockUnitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockEventBuffer.Verify(x => x.FlushAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion
    }
}