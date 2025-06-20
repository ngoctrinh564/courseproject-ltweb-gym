using System;
using System.Collections.Generic;

namespace gym.Data;

public partial class Payment
{
    public int PaymentId { get; set; }
    public decimal Total { get; set; }
    public string? Method { get; set; }
    public string? Note { get; set; }
    public string? Description { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? DueDate { get; set; }
    public virtual ICollection<MemberPayment> MemberPayments { get; set; } = new List<MemberPayment>();
}
