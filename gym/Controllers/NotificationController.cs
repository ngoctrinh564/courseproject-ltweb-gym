using gym.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gym.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly GymContext _context;

        public NotificationController(GymContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> List()
        {
            var username = User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null) return Unauthorized();

            var notifications = await _context.UserNotifications
                .Include(un => un.Notification)
                .Where(un => un.UserId == user.UserId)
                .OrderByDescending(un => un.TimeSend)
                .ToListAsync();

            return View(notifications);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsSeen(int id)
        {
            var username = User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null) return Unauthorized();

            var userNotification = await _context.UserNotifications
                .Include(un => un.Notification)
                .FirstOrDefaultAsync(un => un.UserId == user.UserId && un.NotificationId == id);

            if (userNotification == null) return NotFound();

            userNotification.Seen = true;
            await _context.SaveChangesAsync();

            var result = new
            {
                title = userNotification.Notification.Title,
                content = userNotification.Notification.Content,
                createdAt = userNotification.Notification.CreatedAt
            };

            return PartialView("_NotificationDetailPartial", userNotification.Notification);
        }

    }
}
