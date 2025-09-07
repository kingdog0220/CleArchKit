namespace CleArchKit.Application.Expands
{
    /// <summary>
    /// ユースケースで共通して行う処理を拡張したラッパー
    /// </summary>
    public interface IUseCaseExecutor
    {
        /// <summary>
        /// ユースケースを実行する
        /// </summary>
        /// <param name="useCase"></param>
        /// <param name="cancellationToken"></param>
        Task ExecuteAsync(Func<Task> useCase, CancellationToken cancellationToken = default);
    }
}