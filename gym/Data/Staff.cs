using System;
using System.Collections.Generic;

namespace gym.Data;

public partial class Staff
{
    public int StaffId { get; set; }

    public string? FullName { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public DateTime? WorkingSince { get; set; }

    public virtual ICollection<MemberPayment> MemberPayments { get; set; } = new List<MemberPayment>();
}
