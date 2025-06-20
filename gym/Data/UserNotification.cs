using System;
using System.Collections.Generic;

namespace gym.Data;

public partial class UserNotification
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public DateTime? TimeSend { get; set; }

    public bool? Seen { get; set; }

    public virtual Notification Notification { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
