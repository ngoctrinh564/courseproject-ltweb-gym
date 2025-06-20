using System;
using System.Collections.Generic;

namespace gym.Data;

public partial class Notification
{
    public int NotificationId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? SendRole { get; set; }

    public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
}
