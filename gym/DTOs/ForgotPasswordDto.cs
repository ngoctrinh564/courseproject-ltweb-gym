using System.ComponentModel.DataAnnotations;

namespace gym.Models.DTOs;

public class ForgotPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}