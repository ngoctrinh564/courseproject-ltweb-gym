using System;
using System.Collections.Generic;

namespace gym.Data;

public partial class MemberPayment
{
    public int MemberId { get; set; }

    public int PaymentId { get; set; }

    public int? StaffId { get; set; }

    public DateTime? PaymentDate { get; set; }

    public virtual Member Member { get; set; } = null!;

    public virtual Payment Payment { get; set; } = null!;

    public virtual Staff? Staff { get; set; }
}
