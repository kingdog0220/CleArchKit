using CleArchKit.Domain.Users.Entities;

namespace CleArchKit.Application.Users.Dtos
{
    /// <summary>
    /// 登録用ユーザー DTO
    /// </summary>
    public class CreateUserDto
    {
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
        /// DTOからEntityに変換する
        /// </summary>
        /// <returns>DTO</returns>
        public User ToEntity()
        {
            return new User(Guid.NewGuid(), this.Code, this.Name, this.IsActive, DateTime.UtcNow, null);
        }

    }
}