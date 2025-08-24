namespace BlazorWasmTemplate.Domain.Users.Entities
{
    /// <summary>
    /// ユーザー
    /// </summary>
    public class User
    {
        /// <summary>
        /// 物理ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// コード
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 有効/無効フラグ
        /// </summary>
        public bool IsActive { get; set; }
    }
}