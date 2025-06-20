using System.ComponentModel.DataAnnotations;

namespace gym.ViewModels
{
    public class CreateTrainerViewModel
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        public string TrainerFullName { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        public string TrainerPhone { get; set; }

        public string Specialty { get; set; }
        public string ScheduleNote { get; set; }
        public bool? TrainerGender { get; set; }
    }
    public class CreateMemberViewModel
    {
        [Required] public string UserName { get; set; }
        [Required] public string Password { get; set; }
        [Required, EmailAddress] public string Email { get; set; }

        [Required] public string MemberFullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool? Sex { get; set; }
        public string MemberPhone { get; set; }
        public string MemberAddress { get; set; }
    }

    public class CreateStaffViewModel
    {
        [Required] public string UserName { get; set; }
        [Required] public string Password { get; set; }
        [Required, EmailAddress] public string Email { get; set; }

        [Required] public string StaffFullName { get; set; }
        public string StaffPhone { get; set; }
        public string StaffEmail { get; set; }
    }
}
