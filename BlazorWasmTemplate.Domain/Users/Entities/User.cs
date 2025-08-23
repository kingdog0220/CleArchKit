using System.ComponentModel.DataAnnotations;

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
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// コード
        /// </summary>
        [Required]
        [MaxLength(10)]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(50)]
        public string? Name { get; set; }

        /// <summary>
        /// 有効/無効フラグ
        /// </summary>
        public bool IsActive { get; set; }
    }
}