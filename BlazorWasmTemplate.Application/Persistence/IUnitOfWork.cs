using BlazorWasmTemplate.Domain.Events;

namespace BlazorWasmTemplate.Application.Persistence
{
    /// <summary>
    /// Unit of Workパターンを実装するインターフェース
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// 現在のトランザクション内で行われたすべての変更をデータベースにコミットします。
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// データベースに書き込まれた状態エントリの数を表すタスク。
        /// 変更がない場合は0を返します。
        /// </returns>
        Task<int> CommitAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// ドメインイベントをキューに積む
        /// </summary>
        /// <param name="domainEvent"></param>
        void EnqueueEvent(IDomainEvent domainEvent);
    }
}