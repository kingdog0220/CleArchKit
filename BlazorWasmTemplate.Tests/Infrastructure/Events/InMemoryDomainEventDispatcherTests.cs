using BlazorWasmTemplate.Domain.Events;
using BlazorWasmTemplate.Infrastructure.Events;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BlazorWasmTemplate.Tests.Infrastructure.Events
{
    /// <summary>
    /// InMemoryDomainEventDispatcherのテストクラス
    /// </summary>
    public class InMemoryDomainEventDispatcherTests
    {
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly InMemoryDomainEventDispatcher _dispatcher;

        /// <summary>
        /// コンストラクタ - テスト用のモックオブジェクトを設定
        /// </summary>
        public InMemoryDomainEventDispatcherTests()
        {
            _mockServiceProvider = new Mock<IServiceProvider>();
            _dispatcher = new InMemoryDomainEventDispatcher(_mockServiceProvider.Object);
        }

        #region Constructor Tests

        /// <summary>
        /// コンストラクタ - 正常なサービスプロバイダーで初期化できることを確認
        /// </summary>
        [Fact]
        public void Constructor_WithValidServiceProvider_CreatesInstance()
        {
            // Arrange & Act
            var dispatcher = new InMemoryDomainEventDispatcher(_mockServiceProvider.Object);

            // Assert
            Assert.NotNull(dispatcher);
        }

        #endregion

        #region DispatchAsync Tests

        /// <summary>
        /// DispatchAsync - 単一のハンドラーが存在する場合、正常に呼び出されることを確認
        /// </summary>
        [Fact]
        public async Task DispatchAsync_WithSingleHandler_CallsHandlerSuccessfully()
        {
            // Arrange
            var testEvent = new TestDomainEvent("Test Message");
            var mockHandler = new Mock<IEventHandler<TestDomainEvent>>();
            var handlers = new[] { mockHandler.Object };

            _mockServiceProvider
                .Setup(x => x.GetService(typeof(IEnumerable<IEventHandler<TestDomainEvent>>)))
                .Returns(handlers);

            // Act
            await _dispatcher.DispatchAsync(testEvent);

            // Assert
            mockHandler.Verify(x => x.HandleAsync(testEvent, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// DispatchAsync - 複数のハンドラーが存在する場合、すべてのハンドラーが呼び出されることを確認
        /// </summary>
        [Fact]
        public async Task DispatchAsync_WithMultipleHandlers_CallsAllHandlers()
        {
            // Arrange
            var testEvent = new TestDomainEvent("Test Message");
            var mockHandler1 = new Mock<IEventHandler<TestDomainEvent>>();
            var mockHandler2 = new Mock<IEventHandler<TestDomainEvent>>();
            var mockHandler3 = new Mock<IEventHandler<TestDomainEvent>>();
            var handlers = new[] { mockHandler1.Object, mockHandler2.Object, mockHandler3.Object };

            _mockServiceProvider
                .Setup(x => x.GetService(typeof(IEnumerable<IEventHandler<TestDomainEvent>>)))
                .Returns(handlers);

            // Act
            await _dispatcher.DispatchAsync(testEvent);

            // Assert
            mockHandler1.Verify(x => x.HandleAsync(testEvent, It.IsAny<CancellationToken>()), Times.Once);
            mockHandler2.Verify(x => x.HandleAsync(testEvent, It.IsAny<CancellationToken>()), Times.Once);
            mockHandler3.Verify(x => x.HandleAsync(testEvent, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// DispatchAsync - ハンドラーが存在しない場合、例外が発生しないことを確認
        /// </summary>
        [Fact]
        public async Task DispatchAsync_WithNoHandlers_DoesNotThrowException()
        {
            // Arrange
            var testEvent = new TestDomainEvent("Test Message");
            var handlers = Array.Empty<IEventHandler<TestDomainEvent>>();

            _mockServiceProvider
                .Setup(x => x.GetService(typeof(IEnumerable<IEventHandler<TestDomainEvent>>)))
                .Returns(handlers);

            // Act & Assert
            var exception = await Record.ExceptionAsync(() => _dispatcher.DispatchAsync(testEvent));
            Assert.Null(exception);
        }


        /// <summary>
        /// DispatchAsync - CancellationTokenが正しく渡されることを確認
        /// </summary>
        [Fact]
        public async Task DispatchAsync_WithCancellationToken_PassesTokenToHandler()
        {
            // Arrange
            var testEvent = new TestDomainEvent("Test Message");
            var mockHandler = new Mock<IEventHandler<TestDomainEvent>>();
            var cancellationToken = new CancellationToken();
            var handlers = new[] { mockHandler.Object };

            _mockServiceProvider
                .Setup(x => x.GetService(typeof(IEnumerable<IEventHandler<TestDomainEvent>>)))
                .Returns(handlers);

            // Act
            await _dispatcher.DispatchAsync(testEvent, cancellationToken);

            // Assert
            mockHandler.Verify(x => x.HandleAsync(testEvent, cancellationToken), Times.Once);
        }

        /// <summary>
        /// DispatchAsync - 異なる型のイベントに対して正しいハンドラー型が要求されることを確認
        /// </summary>
        [Fact]
        public async Task DispatchAsync_WithDifferentEventTypes_RequestsCorrectHandlerType()
        {
            // Arrange
            var testEvent1 = new TestDomainEvent("Test Message 1");
            var testEvent2 = new AnotherTestDomainEvent(42);

            var mockHandler1 = new Mock<IEventHandler<TestDomainEvent>>();
            var mockHandler2 = new Mock<IEventHandler<AnotherTestDomainEvent>>();

            var handlers1 = new[] { mockHandler1.Object };
            var handlers2 = new[] { mockHandler2.Object };

            _mockServiceProvider
                .Setup(x => x.GetService(typeof(IEnumerable<IEventHandler<TestDomainEvent>>)))
                .Returns(handlers1);

            _mockServiceProvider
                .Setup(x => x.GetService(typeof(IEnumerable<IEventHandler<AnotherTestDomainEvent>>)))
                .Returns(handlers2);

            // Act
            await _dispatcher.DispatchAsync(testEvent1);
            await _dispatcher.DispatchAsync(testEvent2);

            // Assert
            _mockServiceProvider.Verify(x => x.GetService(typeof(IEnumerable<IEventHandler<TestDomainEvent>>)), Times.Once);
            _mockServiceProvider.Verify(x => x.GetService(typeof(IEnumerable<IEventHandler<AnotherTestDomainEvent>>)), Times.Once);
            mockHandler1.Verify(x => x.HandleAsync(testEvent1, It.IsAny<CancellationToken>()), Times.Once);
            mockHandler2.Verify(x => x.HandleAsync(testEvent2, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region Error Handling Tests

        /// <summary>
        /// エラーハンドリング - ハンドラーが例外を投げた場合、例外が伝播することを確認
        /// </summary>
        [Fact]
        public async Task DispatchAsync_WhenHandlerThrowsException_PropagatesException()
        {
            // Arrange
            var testEvent = new TestDomainEvent("Test Message");
            var mockHandler = new Mock<IEventHandler<TestDomainEvent>>();
            var handlers = new[] { mockHandler.Object };

            mockHandler
                .Setup(x => x.HandleAsync(It.IsAny<TestDomainEvent>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Handler error"));

            _mockServiceProvider
                .Setup(x => x.GetService(typeof(IEnumerable<IEventHandler<TestDomainEvent>>)))
                .Returns(handlers);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _dispatcher.DispatchAsync(testEvent));
            Assert.Equal("Handler error", exception.Message);
        }

        /// <summary>
        /// エラーハンドリング - 複数のハンドラーのうち1つが例外を投げた場合、最初の例外で停止することを確認
        /// </summary>
        [Fact]
        public async Task DispatchAsync_WhenFirstHandlerThrowsException_StopsAtFirstException()
        {
            // Arrange
            var testEvent = new TestDomainEvent("Test Message");
            var mockHandler1 = new Mock<IEventHandler<TestDomainEvent>>();
            var mockHandler2 = new Mock<IEventHandler<TestDomainEvent>>();
            var handlers = new[] { mockHandler1.Object, mockHandler2.Object };

            mockHandler1
                .Setup(x => x.HandleAsync(It.IsAny<TestDomainEvent>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("First handler error"));

            _mockServiceProvider
                .Setup(x => x.GetService(typeof(IEnumerable<IEventHandler<TestDomainEvent>>)))
                .Returns(handlers);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _dispatcher.DispatchAsync(testEvent));
            Assert.Equal("First handler error", exception.Message);

            // 最初のハンドラーは呼ばれるが、2番目のハンドラーは呼ばれない
            mockHandler1.Verify(x => x.HandleAsync(testEvent, It.IsAny<CancellationToken>()), Times.Once);
            mockHandler2.Verify(x => x.HandleAsync(testEvent, It.IsAny<CancellationToken>()), Times.Never);
        }

        /// <summary>
        /// エラーハンドリング - サービスプロバイダーが例外を投げた場合、例外が伝播することを確認
        /// </summary>
        [Fact]
        public async Task DispatchAsync_WhenServiceProviderThrowsException_PropagatesException()
        {
            // Arrange
            var testEvent = new TestDomainEvent("Test Message");

            _mockServiceProvider
                .Setup(x => x.GetService(typeof(IEnumerable<IEventHandler<TestDomainEvent>>)))
                .Throws(new InvalidOperationException("Service provider error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _dispatcher.DispatchAsync(testEvent));
            Assert.Equal("Service provider error", exception.Message);
        }

        /// <summary>
        /// エラーハンドリング - キャンセルされたトークンで例外が発生することを確認
        /// </summary>
        [Fact]
        public async Task DispatchAsync_WithCancelledToken_ThrowsOperationCancelledException()
        {
            // Arrange
            var testEvent = new TestDomainEvent("Test Message");
            var mockHandler = new Mock<IEventHandler<TestDomainEvent>>();
            var handlers = new[] { mockHandler.Object };
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            mockHandler
                .Setup(x => x.HandleAsync(It.IsAny<TestDomainEvent>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());

            _mockServiceProvider
                .Setup(x => x.GetService(typeof(IEnumerable<IEventHandler<TestDomainEvent>>)))
                .Returns(handlers);

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _dispatcher.DispatchAsync(testEvent, cancellationTokenSource.Token));
        }

        #endregion

        #region Integration Tests

        /// <summary>
        /// 統合テスト - 実際のサービスプロバイダーを使用してディスパッチが動作することを確認
        /// </summary>
        [Fact]
        public async Task Integration_WithRealServiceProvider_WorksCorrectly()
        {
            // Arrange
            var services = new ServiceCollection();
            var testHandler = new TestEventHandler();
            services.AddSingleton<IEventHandler<TestDomainEvent>>(testHandler);

            var serviceProvider = services.BuildServiceProvider();
            var dispatcher = new InMemoryDomainEventDispatcher(serviceProvider);
            var testEvent = new TestDomainEvent("Integration Test");

            // Act
            await dispatcher.DispatchAsync(testEvent);

            // Assert
            Assert.True(testHandler.WasCalled);
            Assert.Equal("Integration Test", testHandler.ReceivedMessage);
        }

        /// <summary>
        /// 統合テスト - 複数の異なる型のハンドラーが正しく動作することを確認
        /// </summary>
        [Fact]
        public async Task Integration_WithMultipleEventTypes_WorksCorrectly()
        {
            // Arrange
            var services = new ServiceCollection();
            var testHandler1 = new TestEventHandler();
            var testHandler2 = new AnotherTestEventHandler();

            services.AddSingleton<IEventHandler<TestDomainEvent>>(testHandler1);
            services.AddSingleton<IEventHandler<AnotherTestDomainEvent>>(testHandler2);

            var serviceProvider = services.BuildServiceProvider();
            var dispatcher = new InMemoryDomainEventDispatcher(serviceProvider);

            var testEvent1 = new TestDomainEvent("Test Message");
            var testEvent2 = new AnotherTestDomainEvent(123);

            // Act
            await dispatcher.DispatchAsync(testEvent1);
            await dispatcher.DispatchAsync(testEvent2);

            // Assert
            Assert.True(testHandler1.WasCalled);
            Assert.Equal("Test Message", testHandler1.ReceivedMessage);
            Assert.True(testHandler2.WasCalled);
            Assert.Equal(123, testHandler2.ReceivedValue);
        }

        #endregion
    }

    #region Test Helper Classes

    /// <summary>
    /// テスト用ドメインイベント
    /// </summary>
    public class TestDomainEvent : IDomainEvent
    {
        public string Message { get; }

        public TestDomainEvent(string message)
        {
            Message = message;
        }
    }

    /// <summary>
    /// 別のテスト用ドメインイベント
    /// </summary>
    public class AnotherTestDomainEvent : IDomainEvent
    {
        public int Value { get; }

        public AnotherTestDomainEvent(int value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// テスト用イベントハンドラー
    /// </summary>
    public class TestEventHandler : IEventHandler<TestDomainEvent>
    {
        public bool WasCalled { get; private set; }
        public string? ReceivedMessage { get; private set; }

        public Task HandleAsync(TestDomainEvent @event, CancellationToken cancellationToken = default)
        {
            WasCalled = true;
            ReceivedMessage = @event.Message;
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// 別のテスト用イベントハンドラー
    /// </summary>
    public class AnotherTestEventHandler : IEventHandler<AnotherTestDomainEvent>
    {
        public bool WasCalled { get; private set; }
        public int ReceivedValue { get; private set; }

        public Task HandleAsync(AnotherTestDomainEvent @event, CancellationToken cancellationToken = default)
        {
            WasCalled = true;
            ReceivedValue = @event.Value;
            return Task.CompletedTask;
        }
    }

    #endregion
}