using gym.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class NotificationCountViewComponent : ViewComponent
{
    private readonly GymContext _context;

    public NotificationCountViewComponent(GymContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var username = HttpContext.User.Identity?.Name;
        if (string.IsNullOrEmpty(username)) return View(0);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        if (user == null) return View(0);

        var count = await _context.UserNotifications
            .CountAsync(n => n.UserId == user.UserId && n.Seen == false);

        return View(count);
    }
}
