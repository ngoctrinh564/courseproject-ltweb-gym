using gym.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class AdminController : Controller
{
    private readonly GymContext _context;

    public AdminController(GymContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var viewModel = new AdminDashboardViewModel
        {
            Dashboard = await LoadDashboardData(),
            RevenueData = await LoadRevenueData(),
            MemberGrowthData = await LoadMemberGrowthData(),
            PackageData = await LoadPackageData(),
            RecentActivities = await LoadRecentActivities()
        };

        return View(viewModel);
    }

    private async Task<DashboardData> LoadDashboardData()
    {
        var totalMembers = await _context.Members.CountAsync();

        var activeMembers = await _context.MemberPakages
            .Where(mp => mp.IsActive == true && mp.EndDate > DateTime.Now)
            .Select(mp => mp.MemberId)
            .Distinct()
            .CountAsync();

        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;

        var monthlyRevenue = await _context.MemberPayments
            .Include(mp => mp.Payment)
            .Where(mp => mp.PaymentDate.HasValue &&
                         mp.PaymentDate.Value.Month == currentMonth &&
                         mp.PaymentDate.Value.Year == currentYear &&
                         mp.Payment.IsPaid)
            .SumAsync(mp => mp.Payment.Total);

        var pendingPayments = await _context.Payments
            .Where(p => !p.IsPaid)
            .CountAsync();

        var activePackages = await _context.Packages
            .Where(p => p.IsActive)
            .CountAsync();

        var todaySchedules = await _context.TrainingSchedules
            .Where(ts => ts.TrainingDate.HasValue &&
                         ts.TrainingDate.Value.Date == DateTime.Today)
            .CountAsync();

        var unreadNotifications = await _context.UserNotifications
            .Where(un => (bool)!un.Seen)
            .CountAsync();

        return new DashboardData
        {
            TotalMembers = totalMembers,
            ActiveMembers = activeMembers,
            MonthlyRevenue = monthlyRevenue,
            PendingPayments = pendingPayments,
            ActivePackages = activePackages,
            ScheduledClasses = todaySchedules,
            Notifications = unreadNotifications
        };
    }

    private async Task<List<RevenueData>> LoadRevenueData()
    {
        var currentYear = DateTime.Now.Year;
        var months = new[] { "T1", "T2", "T3", "T4", "T5", "T6" };
        var result = new List<RevenueData>();

        for (int i = 1; i <= 6; i++)
        {
            var revenue = await _context.MemberPayments
                .Include(mp => mp.Payment)
                .Where(mp => mp.PaymentDate.HasValue &&
                             mp.PaymentDate.Value.Month == i &&
                             mp.PaymentDate.Value.Year == currentYear &&
                             mp.Payment.IsPaid)
                .SumAsync(mp => mp.Payment.Total);

            result.Add(new RevenueData
            {
                Month = months[i - 1],
                Revenue = revenue
            });
        }

        return result;
    }

    private async Task<List<MemberGrowthData>> LoadMemberGrowthData()
    {
        var today = DateTime.Today;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        var result = new List<MemberGrowthData>();

        for (int week = 1; week <= 4; week++)
        {
            var weekStart = startOfMonth.AddDays((week - 1) * 7);
            var weekEnd = weekStart.AddDays(6);

            var memberCount = await _context.Members
                .Where(m => m.CreateDate.HasValue &&
                            m.CreateDate.Value >= weekStart &&
                            m.CreateDate.Value <= weekEnd)
                .CountAsync();

            result.Add(new MemberGrowthData
            {
                Week = $"Tuần {week}",
                Members = memberCount
            });
        }

        return result;
    }

    private async Task<List<PackageData>> LoadPackageData()
    {
        // Truy vấn dữ liệu trước (chưa xử lý màu)
        var packageStats = await _context.MemberPakages
            .Include(mp => mp.Package)
            .Where(mp => mp.IsActive == true)
            .GroupBy(mp => mp.Package.Name)
            .Select(g => new
            {
                Name = g.Key ?? "Không xác định",
                Value = g.Count()
            })
            .ToListAsync();

        // Xử lý thêm màu sau
        var result = packageStats.Select(g => new PackageData
        {
            Name = g.Name,
            Value = g.Value,
            Color = GetPackageColor(g.Name)
        }).ToList();

        return result;
    }


    private async Task<List<RecentActivity>> LoadRecentActivities()
    {
        var activities = new List<RecentActivity>();

        var recentMembersRaw = await _context.Members
            .Where(m => m.CreateDate.HasValue)
            .OrderByDescending(m => m.CreateDate)
            .Take(2)
            .Select(m => new
            {
                m.FullName,
                m.CreateDate
            })
            .ToListAsync();

        var recentMembers = recentMembersRaw.Select(m => new RecentActivity
        {
            Type = "new_member",
            Title = "Thành viên mới đăng ký",
            Description = $"{m.FullName} - {GetTimeAgo(m.CreateDate ?? DateTime.Now)}",
            Color = "blue"
        }).ToList();


        activities.AddRange(recentMembers);

        var recentPaymentsRaw = await _context.MemberPayments
            .Include(mp => mp.Member)
            .Include(mp => mp.Payment)
            .Where(mp => mp.Payment.IsPaid && mp.PaymentDate.HasValue)
            .OrderByDescending(mp => mp.PaymentDate)
            .Take(2)
            .Select(mp => new
            {
                MemberName = mp.Member.FullName,
                PaymentDate = mp.PaymentDate
            })
            .ToListAsync();

        var recentPayments = recentPaymentsRaw.Select(mp => new RecentActivity
        {
            Type = "payment",
            Title = "Thanh toán thành công",
            Description = $"{mp.MemberName} - {GetTimeAgo(mp.PaymentDate ?? DateTime.Now)}",
            Color = "green"
        }).ToList();


        activities.AddRange(recentPayments);

        var recentSchedulesRaw = await _context.TrainingSchedules
            .Include(ts => ts.Member)
            .Include(ts => ts.Trainer)
            .Where(ts => ts.TrainingDate.HasValue)
            .OrderByDescending(ts => ts.TrainingDate)
            .Take(1)
            .Select(ts => new
            {
                MemberName = ts.Member.FullName,
                TrainerName = ts.Trainer.FullName,
                TrainingDate = ts.TrainingDate
            })
            .ToListAsync();

        var recentSchedules = recentSchedulesRaw.Select(ts => new RecentActivity
        {
            Type = "schedule",
            Title = "Lịch tập mới được đặt",
            Description = $"{ts.MemberName} với {ts.TrainerName} - {GetTimeAgo(ts.TrainingDate ?? DateTime.Now)}",
            Color = "purple"
        }).ToList();


        activities.AddRange(recentSchedules);

        var expiringPackages = await _context.MemberPakages
            .Include(mp => mp.Member)
            .Include(mp => mp.Package)
            .Where(mp => mp.IsActive == true &&
                         mp.EndDate.HasValue &&
                         mp.EndDate.Value <= DateTime.Now.AddDays(7) &&
                         mp.EndDate.Value > DateTime.Now)
            .OrderBy(mp => mp.EndDate)
            .Take(1)
            .Select(mp => new RecentActivity
            {
                Type = "expiring",
                Title = "Gói tập sắp hết hạn",
                Description = $"{mp.Member.FullName} - {mp.Package.Name} - {GetTimeAgo(mp.EndDate ?? DateTime.Now)}",
                Color = "red"
            })
            .ToListAsync();

        activities.AddRange(expiringPackages);

        return activities.Take(4).ToList();
    }

    private string GetPackageColor(string packageName)
    {
        return packageName.ToLower() switch
        {
            var name when name.Contains("cơ bản") || name.Contains("basic") => "#3B82F6",
            var name when name.Contains("premium") => "#10B981",
            var name when name.Contains("vip") => "#F59E0B",
            var name when name.Contains("pt") || name.Contains("personal") => "#EF4444",
            _ => "#6B7280"
        };
    }

    private static string GetTimeAgo(DateTime date)
    {
        var timeSpan = DateTime.Now - date;

        if (timeSpan.TotalMinutes < 1)
            return "vừa xong";
        if (timeSpan.TotalMinutes < 60)
            return $"{(int)timeSpan.TotalMinutes} phút trước";
        if (timeSpan.TotalHours < 24)
            return $"{(int)timeSpan.TotalHours} giờ trước";
        if (timeSpan.TotalDays < 7)
            return $"{(int)timeSpan.TotalDays} ngày trước";

        return date.ToString("dd/MM/yyyy");
    }
}
