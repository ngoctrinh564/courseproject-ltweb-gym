using gym.Data;
using gym.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace gym.Controllers
{
    public class TrainersController : Controller
    {
        private readonly GymContext _context;

        public TrainersController(GymContext context)
        {
            _context = context;
        }

        // -------------------- CRUD mặc định --------------------

        public async Task<IActionResult> Index(string? name, string? phone, string? specialty, string? gender)
        {
            var trainers = _context.Trainers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                trainers = trainers.Where(t => t.FullName.Contains(name));

            if (!string.IsNullOrWhiteSpace(phone))
                trainers = trainers.Where(t => t.Phone.Contains(phone));

            if (!string.IsNullOrWhiteSpace(specialty))
                trainers = trainers.Where(t => t.Specialty.Contains(specialty));

            if (!string.IsNullOrEmpty(gender))
            {
                bool? genderBool = gender switch
                {
                    "Nam" => true,
                    "Nữ" => false,
                    "Khác" => null,
                    _ => null
                };

                if (genderBool.HasValue)
                    trainers = trainers.Where(t => t.Gender == genderBool);
                else
                    trainers = trainers.Where(t => t.Gender == null);
            }

            var result = await trainers.ToListAsync();
            return View(result);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainers.FirstOrDefaultAsync(m => m.TrainerId == id);
            if (trainer == null) return NotFound();

            return View(trainer);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Trainer trainer, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/trainer");
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    var filePath = Path.Combine(folderPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    trainer.Image = "/images/trainer/" + fileName;
                }

                _context.Add(trainer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null) return NotFound();

            return View(trainer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        public async Task<IActionResult> EditPost(int id, Trainer trainer, IFormFile ImageFile)
        {
            if (id != trainer.TrainerId) return NotFound();

            var oldTrainer = await _context.Trainers.AsNoTracking().FirstOrDefaultAsync(t => t.TrainerId == id);
            if (oldTrainer == null) return NotFound();

            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/trainer");
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    var filePath = Path.Combine(folderPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    trainer.Image = "images/trainer/" + fileName;
                }
                else
                {
                    trainer.Image = oldTrainer.Image; // giữ ảnh cũ nếu không upload ảnh mới
                }

                try
                {
                    _context.Update(trainer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Trainers.Any(t => t.TrainerId == id))
                        return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(trainer);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainers.FirstOrDefaultAsync(m => m.TrainerId == id);
            if (trainer == null) return NotFound();

            return View(trainer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer != null) _context.Trainers.Remove(trainer);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainerExists(int id)
        {
            return _context.Trainers.Any(e => e.TrainerId == id);
        }

        // -------------------- Chức năng Trainer đăng nhập --------------------

        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> MyMembers()
        {
            int trainerId = GetCurrentTrainerId();

            var members = await _context.TrainingSchedules
                .Include(ts => ts.Member)
                .Where(ts => ts.TrainerId == trainerId)
                .Select(ts => ts.Member)
                .Distinct()
                .ToListAsync();

            return View(members);
        }

        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> MemberHistory(int id)
        {
            var sessions = await _context.TrainingSchedules
                .Where(ts => ts.MemberId == id)
                .OrderByDescending(ts => ts.TrainingDate)
                .ToListAsync();

            return View(sessions);
        }

        private int GetCurrentTrainerId()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                throw new Exception("User chưa đăng nhập");

            var user = _context.Users.FirstOrDefault(u => u.UserName == username && u.RoleId == 4);
            if (user == null)
                throw new Exception("Không tìm thấy user hoặc user không phải trainer");

            return (int)user.ReferenceId;
        }
    }
}
