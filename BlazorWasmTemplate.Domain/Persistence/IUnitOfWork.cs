namespace BlazorWasmTemplate.Domain.Persistence
{
    /// <summary>
    /// Unit of Workパターンを実装するインターフェース
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// 現在のトランザクション内で行われたすべての変更をデータベースにコミットします。
        /// </summary>
        /// <returns>
        /// データベースに書き込まれた状態エントリの数を表すタスク。
        /// 変更がない場合は0を返します。
        /// </returns>
        Task<int> CommitAsync();
    }
}