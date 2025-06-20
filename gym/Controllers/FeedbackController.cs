using gym.Data;
using gym.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize]
public class FeedbackController : Controller
{
    private readonly GymContext _context;

    public FeedbackController(GymContext context)
    {
        _context = context;
    }

    // GET: Feedback/Create
    public IActionResult Create()
    {
        return View(new Feedback());
    }

    // POST: Feedback/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Content")] Feedback feedback)
    {
        // Xóa các thuộc tính không nhập từ form
        ModelState.Remove("FeedbackId");
        ModelState.Remove("UserId");
        ModelState.Remove("ThoiGian");
        ModelState.Remove("User");

        if (!ModelState.IsValid)
        {
            return View(feedback);
        }

        try
        {
            feedback.UserId = GetCurrentUserId();
            feedback.ThoiGian = DateTime.Now;

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Phản hồi của bạn đã được gửi thành công!";
            return RedirectToAction("Create");
        }
        catch (Exception ex)
        {
            ViewData["Error"] = "Có lỗi xảy ra khi gửi phản hồi: " + ex.Message;
            return View(feedback);
        }
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst("UserId");
        if (claim == null)
            throw new UnauthorizedAccessException("Bạn chưa đăng nhập.");

        return int.Parse(claim.Value);
    }

    [Authorize]
    public async Task<IActionResult> MyFeedbacks()
    {
        try
        {
            int userId = GetCurrentUserId();

            var feedbacks = await _context.Feedbacks
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.ThoiGian)
                .ToListAsync();

            return View(feedbacks);
        }
        catch (Exception ex)
        {
            ViewData["Error"] = "Không thể tải phản hồi của bạn: " + ex.Message;
            return View(new List<Feedback>());
        }
    }
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> List()
    {
        var feedbacks = await _context.Feedbacks
            .Include(f => f.User)
            .OrderByDescending(f => f.ThoiGian)
            .ToListAsync();

        var result = new List<FeedbackListViewModel>();

        foreach (var f in feedbacks)
        {
            string tenNguoiPhanHoi = "";

            switch (f.User.RoleId)
            {
                case 2: // Member
                    var member = await _context.Members.FindAsync(f.User.ReferenceId);
                    tenNguoiPhanHoi = member?.FullName ?? "(Không tìm thấy Member)";
                    break;

                case 3: // Staff
                    var staff = await _context.Staff.FindAsync(f.User.ReferenceId);
                    tenNguoiPhanHoi = staff?.FullName ?? "(Không tìm thấy Staff)";
                    break;

                case 4: // Trainer
                    var trainer = await _context.Trainers.FindAsync(f.User.ReferenceId);
                    tenNguoiPhanHoi = trainer?.FullName ?? "(Không tìm thấy Trainer)";
                    break;

                default:
                    tenNguoiPhanHoi = "(Không rõ)";
                    break;
            }

            result.Add(new FeedbackListViewModel
            {
                TenNguoiPhanHoi = tenNguoiPhanHoi,
                NoiDung = f.Content ?? "",
                ThoiGian = f.ThoiGian
            });
        }

        return View(result);
    }

}
