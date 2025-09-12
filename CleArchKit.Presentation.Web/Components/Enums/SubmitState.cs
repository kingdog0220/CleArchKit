namespace CleArchKit.Presentation.Web.Components.Enums
{
    /// <summary>
    /// 入力状態
    /// </summary>
    public enum SubmitState
    {
        /// <summary>
        /// 入力可能
        /// </summary>
        Idle = 0,

        /// <summary>
        /// 入力中
        /// </summary>
        Submitting = 1,

        /// <summary>
        /// 成功
        /// </summary>
        Success = 2,

        /// <summary>
        /// エラー
        /// </summary>
        Error = 9,
    }
}