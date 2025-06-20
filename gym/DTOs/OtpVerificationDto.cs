using System.ComponentModel.DataAnnotations;

public class OtpVerificationDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã OTP phải có đúng 6 chữ số")]
    public string OtpCode { get; set; }
}
