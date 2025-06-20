using System;
using System.Collections.Generic;

namespace gym.Data;

public partial class Member
{
    public int MemberId { get; set; }

    public string? FullName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public bool? Sex { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public DateTime? CreateDate { get; set; }

    public virtual ICollection<MemberPakage> MemberPakages { get; set; } = new List<MemberPakage>();

    public virtual ICollection<MemberPayment> MemberPayments { get; set; } = new List<MemberPayment>();

    public virtual ICollection<TrainingSchedule> TrainingSchedules { get; set; } = new List<TrainingSchedule>();
}