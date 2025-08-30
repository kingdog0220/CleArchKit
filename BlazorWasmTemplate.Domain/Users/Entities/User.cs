using BlazorWasmTemplate.Domain.Users.Events;

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
        /// 有効/無効フラグ
        /// </summary>
        public bool IsActive { get; set; }

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
        /// <param name="isActive"></param> <summary>
        public User(string code, string? name, bool isActive = true)
        {
            Id = Guid.NewGuid();
            Code = code;
            Name = name;
            IsActive = isActive;
        }

        /// <summary>
        /// ユーザードメインに関するイベント発行
        /// </summary>
        /// <param name="domainEvent"></param>
        public UserUpdatedEvent PublishUserUpdatedEvent()
        {
            return new UserUpdatedEvent(Id);
        }
    }
}