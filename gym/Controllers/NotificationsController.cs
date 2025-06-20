using gym.Data;
using gym.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gym.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly GymContext _context;

        public NotificationsController(GymContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Lấy toàn bộ Users trừ Admin
            var users = await _context.Users
                .Where(u => u.RoleId != 1 && u.IsAtive == true)
                .ToListAsync();

            // Load trước các bảng Member/Staff/Trainer vào Dictionary để truy xuất nhanh
            var members = await _context.Members.ToDictionaryAsync(m => m.MemberId);
            var staffs = await _context.Staff.ToDictionaryAsync(s => s.StaffId);
            var trainers = await _context.Trainers.ToDictionaryAsync(t => t.TrainerId);

            var userSelections = new List<UserSelection>();

            foreach (var user in users)
            {
                string fullName = "";

                switch (user.RoleId)
                {
                    case 2: // Member
                        if (members.TryGetValue((int)user.ReferenceId, out var member))
                            fullName = member.FullName ?? "";
                        break;

                    case 3: // Staff
                        if (staffs.TryGetValue((int)user.ReferenceId, out var staff))
                            fullName = staff.FullName ?? "";
                        break;

                    case 4: // Trainer
                        if (trainers.TryGetValue((int)user.ReferenceId, out var trainer))
                            fullName = trainer.FullName ?? "";
                        break;
                }

                userSelections.Add(new UserSelection
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    FullName = fullName,
                    Email = user.Email,
                    RoleId = user.RoleId,
                    IsSelected = false
                });
            }

            var model = new NotificationCreateViewModel
            {
                Users = userSelections
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NotificationCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Kiểm tra danh sách người dùng có chọn ít nhất một người hay không
            if (model.Users == null || !model.Users.Any(u => u.IsSelected))
            {
                ModelState.AddModelError("", "Vui lòng chọn ít nhất một người dùng để gửi thông báo.");
                return View(model);
            }

            // Tạo thông báo
            var notification = new Notification
            {
                Title = model.Title,
                Content = model.Content,
                CreatedAt = DateTime.Now,
                SendRole = model.SendRole
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync(); // Lưu để lấy NotificationId

            // Lấy danh sách user được chọn
            var selectedUsers = model.Users.Where(u => u.IsSelected).ToList();

            // Tạo danh sách UserNotification
            var userNotifications = selectedUsers.Select(user => new UserNotification
            {
                NotificationId = notification.NotificationId,
                UserId = user.UserId,
                TimeSend = DateTime.Now,
                Seen = false
            }).ToList();

            _context.UserNotifications.AddRange(userNotifications);
            await _context.SaveChangesAsync();

            // Gửi thông báo thành công
            TempData["Success"] = "Thông báo đã được gửi đến người dùng thành công.";

            return RedirectToAction("NotificationsSent");
        }
        public async Task<IActionResult> NotificationsSent()
        {
            var users = await _context.Users.ToListAsync();
            var members = await _context.Members.ToDictionaryAsync(m => m.MemberId);
            var staffs = await _context.Staff.ToDictionaryAsync(s => s.StaffId);
            var trainers = await _context.Trainers.ToDictionaryAsync(t => t.TrainerId);

            var sent = await _context.UserNotifications
                .Include(un => un.Notification)
                .Include(un => un.User)
                .ToListAsync();

            var result = new List<SentNotificationViewModel>();

            foreach (var item in sent)
            {
                string fullName = "";
                switch (item.User.RoleId)
                {
                    case 2:
                        members.TryGetValue((int)item.User.ReferenceId, out var member);
                        fullName = member?.FullName ?? "";
                        break;
                    case 3:
                        staffs.TryGetValue((int)item.User.ReferenceId, out var staff);
                        fullName = staff?.FullName ?? "";
                        break;
                    case 4:
                        trainers.TryGetValue((int)item.User.ReferenceId, out var trainer);
                        fullName = trainer?.FullName ?? "";
                        break;
                }

                result.Add(new SentNotificationViewModel
                {
                    Title = item.Notification.Title,
                    Content = item.Notification.Content,
                    CreatedAt = item.Notification.CreatedAt,
                    UserName = item.User.UserName,
                    FullName = fullName
                });
            }

            return View(result);
        }

    }
}
