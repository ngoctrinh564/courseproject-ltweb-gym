using System.ComponentModel.DataAnnotations;

namespace gym.Models.DTOs;

public class LoginDto
{
    [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}