using System.ComponentModel.DataAnnotations;
using CleArchKit.Domain.Users.Entities;

namespace CleArchKit.Application.Users.Dtos
{
    /// <summary>
    /// 更新用ユーザー DTO
    /// </summary>
    public class UpdateUserDto
    {
        /// <summary>
        /// 物理ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// コード
        /// </summary>
        [Required(ErrorMessage = "コードは必須です。")]
        [StringLength(10, ErrorMessage = "コードは10文字以内で入力してください。")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(50, ErrorMessage = "名称は50文字以内で入力してください。")]
        public string? Name { get; set; }

        /// <summary>
        /// 作成日時
        /// </summary>

        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// DTOからEntityに変換する
        /// </summary>
        /// <returns>DTO</returns>
        public User ToEntity()
        {
            return new User(this.Id, this.Code, this.Name, this.CreatedAt, DateTime.UtcNow);
        }
    }
}