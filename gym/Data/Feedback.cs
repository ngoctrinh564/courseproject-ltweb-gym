using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gym.Data
{
    public partial class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung phản hồi.")]
        public string? Content { get; set; }

        public DateTime ThoiGian { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
