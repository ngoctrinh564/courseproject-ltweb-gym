// Controller: UserController.cs
using gym.Data;
using gym.Services;
using gym.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

[Authorize(Roles = "Admin")]
public class UserController : Controller
{
    private readonly GymContext _context;
    private readonly EmailService _emailService;

    public UserController(GymContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    private string GenerateRandomPassword(int length = 10)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private string PopulateTemplate(string fileName, Dictionary<string, string> data)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "Templates", fileName);
        string content = System.IO.File.ReadAllText(path);
        foreach (var item in data)
        {
            content = content.Replace($"{{{{{item.Key}}}}}", item.Value);
        }
        return content;
    }

    // MEMBER
    public IActionResult Member()
    {
        var vm = new CreateMemberViewModel
        {
            Password = GenerateRandomPassword()
        };
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Member(CreateMemberViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var member = new Member
        {
            FullName = vm.MemberFullName,
            DateOfBirth = vm.DateOfBirth,
            Sex = vm.Sex,
            Phone = vm.MemberPhone,
            Address = vm.MemberAddress,
            CreateDate = DateTime.Now
        };
        _context.Members.Add(member);
        _context.SaveChanges();

        var user = new User
        {
            UserName = vm.UserName,
            Password = BCrypt.Net.BCrypt.HashPassword(vm.Password),
            Email = vm.Email,
            RoleId = 2,
            ReferenceId = member.MemberId,
            Status = "Hoạt động",
            IsAtive = true
        };
        _context.Users.Add(user);
        _context.SaveChanges();

        var replacements = new Dictionary<string, string>
        {
            { "FullName", member.FullName },
            { "Role", "Hội viên" },
            { "UserName", user.UserName },
            { "Password", vm.Password }
        };
        string htmlContent = PopulateTemplate("AccountCreated.html", replacements);
        await _emailService.SendEmailAsync(user.Email, "Tài khoản hội viên", htmlContent);

        return RedirectToAction("Index", "Users");
    }

    // STAFF
    public IActionResult Staff()
    {
        var vm = new CreateStaffViewModel
        {
            Password = GenerateRandomPassword()
        };
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Staff(CreateStaffViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var staff = new Staff
        {
            FullName = vm.StaffFullName,
            Phone = vm.StaffPhone,
            Email = vm.StaffEmail,
            WorkingSince = DateTime.Now
        };
        _context.Staff.Add(staff);
        _context.SaveChanges();

        var user = new User
        {
            UserName = vm.UserName,
            Password = BCrypt.Net.BCrypt.HashPassword(vm.Password),
            Email = vm.Email,
            RoleId = 3,
            ReferenceId = staff.StaffId,
            Status = "Hoạt động",
            IsAtive = true
        };
        _context.Users.Add(user);
        _context.SaveChanges();

        var replacements = new Dictionary<string, string>
        {
            { "FullName", staff.FullName },
            { "Role", "Nhân viên" },
            { "UserName", user.UserName },
            { "Password", vm.Password }
        };
        string htmlContent = PopulateTemplate("AccountCreated.html", replacements);
        await _emailService.SendEmailAsync(user.Email, "Tài khoản nhân viên", htmlContent);

        return RedirectToAction("Index", "Users");
    }

    // TRAINER
    public IActionResult Trainer()
    {
        var vm = new CreateTrainerViewModel
        {
            Password = GenerateRandomPassword()
        };
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Trainer(CreateTrainerViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            foreach (var key in ModelState.Keys)
            {
                var errors = ModelState[key].Errors;
                foreach (var error in errors)
                {
                    Console.WriteLine($"[Model Error] Field: {key} - {error.ErrorMessage}");
                }
            }
            return View(vm);
        }

        var trainer = new Trainer
        {
            FullName = vm.TrainerFullName,
            Phone = vm.TrainerPhone,
            Specialty = vm.Specialty,
            ScheduleNote = vm.ScheduleNote,
            Gender = vm.TrainerGender,
            Image = null,
        };
        _context.Trainers.Add(trainer);
        _context.SaveChanges();

        var user = new User
        {
            UserName = vm.UserName,
            Password = BCrypt.Net.BCrypt.HashPassword(vm.Password),
            Email = vm.Email,
            RoleId = 4,
            ReferenceId = trainer.TrainerId,
            Status = "Hoạt động",
            IsAtive = true
        };
        _context.Users.Add(user);
        _context.SaveChanges();

        var replacements = new Dictionary<string, string>
        {
            { "FullName", trainer.FullName },
            { "Role", "Huấn luyện viên" },
            { "UserName", user.UserName },
            { "Password", vm.Password }
        };

        string htmlContent = PopulateTemplate("AccountCreated.html", replacements);
        await _emailService.SendEmailAsync(user.Email, "Tài khoản huấn luyện viên", htmlContent);

        return RedirectToAction("Index", "Users");
    }
}
