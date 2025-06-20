using System;
using System.Collections.Generic;

namespace gym.Data;

public partial class Package
{
    public int PackageId { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public decimal? Price { get; set; }

    public int? DurationInDays { get; set; }

    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ICollection<MemberPakage> MemberPakages { get; set; } = new List<MemberPakage>();
}
