using System;
using System.Collections.Generic;

namespace gym.Data;

public partial class Trainer
{
    public int TrainerId { get; set; }

    public string? FullName { get; set; }

    public string? Phone { get; set; }

    public string? Specialty { get; set; }

    public string? ScheduleNote { get; set; }
    public string? Image { get; set; }
    public bool? Gender { get; set; }
    public virtual ICollection<TrainingSchedule> TrainingSchedules { get; set; } = new List<TrainingSchedule>();
}
