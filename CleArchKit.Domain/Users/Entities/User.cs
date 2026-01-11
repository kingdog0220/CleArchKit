namespace CleArchKit.Domain.Users.Entities
{
    /// <summary>
    /// ユーザー
    /// </summary>
    public class User
    {
        /// <summary>
        /// 物理ID
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// コード
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 作成日時
        /// </summary>

        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// 更新日時
        /// </summary>

        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <remarks>ORM用</remarks>
        protected User() { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="createdAt"></param>
        /// <param name="updatedAt"></param>
        public User(Guid id, string code, string? name, DateTime? createdAt, DateTime? updatedAt)
        {
            Id = id;
            Code = code;
            Name = name;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    }
}