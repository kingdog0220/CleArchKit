namespace BlazorWasmTemplate.Application.Users.Dtos
{
    /// <summary>
    /// ユーザー DTO
    /// </summary>
    public class UserDto
    {
        //TODO ここはもうちょい練る。リクエスト/レスポンスと分離するかなど

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

        /// <summary>
        /// 作成日時
        /// </summary>

        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// 更新日時
        /// </summary>

        public DateTime? UpdatedAt { get; set; }
    }
}