using System;
using System.ComponentModel.DataAnnotations;

public class RegisterDto
{
    // ========== THÔNG TIN TÀI KHOẢN ==========

    [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
    [Display(Name = "Tên đăng nhập")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    [Display(Name = "Xác nhận mật khẩu")]
    public string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
    [Display(Name = "Email")]
    public string Email { get; set; }

    // ========== THÔNG TIN HỘI VIÊN ==========

    [Required(ErrorMessage = "Họ tên không được để trống")]
    [Display(Name = "Họ tên")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Ngày sinh không được để trống")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày sinh")]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn giới tính")]
    [Display(Name = "Giới tính")]
    public bool Sex { get; set; } // true = Nam, false = Nữ

    [Required(ErrorMessage = "Số điện thoại không được để trống")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [Display(Name = "Số điện thoại")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "Địa chỉ không được để trống")]
    [Display(Name = "Địa chỉ")]
    public string Address { get; set; }
}
