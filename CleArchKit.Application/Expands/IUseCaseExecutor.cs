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
        /// <param name="useCaseFunc"></param>
        /// <typeparam name="TUseCase"></typeparam>
        Task ExecuteAsync<TUseCase>(Func<TUseCase, Task> useCaseFunc) where TUseCase : notnull;

    }
}