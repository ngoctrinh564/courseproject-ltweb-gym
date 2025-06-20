using System.ComponentModel;

public class AdminDashboardViewModel
{
    public DashboardData Dashboard { get; set; } = new(); // Bổ sung khởi tạo mặc định
    public List<RevenueData> RevenueData { get; set; } = new();
    public List<MemberGrowthData> MemberGrowthData { get; set; } = new();
    public List<PackageData> PackageData { get; set; } = new();
    public List<RecentActivity> RecentActivities { get; set; } = new();
}


public class DashboardData
{
    public int TotalMembers { get; set; }
    public int ActiveMembers { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public int PendingPayments { get; set; }
    public int ActivePackages { get; set; }
    public int ScheduledClasses { get; set; }
    public int Notifications { get; set; }
}

public class RevenueData
{
    [DisplayName("Tháng")]
    public string Month { get; set; } = "";

    [DisplayName("Doanh thu")]
    public decimal Revenue { get; set; }
}

public class MemberGrowthData
{
    [DisplayName("Tuần")]
    public string Week { get; set; } = "";

    [DisplayName("Số thành viên")]
    public int Members { get; set; }
}

public class PackageData
{
    [DisplayName("Tên gói")]
    public string Name { get; set; } = "";

    [DisplayName("Số lượng")]
    public int Value { get; set; }

    public string Color { get; set; } = "";
}

public class RecentActivity
{
    public string Type { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Color { get; set; } = "";
}
