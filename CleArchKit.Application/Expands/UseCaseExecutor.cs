using CleArchKit.Application.Events;
using CleArchKit.Application.Persistence;

namespace CleArchKit.Application.Expands
{
    /// <summary>
    /// ユースケースで共通して行う処理を拡張したラッパー
    /// </summary>
    public class UseCaseExecutor
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

        /// <summary>
        /// ユースケースを実行する
        /// </summary>
        /// <param name="useCase"></param>
        /// <param name="cancellationToken"></param>
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