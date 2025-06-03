using System.ComponentModel.DataAnnotations;

namespace GarageMasterBE.Models
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string Password { get; set; } = null!;

        [Required]
        [MinLength(3, ErrorMessage = "Tên người dùng phải có ít nhất 3 ký tự.")]
        [MaxLength(50, ErrorMessage = "Tên người dùng không được vượt quá 50 ký tự.")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Tên người dùng chỉ được chứa chữ cái, số và dấu gạch dưới.")]
        public string username { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; } = DateTime.MinValue;
    }
}
