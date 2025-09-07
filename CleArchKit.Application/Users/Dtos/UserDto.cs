using CleArchKit.Domain.Users.Entities;

namespace CleArchKit.Application.Users.Dtos
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

        /// <summary>
        /// EntityからDTOに変換する
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>DTO</returns>
        public static UserDto From(User entity) => new UserDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };

        /// <summary>
        /// DTOからEntityに変換する
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>DTO</returns>
        public User ToEntity() => new User(
        Id,
        Code,
        Name,
        IsActive,
        CreatedAt,
        UpdatedAt);

    }
}