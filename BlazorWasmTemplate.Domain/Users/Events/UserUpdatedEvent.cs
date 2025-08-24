namespace BlazorWasmTemplate.Domain.Users.Events
{
    /// <summary>
    /// ユーザー更新イベント
    /// </summary>
    public class UserUpdatedEvent
    {
        /// <summary>
        /// 物理ID
        /// </summary>
        public Guid UserId { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="userId"></param>
        public UserUpdatedEvent(Guid userId)
        {
            UserId = userId;
        }
    }
}