using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace gym.Data;

public partial class User
{
    public int UserId { get; set; }
    [Required(ErrorMessage = "Vai trò là bắt buộc.")]
    public int RoleId { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }
    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    public string? Email { get; set; }

    public int? ReferenceId { get; set; }

    public string? Status { get; set; }

    public bool? IsAtive { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}
