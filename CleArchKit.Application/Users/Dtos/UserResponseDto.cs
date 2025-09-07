using CleArchKit.Domain.Users.Entities;

namespace CleArchKit.Application.Users.Dtos
{
    /// <summary>
    /// 取得用ユーザー DTO
    /// </summary>
    public class UserResponseDto
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

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// EntityからDTOに変換する
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>DTO</returns>
        public static UserResponseDto From(User entity) => new UserResponseDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };

    }
}