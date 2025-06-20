using System;
using System.Collections.Generic;

namespace gym.Data;

public partial class MemberPakage
{
    public int MemberId { get; set; }

    public int PackageId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool? IsPaid { get; set; }

    public bool? IsActive { get; set; }

    public virtual Member Member { get; set; } = null!;

    public virtual Package Package { get; set; } = null!;
    public int PaymentId { get; set; } // nếu bạn dùng bắt buộc
    public Payment Payment { get; set; }
}
