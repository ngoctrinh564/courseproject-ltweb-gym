using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using gym.Data;

namespace gym.Controllers
{
    public class UsersController : Controller
    {
        private readonly GymContext _context;

        public UsersController(GymContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index(string? userName, string? email, string? status, int? roleId)
        {
            var users = _context.Users.Include(u => u.Role).AsQueryable();

            if (!string.IsNullOrWhiteSpace(userName))
                users = users.Where(u => u.UserName!.Contains(userName));

            if (!string.IsNullOrWhiteSpace(email))
                users = users.Where(u => u.Email!.Contains(email));

            if (!string.IsNullOrWhiteSpace(status))
            {
                users = users.Where(u => u.Status != null && u.Status.Trim().ToLower() == status.Trim().ToLower());
            }

            if (roleId.HasValue)
                users = users.Where(u => u.RoleId == roleId.Value);

            ViewBag.Roles = new SelectList(await _context.Roles.ToListAsync(), "RoleId", "RoleName");
            ViewBag.SelectedRoleId = roleId;

            return View(await users.ToListAsync());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateIsActive(int userId, bool isActive)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            user.IsAtive = isActive;
            if (isActive == false)
                user.Status = "Vô hiệu hóa";
            else user.Status = "Hoạt động";
            await _context.SaveChangesAsync();

            return Ok();
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,RoleId,UserName,Password,Email,ReferenceId,Status,IsAtive")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", user.RoleId);
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _context.Roles.ToListAsync();
            if (!roles.Any())
            {
                TempData["FormErrors"] = "Không có vai trò nào trong hệ thống. Vui lòng thêm vai trò trước.";
                return View(user);
            }

            // Debug: Log danh sách roles
            Console.WriteLine($"Số lượng roles: {roles.Count}");
            foreach (var role in roles)
            {
                Console.WriteLine($"RoleId: {role.RoleId}, RoleName: {role.RoleName}");
            }

            // Kiểm tra RoleId của người dùng
            if (!roles.Any(r => r.RoleId == user.RoleId))
            {
                TempData["FormErrors"] = "Vai trò hiện tại của người dùng không tồn tại, đã chọn vai trò mặc định.";
                user.RoleId = roles.FirstOrDefault()?.RoleId ?? 0;
            }

            ViewData["RoleId"] = new SelectList(roles, "RoleId", "RoleName", user.RoleId);
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,RoleId,UserName,Email,ReferenceId,IsAtive,Status")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            // Debug: Log giá trị nhận được
            Console.WriteLine($"UserId: {user.UserId}, RoleId: {user.RoleId}, UserName: {user.UserName}, Email: {user.Email}, ReferenceId: {user.ReferenceId}, IsAtive: {user.IsAtive}, Status: {user.Status}");

            // Kiểm tra ReferenceId hợp lệ dựa trên RoleId
            if (user.RoleId == 2 && user.ReferenceId.HasValue) // Member
            {
                if (!await _context.Members.AnyAsync(m => m.MemberId == user.ReferenceId))
                {
                    ModelState.AddModelError("ReferenceId", "Member ID không tồn tại.");
                }
            }
            else if (user.RoleId == 3 && user.ReferenceId.HasValue) // Staff
            {
                if (!await _context.Staff.AnyAsync(s => s.StaffId == user.ReferenceId))
                {
                    ModelState.AddModelError("ReferenceId", "Staff ID không tồn tại.");
                }
            }
            else if (user.RoleId == 4 && user.ReferenceId.HasValue) // Trainer
            {
                if (!await _context.Trainers.AnyAsync(t => t.TrainerId == user.ReferenceId))
                {
                    ModelState.AddModelError("ReferenceId", "Trainer ID không tồn tại.");
                }
            }
            else if (user.RoleId == 1) // Admin
            {
                user.ReferenceId = null; // Admin không cần ReferenceId
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["FormErrors"] = string.Join("; ", errors);
                Console.WriteLine($"ModelState Errors: {string.Join("; ", errors)}");
                ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
                return View(user);
            }

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            // Kiểm tra RoleId hợp lệ
            if (!await _context.Roles.AnyAsync(r => r.RoleId == user.RoleId))
            {
                ModelState.AddModelError("RoleId", "Vai trò được chọn không tồn tại.");
                TempData["FormErrors"] = "Vai trò được chọn không tồn tại.";
                ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
                return View(user);
            }

            // Cập nhật các trường
            existingUser.UserName = user.UserName;
            existingUser.Email = user.Email;
            existingUser.ReferenceId = user.ReferenceId;
            existingUser.RoleId = user.RoleId;
            existingUser.IsAtive = user.IsAtive;
            existingUser.Status = user.IsAtive == true ? "Hoạt động" : "Vô hiệu hóa";

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["FormErrors"] = $"Lỗi khi lưu dữ liệu: {ex.Message}";
                Console.WriteLine($"SaveChanges Error: {ex.Message}");
                ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
                return View(user);
            }
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
