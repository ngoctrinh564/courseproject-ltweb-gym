using System;
using System.Collections.Generic;

namespace gym.Data;

public partial class TrainingSchedule
{
    public int TrainerId { get; set; }

    public int MemberId { get; set; }

    public DateTime? TrainingDate { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public string? Node { get; set; }

    public virtual Member Member { get; set; } = null!;

    public virtual Trainer Trainer { get; set; } = null!;
}
