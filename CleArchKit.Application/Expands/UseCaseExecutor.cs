using CleArchKit.Application.Events;
using CleArchKit.Application.Persistence;

namespace CleArchKit.Application.Expands
{
    /// <inheritdoc/>
    public class UseCaseExecutor : IUseCaseExecutor
    {
        /// <summary>
        /// Unit of Workパターン
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// ドメインイベント管理
        /// </summary>
        private readonly IDomainEventBuffer _eventBuffer;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="eventBuffer"></param>
        public UseCaseExecutor(IUnitOfWork unitOfWork, IDomainEventBuffer eventBuffer)
        {
            _unitOfWork = unitOfWork;
            _eventBuffer = eventBuffer;
        }

        /// <inheritdoc/>
        public async Task ExecuteAsync(Func<Task> useCase, CancellationToken cancellationToken = default)
        {
            try
            {
                // ユースケース実行
                await useCase();

                // コミット
                await _unitOfWork.CommitAsync(cancellationToken);

                // イベント通知
                await _eventBuffer.FlushAsync(cancellationToken);

            }
            catch (Exception ex)
            {
                _eventBuffer.Clear();
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}